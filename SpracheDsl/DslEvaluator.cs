using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public class DslEvaluator
    {
        public ExecContext Context { get; } = new ExecContext();
        static IDictionary<int, IList<Argument>> DslCache = new Dictionary<int, IList<Argument>>();

        static IDictionary<int, IList<List<DslAttribute>>> AttrCache = new Dictionary<int, IList<List<DslAttribute>>>();

        public Line Line
        {
            get { return Context.Line; }
            set { Context.Line = value; }
        }

        public IDictionary<string, ResultValue> ParamBag
        {
            get { return Context.ParamBag; }
            set { Context.ParamBag = value; }
        }

        public string JurisdictionType
        {
            get { return Context.JurisdictionType; }
            set { Context.JurisdictionType = value; }
        }

        public DslEvaluator()
        {
        }

        public DslEvaluator(ExecContext context)
        {
            Line = context.Line;
            ParamBag = context.ParamBag;
            JurisdictionType = context.JurisdictionType;
        }

        public IList<ResultValue> Eval(string dsl, bool forceReparse = false)
        {
            var hash = dsl.GetHashCode();
            EvalInternal(dsl, hash, forceReparse);

            return Reduce(Context, DslCache[hash])
                .Select(a => a.ToResultValue()).ToList();
        }

        public IList<List<DslAttribute>> Attributes(string dsl, bool forceReparse = false)
        {
            var hash = dsl.GetHashCode();
            EvalInternal(dsl, hash, forceReparse);

            return AttrCache[hash];
        }

        private void EvalInternal(string dsl, int hash, bool forceReparse = false)
        {
            if (forceReparse || !DslCache.ContainsKey(hash))
            {
                var stripped = StripComments(dsl);
                var fullRules = DslGrammar.CompoundRule.Parse(stripped);

                DslCache[hash] = fullRules.Select(fr => Argument.AsFunctionCall(fr.Invocation)).ToList();
                AttrCache[hash] = fullRules.Select(fr => ReduceAttributes(fr.Attributes).ToList()).ToList();
            }
        }

        public static IEnumerable<Argument> Reduce(ExecContext context, IEnumerable<Argument> args)
            => args.Select(a => Reduce(context, a));

        public static Argument Reduce(ExecContext context, Argument arg)
        {
            if (arg.Type == ArgumentTypes.FunctionCall)
            {
                var invocation = arg.FuncInvocation;

                var result = Functions.Apply(context, invocation);

                return result;
            }
            else if (arg.Type == ArgumentTypes.Variable)
            {
                if (context.ParamBag.ContainsKey(arg.Id))
                {
                    return context.ParamBag[arg.Id].ToArgument();
                }
                else
                {
                    throw new ArgumentException($"no variable @{arg.Id} in ParamBag");
                }
            }
            else if (arg.Type == ArgumentTypes.Dsl)
            {
                var result = new DslEvaluator(context).Eval(arg.Dsl);
                // only return the first result, since nested functions aren't compound (because they're expressions)
                return result.First().ToArgument();
            }

            return arg;
        }

        private IEnumerable<DslAttribute> ReduceAttributes(IEnumerable<DslAttribute> attrs)
        {
            var map = new Dictionary<string, DslAttribute>();

            foreach (var attr in attrs)
            {
                if (map.ContainsKey(attr.Name))
                {
                    map[attr.Name].Values.Add(attr.Values.First());
                }
                else
                {
                    map[attr.Name] = attr;
                }
            }

            return map.Values;
        }

        private string StripComments(string dsl)
        {
            dsl = Regex.Replace(dsl, @"//.*$", "", RegexOptions.Multiline);
            dsl = Regex.Replace(dsl, @"/\*.*?\*/", "");

            return dsl;
        }

        private string StripAttributes(string dsl)
        {
            dsl = Regex.Replace(dsl, @"\[.*?\]", "");

            return dsl;
        }
    }
}

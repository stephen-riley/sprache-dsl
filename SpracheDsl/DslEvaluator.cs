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
        static IDictionary<int, Argument> DslCache = new Dictionary<int, Argument>();

        static IDictionary<int, IList<DslAttribute>> AttrCache = new Dictionary<int, IList<DslAttribute>>();

        public Line Line { get; set; }

        public IDictionary<string, ResultValue> ParamBag { get; set; }

        public string JurisdictionType { get; set; }

        public DslEvaluator()
        {
        }

        public DslEvaluator(ExecContext context)
        {
            Line = context.Line;
            ParamBag = context.ParamBag;
            JurisdictionType = context.JurisdictionType;
        }

        public ResultValue Eval(string dsl, bool forceReparse = false)
        {
            var hash = dsl.GetHashCode();
            EvalInternal(dsl, hash, forceReparse);

            return Reduce(new ExecContext { Line = Line, ParamBag = ParamBag, JurisdictionType = JurisdictionType }, DslCache[hash]).ToResultValue();
        }

        public IList<DslAttribute> Attributes(string dsl, bool forceReparse = false)
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
                var fullRule = DslGrammar.FullRule.Parse(stripped);

                DslCache[hash] = Argument.AsFunctionCall(fullRule.Invocation);
                AttrCache[hash] = ReduceAttributes(fullRule.Attributes).ToList();
            }
        }

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
                return result.ToArgument();
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

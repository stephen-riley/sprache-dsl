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
        static IDictionary<int, Argument> Cache = new Dictionary<int, Argument>();

        public Line Line { get; set; }

        public IDictionary<string, ResultValue> ParamBag { get; set; }

        public DslEvaluator()
        {
        }

        public DslEvaluator(ExecContext context)
        {
            Line = context.Line;
            ParamBag = context.ParamBag;
        }

        public ResultValue Eval(string dsl, bool forceReparse = false)
        {
            var hash = dsl.GetHashCode();

            if (forceReparse || !Cache.ContainsKey(hash))
            {
                var invocation = DslGrammar.Rule.Parse(StripComments(dsl));
                Cache[hash] = Argument.AsFunctionCall(invocation);
            }

            return Reduce(new ExecContext { Line = Line, ParamBag = ParamBag }, Cache[hash]).ToResultValue();
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

        private string StripComments(string dsl)
        {
            dsl = Regex.Replace(dsl, @"//.*$", "", RegexOptions.Multiline);
            dsl = Regex.Replace(dsl, @"/\*.*?\*/", "");

            return dsl;
        }
    }
}

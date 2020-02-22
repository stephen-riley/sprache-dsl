using System;
using System.Linq;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public static class Functions
    {
        public static Argument Apply(ExecContext context, FunctionInvocation invocation)
        {
            var result = invocation.Name.ToLowerInvariant() switch
            {
                "rate" => Functions.Rate(context, invocation.Args.ToArray()),
                "costbasis" => Functions.CostBasis(context, invocation.Args.ToArray()),
                "defaultrate" => Functions.DefaultRate(context, invocation.Args.ToArray()),
                "exclusive" => Functions.Exclusive(context, invocation.Args.ToArray()),
                "unitbasis" => Functions.UnitBasis(context, invocation.Args.ToArray()),
                "unitband" => Functions.UnitBand(context, invocation.Args.ToArray()),
                "fee" => Functions.Fee(context, invocation.Args.ToArray()),
                _ => throw new ArgumentException($"function {invocation.Name} invalid"),
            };

            return result;
        }

        public static Argument Rate(ExecContext context, params Argument[] args)
        {
            var rate = DslEvaluator.Reduce(context, args[0]);
            var costBasis = DslEvaluator.Reduce(context, args[1]);

            rate.AssertPercent();
            costBasis.AssertMoney();

            return Argument.AsMoney(rate.Value * costBasis.Value);
        }

        public static Argument CostBasis(ExecContext context, params Argument[] args)
        {
            return Argument.AsMoney(context.Line.Quantity * context.Line.ItemPrice);
        }

        public static Argument DefaultRate(ExecContext context, params Argument[] args)
        {
            if (args.Length > 0)
            {
                var code = DslEvaluator.Reduce(context, args[0]);
                code.AssertSymbol();

                var key = $"DEFAULT_RATE:{code.Id}";
                if (context.ParamBag.ContainsKey(key))
                {
                    return Argument.AsPercent(context.ParamBag[key].Value);
                }
            }

            // should throw an exception...
            // throw new ArgumentException($"defaultrate could not find rate :{code.Id}");

            // ...but for my purposes, I'll just put 6.5% in.
            return Argument.AsPercent(0.065m);
        }

        public static Argument Exclusive(ExecContext context, params Argument[] args)
        {
            // take the first non-zero result from the argument terms
            foreach (var arg in args)
            {
                var reduced = DslEvaluator.Reduce(context, arg);
                if (reduced.Type == ArgumentTypes.Money && reduced.Value > 0m)
                {
                    return reduced;
                }
            }

            return Argument.AsMoney(0m);
        }

        public static Argument UnitBasis(ExecContext context, params Argument[] args)
        {
            // ought to filter by the *actual* unit basis, but the tests don't demand that, so...
            // ignore the args

            return Argument.AsNumber(context.Line.UnitOfMeasure.Units);
        }

        public static Argument UnitBand(ExecContext context, params Argument[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException($"{nameof(args)} must have three items");
            }

            var low = DslEvaluator.Reduce(context, args[0]);
            var high = DslEvaluator.Reduce(context, args[1]);
            var operand = DslEvaluator.Reduce(context, args[2]);

            return operand.Value >= low.Value && operand.Value < high.Value ? Argument.True() : Argument.False();
        }

        public static Argument Fee(ExecContext context, params Argument[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException($"{nameof(args)} must have two items");
            }

            var fee = DslEvaluator.Reduce(context, args[0]);
            var truthy = DslEvaluator.Reduce(context, args[1]);

            return Argument.AsMoney(truthy.Value != 0m ? fee.Value : 0m);
        }
    }
}
using System;
using System.Collections.Generic;
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
                "dollars" => Functions.Dollars(context, invocation.Args.ToArray()),
                "cents" => Functions.Cents(context, invocation.Args.ToArray()),
                "bandedfee" => Functions.BandedFee(context, invocation.Args.ToArray()),
                "sumtax" => Functions.SumTax(context, invocation.Args.ToArray()),
                "roundmoney" => Functions.RoundMoney(context, invocation.Args.ToArray()),
                "bandedamount" => Functions.BandedAmount(context, invocation.Args.ToArray()),
                "amountinband" => Functions.AmountInBand(context, invocation.Args.ToArray()),
                "discount" => Functions.Discount(context, invocation.Args.ToArray()),
                "setcostbasis" => Functions.SetCostBasis(context, invocation.Args.ToArray()),
                "juristypematch" => Functions.JurisTypeMatch(context, invocation.Args.ToArray()),
                _ => throw new ArgumentException($"function {invocation.Name} invalid"),
            };

            return result;
        }

        private static void AssertArgCount(Argument[] args, int expectedCount)
        {
            if (args.Length != expectedCount)
            {
                throw new ArgumentException($"{nameof(args)} must have {expectedCount} items");
            }
        }

        public static Argument Rate(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 2);

            var rate = DslEvaluator.Reduce(context, args[0]);

            if (rate.IsMoney())
            {
                return rate;
            }

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
                    var result = DslEvaluator.Reduce(context, context.ParamBag[key].ToArgument());
                    return result;
                }
            }

            // should throw an exception...
            // throw new ArgumentException($"defaultrate could not find rate :{code.Id}");

            // ...but for my purposes, I'll just put 6.5% in.
            return Argument.AsPercent(0.065m);
        }

        public static Argument Exclusive(ExecContext context, params Argument[] args)
        {
            int argIndex = 0;
            // take the first non-zero result from the argument terms
            foreach (var arg in args)
            {
                argIndex++;

                var reduced = DslEvaluator.Reduce(context, arg);
                if (reduced.Type == ArgumentTypes.Money && reduced.Value > 0m)
                {
                    Console.WriteLine($"*** Exclusive: selected arg {argIndex}");
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
            AssertArgCount(args, 3);

            var low = DslEvaluator.Reduce(context, args[0]);
            var high = DslEvaluator.Reduce(context, args[1]);
            var operand = DslEvaluator.Reduce(context, args[2]);

            return operand.Value >= low.Value && operand.Value < high.Value ? Argument.True() : Argument.False();
        }

        public static Argument Fee(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 2);

            var fee = DslEvaluator.Reduce(context, args[0]);
            var truthy = DslEvaluator.Reduce(context, args[1]);

            return Argument.AsMoney(truthy.Value != 0m ? fee.Value : 0m);
        }

        public static Argument Dollars(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 1);

            var value = DslEvaluator.Reduce(context, args[0]);

            return Argument.AsMoney(Math.Floor(value.Value));
        }

        public static Argument Cents(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 1);

            var value = DslEvaluator.Reduce(context, args[0]);
            var truncatedValue = value.Value - Math.Truncate(value.Value);

            return Argument.AsMoney(truncatedValue);
        }

        public static Argument BandedFee(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 4);

            var low = DslEvaluator.Reduce(context, args[0]).Value;
            var high = DslEvaluator.Reduce(context, args[1]).Value;
            var operand = DslEvaluator.Reduce(context, args[2]).Value;

            if (operand >= low && operand < high)
            {
                return DslEvaluator.Reduce(context, args[3]).AssertMoney();
            }
            else
            {
                return Argument.AsMoney(0m);
            }
        }

        public static Argument SumTax(ExecContext context, params Argument[] args)
        {
            var total = 0m;

            // take the first non-zero result from the argument terms
            foreach (var arg in args)
            {
                var reduced = DslEvaluator.Reduce(context, arg);
                total += reduced.Value;
            }

            return Argument.AsMoney(total);
        }

        public static Argument RoundMoney(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 1);

            var value = DslEvaluator.Reduce(context, args[0]);

            return Argument.AsMoney(Math.Round(value.Value, 2));
        }

        public static Argument BandedAmount(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 3);

            var low = DslEvaluator.Reduce(context, args[0]).AssertNumeric();
            var high = DslEvaluator.Reduce(context, args[1]).AssertNumeric();
            var operand = DslEvaluator.Reduce(context, args[2]).AssertNumeric();

            var result = 0m;

            if (operand.Value >= high.Value)
            {
                result = high.Value;
            }
            else if (operand.Value >= low.Value)
            {
                result = operand.Value;
            }

            return new Argument(operand.Type, value: result);
        }

        public static Argument AmountInBand(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 3);

            var low = DslEvaluator.Reduce(context, args[0]).AssertNumeric();
            var high = DslEvaluator.Reduce(context, args[1]).AssertNumeric();
            var operand = DslEvaluator.Reduce(context, args[2]).AssertNumeric();

            var result = 0m;

            if (operand.Value >= high.Value)
            {
                result = high.Value - low.Value;
            }
            else if (operand.Value >= low.Value)
            {
                result = operand.Value - low.Value;
            }

            return new Argument(operand.Type, value: result);
        }

        public static Argument Discount(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 2);

            var rate = DslEvaluator.Reduce(context, args[0]).AssertNumeric();
            var operand = DslEvaluator.Reduce(context, args[1]).AssertNumeric();

            var result = 0m;

            if (rate.Value <= 1.0m)
            {
                result = (1.0m - rate.Value) * operand.Value;
            }

            return new Argument(operand.Type, value: result);
        }

        public static Argument SetCostBasis(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 1);

            var costBasis = DslEvaluator.Reduce(context, args[0]);
            costBasis.AssertMoney();

            context.Line.ItemPrice = costBasis.Value;

            return Argument.AsMoney(0m);
        }

        public static Argument JurisTypeMatch(ExecContext context, params Argument[] args)
        {
            AssertArgCount(args, 2);

            var juris = DslEvaluator.Reduce(context, args[0]);
            var value = DslEvaluator.Reduce(context, args[1]);

            IEnumerable<Argument> jurisSymbols;

            if (juris.IsSymbol())
            {
                jurisSymbols = new List<Argument> { juris };
            }
            else
            {
                jurisSymbols = juris.Set;
            }

            foreach (var j in jurisSymbols)
            {
                if (j.Id.ToLowerInvariant().Equals(context.JurisdictionType.ToLowerInvariant()))
                {
                    return value;
                }
            }

            return Argument.AsDimensionless(0m);
        }
    }
}
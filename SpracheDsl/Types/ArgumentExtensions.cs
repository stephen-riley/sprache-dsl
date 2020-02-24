using System;
using System.Collections.Generic;

namespace SpracheDsl.Types
{
    using static ArgumentTypes;

    public static class ArgumentExtensions
    {
        public static Argument AssertType(this Argument arg, ArgumentTypes type)
        {
            if (arg.Type != type)
            {
                throw new ArgumentException($"type was not correct (expected {type.ToString()})", nameof(arg));
            }

            return arg;
        }

        public static Argument AssertNumeric(this Argument arg)
        {
            if (arg.Type != Percent && arg.Type != Money && arg.Type != Number)
            {
                throw new ArgumentException($"type was not correct (expected {arg.Type.ToString()})", nameof(arg));
            }

            return arg;
        }

        public static Argument AssertPercent(this Argument arg) => AssertType(arg, Percent);
        public static Argument AssertMoney(this Argument arg) => AssertType(arg, Money);
        public static Argument AssertVariable(this Argument arg) => AssertType(arg, Variable);
        public static Argument AssertSymbol(this Argument arg) => AssertType(arg, Symbol);

        public static bool IsType(this Argument arg, ArgumentTypes type) => arg.Type == type;

        public static bool IsPercent(this Argument arg) => arg.Type == Percent;
        public static bool IsMoney(this Argument arg) => arg.Type == Money;
        public static bool IsVariable(this Argument arg) => arg.Type == Variable;
        public static bool IsSymbol(this Argument arg) => arg.Type == Symbol;
        public static bool IsFuncInvocation(this Argument arg) => arg.Type == FunctionCall;
        public static bool IsBoolean(this Argument arg) => arg.Type == Bool;

        public static ResultValue ToResultValue(this Argument arg)
        {
            IDictionary<ArgumentTypes, ResultTypes> typeMap = new Dictionary<ArgumentTypes, ResultTypes>
            {
                [Percent] = ResultTypes.Percent,
                [Money] = ResultTypes.Money,
                [Dimensionless] = ResultTypes.Dimensionless,
            };

            if (!typeMap.ContainsKey(arg.Type))
            {
                throw new ArgumentException($"arg.Type cannot be {arg.Type.ToString()}");
            }

            return new ResultValue(typeMap[arg.Type], arg.Value);
        }
    }
}
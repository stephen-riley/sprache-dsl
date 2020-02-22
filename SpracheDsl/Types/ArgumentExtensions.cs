using System;
using System.Collections.Generic;

namespace SpracheDsl.Types
{
    using static ArgumentTypes;

    public static class ArgumentExtensions
    {
        public static void AssertType(this Argument arg, ArgumentTypes type)
        {
            if (arg.Type != type)
            {
                throw new ArgumentException($"type was not correct (expected {type.ToString()})", nameof(arg));
            }
        }

        public static void AssertPercent(this Argument arg) => AssertType(arg, Percent);
        public static void AssertMoney(this Argument arg) => AssertType(arg, Money);
        public static void AssertVariable(this Argument arg) => AssertType(arg, Variable);
        public static void AssertSymbol(this Argument arg) => AssertType(arg, Symbol);

        public static bool IsType(this Argument arg, ArgumentTypes type) => arg.Type == type;

        public static bool IsPercent(this Argument arg) => arg.Type == Percent;
        public static bool IsMoney(this Argument arg) => arg.Type == Money;
        public static bool IsVariable(this Argument arg) => arg.Type == Variable;
        public static bool IsSymbol(this Argument arg) => arg.Type == Symbol;
        public static bool IsFuncInvocation(this Argument arg) => arg.Type == FunctionCall;

        public static ResultValue ToResultValue(this Argument arg)
        {
            IDictionary<ArgumentTypes, ResultTypes> typeMap = new Dictionary<ArgumentTypes, ResultTypes>
            {
                [Percent] = ResultTypes.Percent,
                [Money] = ResultTypes.Money,
            };

            if (!typeMap.ContainsKey(arg.Type))
            {
                throw new ArgumentException($"arg.Type cannot be {arg.Type.ToString()}");
            }

            return new ResultValue(typeMap[arg.Type], arg.Value);
        }
    }
}
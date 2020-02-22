using System;

namespace SpracheDsl.Types
{
    public static class ResultValueExtensions
    {
        public static Argument ToArgument(this ResultValue resultValue)
        {
            return resultValue.Unit switch
            {
                ResultTypes.Money => Argument.AsMoney(resultValue.Value),
                ResultTypes.Percent => Argument.AsPercent(resultValue.Value),
                ResultTypes.Dsl => Argument.AsDsl(resultValue.DslCode),
                _ => throw new ArgumentException($"cannot convert ResultValue of type {resultValue.Unit.ToString()}")
            };
        }
    }
}
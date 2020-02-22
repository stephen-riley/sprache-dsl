namespace SpracheDsl.Types
{
    using System;
    using static ArgumentTypes;

    public class Argument
    {
        public ArgumentTypes Type { get; private set; }

        public string Id { get; private set; }

        public decimal Value { get; private set; }

        public string Dsl { get; private set; }

        public FunctionInvocation FuncInvocation { get; private set; }

        public static Argument AsVariable(string id)
        {
            return new Argument { Type = Variable, Id = id };
        }

        public static Argument AsDecimal(string num)
        {
            return new Argument { Type = Number, Value = Convert.ToDecimal(num) };
        }

        public static Argument AsSymbol(string id)
        {
            return new Argument { Type = Symbol, Id = id };
        }

        public static Argument AsPercent(string num)
        {
            return new Argument { Type = Percent, Value = Convert.ToDecimal(num) / 100m };
        }

        public static Argument AsPercent(decimal num)
        {
            return new Argument { Type = Percent, Value = num };
        }

        public static Argument AsMoney(string num)
        {
            return new Argument { Type = Money, Value = Convert.ToDecimal(num) };
        }

        public static Argument AsMoney(decimal num)
        {
            return new Argument { Type = Money, Value = num };
        }

        public static Argument AsIdentifier(string id)
        {
            return new Argument { Type = Identifier, Id = id };
        }

        public static Argument AsFunctionCall(FunctionInvocation invocation)
        {
            return new Argument { Type = FunctionCall, FuncInvocation = invocation };
        }

        public static Argument AsBoolean(int b)
        {
            return new Argument { Type = Bool, Value = b };
        }

        public static Argument AsBoolean(bool b)
        {
            return new Argument { Type = Bool, Value = b ? 1 : 0 };
        }

        public static Argument AsNumber(string n)
        {
            return new Argument { Type = Number, Value = Convert.ToDecimal(n) };
        }

        public static Argument AsNumber(decimal n)
        {
            return new Argument { Type = Number, Value = n };
        }

        public static Argument True()
        {
            return Argument.AsBoolean(true);
        }

        public static Argument False()
        {
            return Argument.AsBoolean(false);
        }

        public static Argument AsDsl(string dsl)
        {
            return new Argument { Type = ArgumentTypes.Dsl, Dsl = dsl };
        }
    }
}
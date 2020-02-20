namespace SpracheDsl.Types
{
    using System;
    using static ArgumentTypes;

    public class Argument
    {
        public ArgumentTypes Type { get; private set; }

        public string Id { get; private set; }

        public decimal Value { get; private set; }

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

        public static Argument AsMoney(string num)
        {
            return new Argument { Type = Money, Value = Convert.ToDecimal(num) };
        }

        public static Argument AsIdentifier(string id)
        {
            return new Argument { Type = Identifier, Id = id };
        }

    }
}
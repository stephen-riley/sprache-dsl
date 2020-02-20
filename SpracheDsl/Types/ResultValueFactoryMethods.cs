namespace SpracheDsl.Types
{
    public partial class ResultValue
    {
        public static ResultValue Money(decimal value)
        {
            return new ResultValue(ResultTypes.Money, value);
        }

        public static ResultValue Percent(decimal value)
        {
            return new ResultValue(ResultTypes.Percent, value);
        }

        public static ResultValue Dimensionless(decimal value)
        {
            return new ResultValue(ResultTypes.Dimensionless, value);
        }

        public static ResultValue Dsl(string dslCode)
        {
            return new ResultValue(ResultTypes.Dsl) { DslCode = dslCode };
        }
    }
}

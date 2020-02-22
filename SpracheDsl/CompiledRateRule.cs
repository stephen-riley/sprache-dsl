using SpracheDsl.Types;

namespace SpracheDsl
{
    public class CompiledRateRule : ICompiledRule
    {
        // rate( 6.5%, costbasis() )

        public ResultValue Execute(ExecContext context, params Argument[] args)
        {
            return ResultValue.Money(0.065m * Functions.CostBasis(context).Value);
        }
    }
}
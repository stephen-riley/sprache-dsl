using System;
using SpracheDsl.Types;

namespace SprachDsl
{
    public class Functions
    {
        public Line Line { get; private set; }

        public Functions(Line line)
        {
            Line = line;
        }

        public void RateFunction()
        {
            throw new NotImplementedException();
        }

        public Argument CostBasisFunction()
        {
            return Argument.AsMoney(Line.Quantity * Line.ItemPrice);
        }
    }
}
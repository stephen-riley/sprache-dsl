
namespace SpracheDsl.Types
{
    public class UnitOfMeasure
    {
        public decimal Units { get; set; }

        public MeasureAs MeasureAs { get; set; }

        public UnitOfMeasure()
        {
            Units = 1.0m;
            MeasureAs = MeasureAs.ea;
        }

        public UnitOfMeasure(decimal units, MeasureAs measureAs)
        {
            Units = units;
            MeasureAs = measureAs;
        }
    }
}

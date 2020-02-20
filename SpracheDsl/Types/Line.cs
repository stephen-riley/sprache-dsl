
using System;

namespace SpracheDsl.Types
{
    public class Line
    {
        public string ProductCode { get; set; }

        public decimal ItemPrice { get; set; }

        public uint Quantity { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }

        public Line()
        {
            UnitOfMeasure = new UnitOfMeasure(1.0m, MeasureAs.ea);
        }

        public static Line Clone(Line line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            var result = new Line()
            {
                ItemPrice = line.ItemPrice,
                Quantity = line.Quantity,
                ProductCode = line.ProductCode,
                UnitOfMeasure = line.UnitOfMeasure,
            };

            return result;
        }

        public Line Clone()
        {
            return Clone(this);
        }
    }
}

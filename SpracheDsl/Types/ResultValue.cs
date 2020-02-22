using System.Collections.Generic;
using System.Globalization;

namespace SpracheDsl.Types
{
    public partial class ResultValue
    {
        static Dictionary<int, ResultTypes> ProductTypes = new Dictionary<int, ResultTypes>
        {
            { (int)(ResultTypes.Money | ResultTypes.Dimensionless), ResultTypes.Money },
            { (int)(ResultTypes.Unit | ResultTypes.Dimensionless), ResultTypes.Unit },
            { (int)(ResultTypes.Money | ResultTypes.Percent), ResultTypes.Money },
            { (int)(ResultTypes.Unit | ResultTypes.Percent), ResultTypes.Unit },
            { (int)(ResultTypes.Money | ResultTypes.Unit), ResultTypes.Money },
        };

        public decimal Value { get; set; }

        public string DslCode { get; set; }

        public ResultTypes Unit { get; set; }

        public string Description { get; set; }

        public ResultValue()
        {
            Value = 0.0m;
            Unit = ResultTypes.Undefined;
        }

        public ResultValue(ResultTypes type) : this()
        {
            Unit = type;
        }

        public ResultValue(ResultTypes type, decimal value) : this(type)
        {
            Value = value;
        }

        public static ResultValue Undefined()
        {
            return new ResultValue() { Value = 0.0m, Unit = ResultTypes.Undefined };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1013:OverloadOperatorEqualsOnOverloadingAddAndSubtract")]
        public static ResultValue operator +(ResultValue left, ResultValue right)
        {
            if (left == null || right == null)
            {
                return Undefined();
            }

            if (left.Unit == ResultTypes.Undefined || right.Unit == ResultTypes.Undefined)
            {
                return Undefined();
            }

            if (left.Unit != right.Unit)
            {
                return Undefined();
            }

            return new ResultValue() { Value = left.Value + right.Value, Unit = left.Unit };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
        public static ResultValue operator *(ResultValue left, ResultValue right)
        {
            if (left == null || right == null)
            {
                return Undefined();
            }

            if (left.Unit == ResultTypes.Undefined || right.Unit == ResultTypes.Undefined)
            {
                return Undefined();
            }

            var result = new ResultValue() { Value = left.Value * right.Value };

            if (ProductTypes.ContainsKey((int)(left.Unit | right.Unit)))
            {
                result.Unit = ProductTypes[(int)(left.Unit | right.Unit)];
            }
            else
            {
                result.Unit = ResultTypes.Undefined;
            }

            return result;
        }

        public static bool operator ==(ResultValue lhs, ResultValue rhs)
        {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }

            if ((lhs.Unit == rhs.Unit) && (lhs.Value == rhs.Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(ResultValue lhs, ResultValue rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            var termValue = obj as ResultValue;

            if (termValue != null)
            {
                return this == termValue;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            switch (Unit)
            {
                case ResultTypes.Money:
                    return "$" + Value;
                case ResultTypes.Percent:
                    return (Value * 100m) + "%";
                case ResultTypes.Undefined:
                    return "(undefined term value)";
                default:
                    return Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}

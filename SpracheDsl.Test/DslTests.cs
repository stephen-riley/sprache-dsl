
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpracheDsl.Types;
using System.Collections.Generic;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslTests
    {
        static Line line = new Line()
        {
            ItemPrice = 100.00m,
            UnitOfMeasure = new UnitOfMeasure(30m, MeasureAs.mL),
            Quantity = 1,
        };

        [TestMethod]
        public void CanRunSimpleDsl01()
        {
            var interpreter = new DslEvaluator() { Line = line };
            var result = interpreter.Eval("rate( 6.5%, costbasis() )");
            Assert.AreEqual(ResultValue.Money(6.50m), result);
        }

        [TestMethod]
        public void CanRunSimpleDsl02()
        {
            var interpreter = new DslEvaluator() { Line = line };
            var result = interpreter.Eval("rate( defaultrate(), costbasis() )");
            Assert.AreEqual(ResultValue.Money(6.50m), result);
        }

        [TestMethod]
        public void CanRunComplexDsl01()
        {
            var interpreter = new DslEvaluator() { Line = line };
            var result = interpreter.Eval(@"
                exclusive( 
                    fee( $3, unitband(  $4, $15, unitbasis( :in-diag ) ) ), 
                    fee( $4, unitband( $15, $35, unitbasis( :in-diag ) ) ), 
                    fee( $5, unitband( $35, INF, unitbasis( :in-diag ) ) )
                )");

            Assert.AreEqual(ResultValue.Money(4.00m), result);
        }

        [TestMethod]
        public void CanRunDslWithParams()
        {
            var interpreter = new DslEvaluator
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue> { { "var1", new ResultValue(ResultTypes.Percent, 0.50m) } }
            };
            var result = interpreter.Eval(@"rate( @var1, costbasis() )");

            Assert.AreEqual(ResultValue.Money(50.00m), result);
        }

        [TestMethod]
        public void CanRunDslWithCppComments()
        {
            var interpreter = new DslEvaluator
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue> { { "var1", new ResultValue(ResultTypes.Percent, 0.50m) } }
            };
            var result = interpreter.Eval(@"
                rate( 
                    @var1,          // Look up from paramBag
                    costbasis()     // From the line item
                )
                ");

            Assert.AreEqual(ResultValue.Money(50.00m), result);
        }

        [TestMethod]
        public void CanRunMultilineDsl()
        {
            var interpreter = new DslEvaluator
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue> { { "var1", new ResultValue(ResultTypes.Percent, 0.50m) } }
            };
            var result = interpreter.Eval(@"
                rate( 
                    @var1,
                    costbasis()
                )
                ");

            Assert.AreEqual(ResultValue.Money(50.00m), result);
        }

        [TestMethod]
        public void CanRunDslWithCComments()
        {
            var interpreter = new DslEvaluator()
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue> { { "var1", new ResultValue(ResultTypes.Percent, 0.50m) } }
            };
            var result = interpreter.Eval(@"
                rate( 
                    @var1,          /* Look up from paramBag */
                    costbasis()     /* From the line item */
                )
                ");

            Assert.AreEqual(ResultValue.Money(50.00m), result);
        }

        [TestMethod]
        public void CanRunDslAndGetNotes()
        {
            var interpreter = new DslEvaluator()
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue>() { { "DEFAULT_RATE:G", ResultValue.Percent(0.065m) } },
            };

            var result = interpreter.Eval("rate( defaultrate( :G ), costbasis() )");
            Assert.AreEqual(ResultValue.Money(6.5m), result);

        }
    }
}

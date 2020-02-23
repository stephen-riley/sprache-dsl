using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpracheDsl.Types;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslFunctionTests
    {
        static Line line = new Line()
        {
            ItemPrice = 1.00m,
            Quantity = 1,
        };

        [TestMethod]
        public void CanRunDslBandedAmount()
        {
            var dsl = "bandedamount( $100, $200, costbasis() )";

            var interpreter = new DslEvaluator() { Line = line };
            interpreter.Line.ItemPrice = 150.00m;
            Assert.AreEqual(ResultValue.Money(150.00m), interpreter.Eval(dsl));

            interpreter.Line.ItemPrice = 50.00m;
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));

            interpreter.Line.ItemPrice = 250.00m;
            Assert.AreEqual(ResultValue.Money(200.00m), interpreter.Eval(dsl));
        }

        [TestMethod]
        public void CanRunDslAmountInBand()
        {
            var dsl = "amountinband( $100, $200, costbasis() )";

            var interpreter = new DslEvaluator() { Line = line };

            interpreter.Line.ItemPrice = 150.00m;
            Assert.AreEqual(ResultValue.Money(50.00m), interpreter.Eval(dsl));

            interpreter.Line.ItemPrice = 50.00m;
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));

            interpreter.Line.ItemPrice = 250.00m;
            Assert.AreEqual(ResultValue.Money(100.00m), interpreter.Eval(dsl));

            // fencepost
            interpreter.Line.ItemPrice = 0.00m;
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));

            // fencepost
            interpreter.Line.ItemPrice = 100.00m;
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));

            // fencepost
            interpreter.Line.ItemPrice = 200.00m;
            Assert.AreEqual(ResultValue.Money(100.00m), interpreter.Eval(dsl));
        }

        [TestMethod]
        public void CanRunDslJurisTypeMatch()
        {
            var dsl = "juristypematch( :STATE, $1.00 )";

            var interpreter = new DslEvaluator() { JurisdictionType = "STATE" };
            Assert.AreEqual(ResultValue.Money(1.00m), interpreter.Eval(dsl));

            interpreter.JurisdictionType = "COUNTY";
            Assert.AreEqual(ResultValue.Dimensionless(0.00m), interpreter.Eval(dsl));

            dsl = "juristypematch( [ :COUNTY, :CITY ], $1.00 )";
            interpreter.JurisdictionType = "COUNTY";
            Assert.AreEqual(ResultValue.Money(1.00m), interpreter.Eval(dsl));

            // fencepost
            interpreter.JurisdictionType = "STATE";
            Assert.AreEqual(ResultValue.Dimensionless(0.00m), interpreter.Eval(dsl));
        }

        [TestMethod]
        public void CanRunDslDiscount()
        {
            var interpreter = new DslEvaluator();

            var dsl = "discount( 20%, $1.00 )";
            Assert.AreEqual(ResultValue.Money(0.80m), interpreter.Eval(dsl));

            dsl = "discount( 150%, $1.00 )";
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));

            dsl = "discount( $0.20, $1.00 )";
            Assert.AreEqual(ResultValue.Money(0.80m), interpreter.Eval(dsl));

            dsl = "discount( $1.20, $1.00 )";
            Assert.AreEqual(ResultValue.Money(0.00m), interpreter.Eval(dsl));
        }

        [TestMethod]
        public void CanRunDslSetCostBasis()
        {
            var interpreter = new DslEvaluator() { Line = line };

            interpreter.Line.ItemPrice = 0.0m;
            interpreter.Eval("setcostbasis( $1.00 )");
            var result = interpreter.Eval("costbasis()");
            Assert.AreEqual(ResultValue.Money(1.00m), result);
        }
    }
}

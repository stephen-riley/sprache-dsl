
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpracheDsl;
using SpracheDsl.Types;
using System.Collections.Generic;
using System.Linq;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslTests
    {
        static Line line = new Line()
        {
            ItemPrice = 100.00m,
            UnitOfMeasure = new UnitOfMeasure(30m, MeasureAs.inDiag),
            Quantity = 1,
        };

        [TestMethod]
        public void CanRunSimpleDsl01()
        {
            var interpreter = new DslEvaluator() { Line = line };
            var result = interpreter.Eval("rate( 6.5%, costbasis() )");
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(6.50m), result.First());
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

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(4.00m), result.First());
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

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(50.00m), result.First());
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

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(50.00m), result.First());
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

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(50.00m), result.First());
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

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(ResultValue.Money(50.00m), result.First());
        }

        // [TestMethod]
        // public void CanRunDslAndGetNotes()
        // {
        //     var interpreter = new DslEvaluator()
        //     {
        //         Line = line,
        //         ParamBag = new Dictionary<string, ResultValue>() { { "DEFAULT_RATE:G", ResultValue.Percent(0.065m) } },
        //     };

        //     var result = interpreter.Eval("rate( defaultrate( :G ), costbasis() )");
        //     Assert.AreEqual(1, result.Count());
        //     Assert.AreEqual(ResultValue.Money(6.5m), result.First());

        //     var notes = result.First().Notes;
        //     Assert.AreEqual(3, notes.Count);
        //     Assert.AreEqual("defaultrate", notes[0].Substring(0, 11));
        //     Assert.AreEqual("costbasis", notes[1].Substring(0, 9));
        //     Assert.AreEqual("rate", notes[2].Substring(0, 4));
        // }

        [TestMethod]
        public void CanEvaluateBooleans()
        {
            var interpreter = new DslEvaluator()
            {
                Line = line,
            };

            Assert.AreEqual(1.0m, interpreter.Eval("rate( true, $1 )").First().Value);
            Assert.AreEqual(0.0m, interpreter.Eval("rate( false, $1 )").First().Value);
        }

        [TestMethod]
        public void CanCalculateWashingtonLiquorRules()
        {
            var interpreter = new DslEvaluator()
            {
                Line = new Line()
                {
                    ItemPrice = 100.00m,
                    UnitOfMeasure = new UnitOfMeasure(750m, MeasureAs.mL),
                    Quantity = 2,
                },
                ParamBag = new Dictionary<string, ResultValue>()
                {
                    { "washington.liquor.salestax", ResultValue.Percent(0.205m) },
                    { "washington.liquor.salesfee", ResultValue.Money(0.0037708m) }
                },
            };

            var rule = @"sumtax(
                             rate( @washington.liquor.salestax, costbasis() ),
                             rate( @washington.liquor.salesfee, unitbasis( :mL ) )
                         )";

            var results = interpreter.Eval(rule);
            Assert.AreEqual(1, results.Count());

            var result = results.First();

            // test the overall result
            Assert.AreEqual(46.6562m, result.Value);
            Assert.AreEqual(ResultTypes.Money, result.Unit);

            // test the components (this only works for this rule)
            // Assert.AreEqual(41.0m, result.LeftValue.Value);
            // Assert.AreEqual(5.6562m, result.RightValue.Value);

            // Assert.AreEqual(7, result.Notes.Count);
            // Assert.AreEqual("sumtax(): $41.00000 $5.6562000 = $46.6562000", result.Notes[6]);
        }
    }
}

using SpracheDsl.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SpracheDsl;
using System.Linq;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslBracketFunctionTests
    {
        static string MarylandBracketDsl = @"
            exclusive(
                bandedfee(
                    $0.00, $1.00,
                    costbasis(),
                    exclusive(
                        bandedfee( $0.00, $0.20, cents( costbasis() ), $0.00 ),
                        bandedfee( $0.20, $0.21, cents( costbasis() ), $0.01 ),
                        bandedfee( $0.21, $0.34, cents( costbasis() ), $0.02 ),
                        bandedfee( $0.34, $0.51, cents( costbasis() ), $0.03 ),
                        bandedfee( $0.51, $0.67, cents( costbasis() ), $0.04 ),
                        bandedfee( $0.67, $0.84, cents( costbasis() ), $0.05 ),
                        bandedfee( $0.84, $1.00, cents( costbasis() ), $0.06 )
                    )
                ),
                bandedfee(
                    $1.00, INF,
                    costbasis(),
                    sumtax(
                        rate( 6%, dollars( costbasis() ) ),
                        exclusive(
                            bandedfee( $0.00, $0.01, cents( costbasis() ), $0.00 ),
                            bandedfee( $0.01, $0.17, cents( costbasis() ), $0.01 ),
                            bandedfee( $0.17, $0.34, cents( costbasis() ), $0.02 ),
                            bandedfee( $0.34, $0.51, cents( costbasis() ), $0.03 ),
                            bandedfee( $0.51, $0.67, cents( costbasis() ), $0.04 ),
                            bandedfee( $0.67, $0.84, cents( costbasis() ), $0.05 ),
                            bandedfee( $0.84, $1.00, cents( costbasis() ), $0.06 )
                        )
                    )
                )
            )";

        static string FloridaBracketDsl = @"
            exclusive(
                bandedfee(
                    $0.01, $0.09,
                    cents( costbasis() ),
                    sumtax(
                        rate( 6%, dollars( costbasis() ) ),
                        $0.01
                    )
                ),
                roundmoney(
                    sumtax(
                        rate( 6%, costbasis() ),
                        $0.0048
                    )
                )
            )";

        [TestMethod]
        public void CanCalculateMarylandBracketTax()
        {
            var line = new Line() { ItemPrice = 2.50m, Quantity = 1 };
            var evaluator = new DslEvaluator() { Line = line };
            var result = evaluator.Eval(MarylandBracketDsl);

            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.12m + 0.03m, result.First().Value);

            evaluator.Line.ItemPrice = 100.00m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(6.00m, result.First().Value);

            evaluator.Line.ItemPrice = 0.50m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.03m, result.First().Value);

            evaluator.Line.ItemPrice = 0.15m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.00m, result.First().Value);

            evaluator.Line.ItemPrice = 1.15m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.06m + 0.01m, result.First().Value);

            evaluator.Line.ItemPrice = 0.20m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.01m, result.First().Value);

            evaluator.Line.ItemPrice = 0.00m;
            result = evaluator.Eval(MarylandBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.00m, result.First().Value);
        }

        [TestMethod]
        public void CanCalculateMarylandBracketTaxAsDefaultRate()
        {
            var line = new Line { ItemPrice = 1.15m, Quantity = 1 };
            var evaluator = new DslEvaluator()
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue>()
                {
                    { "DEFAULT_RATE:G", ResultValue.Dsl(MarylandBracketDsl) }
                }
            };

            var result = evaluator.Eval("rate( defaultrate( :G ), costbasis() )");
            Assert.AreEqual(0.06m + 0.01m, result.First().Value);
        }

        [TestMethod]
        public void CanCalculateFloridaBracketTax()
        {
            var line = new Line() { ItemPrice = 2.50m, Quantity = 1 };
            var evaluator = new DslEvaluator() { Line = line };
            var result = evaluator.Eval(FloridaBracketDsl);

            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.15m, result.First().Value);

            evaluator.Line.ItemPrice = 100.00m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(6.00m, result.First().Value);

            evaluator.Line.ItemPrice = 0.50m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.03m, result.First().Value);

            evaluator.Line.ItemPrice = 0.15m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.01m, result.First().Value);

            evaluator.Line.ItemPrice = 1.15m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.07m, result.First().Value);

            evaluator.Line.ItemPrice = 0.20m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.02m, result.First().Value);

            evaluator.Line.ItemPrice = 0.00m;
            result = evaluator.Eval(FloridaBracketDsl);
            Assert.AreEqual(ResultTypes.Money, result.First().Unit);
            Assert.AreEqual(0.00m, result.First().Value);
        }

        [TestMethod]
        public void CanCalculateFloridaBracketTaxAsDefaultRate()
        {
            var line = new Line { ItemPrice = 1.15m, Quantity = 1 };
            var evaluator = new DslEvaluator()
            {
                Line = line,
                ParamBag = new Dictionary<string, ResultValue>()
                {
                    { "DEFAULT_RATE:G", ResultValue.Dsl(FloridaBracketDsl) }
                }
            };

            var result = evaluator.Eval("rate( defaultrate( :G ), costbasis() )");
            Assert.AreEqual(0.07m, result.First().Value);
        }
    }
}

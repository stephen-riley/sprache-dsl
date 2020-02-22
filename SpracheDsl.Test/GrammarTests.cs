using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl.Test
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void TestExpressionMONEY()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse("$12.34");
            Assert.AreEqual(12.34m, result.Value);
            Assert.AreEqual(ArgumentTypes.Money, result.Type);
        }

        [TestMethod]
        public void TestExpressionPERCENT()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse("6.5%");
            Assert.AreEqual(0.065m, result.Value);
            Assert.AreEqual(ArgumentTypes.Percent, result.Type);
        }

        [TestMethod]
        public void TestExpressionSYMBOL()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse(":Symbol-With-Stuff-In_It");
            Assert.AreEqual("Symbol-With-Stuff-In_It", result.Id);
            Assert.AreEqual(ArgumentTypes.Symbol, result.Type);
        }

        [TestMethod]
        public void TestVoidFunctionInvocation()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse("rate()");
            Assert.AreEqual(ArgumentTypes.FunctionCall, result.Type);
            Assert.AreEqual("rate", result.FuncInvocation.Name);
            Assert.AreEqual(0, result.FuncInvocation.Args.Count);
        }

        [TestMethod]
        public void TestNormalFunctionInvocation()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse("rate(6.5%, :costbasis)");
            Assert.AreEqual(ArgumentTypes.FunctionCall, result.Type);
            Assert.AreEqual("rate", result.FuncInvocation.Name);
            Assert.AreEqual(2, result.FuncInvocation.Args.Count);
        }

        [TestMethod]
        public void TestRecursiveFunctionInvocation()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.Expression.Parse("rate(6.5%, costbasis())");
            Assert.AreEqual(ArgumentTypes.FunctionCall, result.Type);
            Assert.AreEqual("rate", result.FuncInvocation.Name);
            Assert.AreEqual(2, result.FuncInvocation.Args.Count);
        }

        [TestMethod]
        public void CanParseSimpleAttribute()
        {
            var interpreter = new DslEvaluator();
            var result = DslGrammar.AttrList.Parse("[RunBefore=:COUNTY]");
            Assert.AreEqual(1, result.ToList().Count);
            Assert.AreEqual("RunBefore", result.First().Name);
            Assert.AreEqual(1, result.First().Values.Count);
            Assert.AreEqual(":COUNTY", result.First().Values[0]);
        }

        [TestMethod]
        public void CanParseMultipleSimpleAttributes()
        {
            var text = @"
                [RunBefore=:COUNTY]
                [RunBefore=:CITY]
                [RunBefore=:STJ]";

            var interpreter = new DslEvaluator();
            var result = DslGrammar.AttrList.Parse(text);
            Assert.AreEqual(3, result.ToList().Count);
            Assert.AreEqual("RunBefore", result.First().Name);
            Assert.AreEqual(1, result.First().Values.Count);
            Assert.AreEqual(":COUNTY", result.First().Values[0]);
        }

        [TestMethod]
        public void CanParseSimpleAttributeWithSurroundingText()
        {
            var text = @"
                // some comments
                [RunBefore=:COUNTY]
                rate( 6.5%, costbasis() )";

            var interpreter = new DslEvaluator();
            var result = DslGrammar.AttrList.Parse(text);
            Assert.AreEqual(1, result.ToList().Count);
            Assert.AreEqual("RunBefore", result.First().Name);
            Assert.AreEqual(1, result.First().Values.Count);
            Assert.AreEqual(":COUNTY", result.First().Values[0]);
        }

        [TestMethod]
        public void CanParseMultipleAttributesWithSurroundingText()
        {
            var text = @"
                // some comments
                [RunBefore=:COUNTY]
                [RunBefore=:CITY]
                [RunBefore=:STJ]
                rate( 6.5%, costbasis() )";

            var interpreter = new DslEvaluator();
            var result = DslGrammar.AttrList.Parse(text);
            Assert.AreEqual(3, result.ToList().Count);
            Assert.AreEqual("RunBefore", result.First().Name);
            Assert.AreEqual(1, result.First().Values.Count);
            Assert.AreEqual(":COUNTY", result.First().Values[0]);
        }
    }
}
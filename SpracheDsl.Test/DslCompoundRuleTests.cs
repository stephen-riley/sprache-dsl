using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpracheDsl;
using System.Linq;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslCompoundRuleTests
    {
        string singleDsl = @"
                [Description='part1']
                rate(1%, $100)";

        string compoundDsl = @"
                [Description='part1']
                rate(1%, $100)

                [Description='part2']
                rate(2%, $100)";

        [TestMethod]
        public void CanEvaluateCompoundRule()
        {
            var interpreter = new DslEvaluator();
            var results = interpreter.Eval(compoundDsl);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(1m, results.ElementAt(0).Value);
            Assert.AreEqual(2m, results.ElementAt(1).Value);
        }

        [TestMethod]
        public void CanGetRuleDescriptions()
        {
            var interpreter = new DslEvaluator();
            var results = interpreter.Attributes(compoundDsl);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("part1", results.ElementAt(0).Where(a => a.Name == "Description").First().Values.First());
            Assert.AreEqual("part2", results.ElementAt(1).Where(a => a.Name == "Description").First().Values.First());
        }

        [TestMethod]
        public void CanEvaluateNonCompoundRule()
        {
            var interpreter = new DslEvaluator();
            var results = interpreter.Eval(singleDsl);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(1m, results.ElementAt(0).Value);
        }
    }
}

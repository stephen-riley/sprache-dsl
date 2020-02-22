using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpracheDsl.Test
{
    [TestClass]
    public class DslAttrTests
    {
        [TestMethod]
        public void CanGetDslAttributes()
        {
            var dsl = @"
                // Might this be a way to deal with Eager AZ?
                [RunBefore=:COUNTY]
                [RunBefore=:CITY]
                [RunBefore=:STJ]
                setcostbasis( discount( 35%, costbasis() ) )
                ";

            var interpreter = new DslEvaluator();
            var result = interpreter.Attributes(dsl);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("RunBefore", result[0].Name);
            Assert.AreEqual(3, result[0].Values.Count);
            Assert.IsTrue(result[0].Values.Contains(":COUNTY"));
            Assert.IsTrue(result[0].Values.Contains(":CITY"));
            Assert.IsTrue(result[0].Values.Contains(":STJ"));
        }

        [TestMethod]
        public void CanHandleNoDslAttributes()
        {
            var dsl = @"
                // No attributes here!
                setcostbasis( discount( 35%, costbasis() ) )
                ";

            var interpreter = new DslEvaluator();
            var result = interpreter.Attributes(dsl);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SpracheDsl.Types;

namespace SpracheDsl.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp30)]
    public class BenchmarkWarmedUp
    {
        static Line line = new Line()
        {
            ItemPrice = 100.00m,
            UnitOfMeasure = new UnitOfMeasure(30m, MeasureAs.mL),
            Quantity = 1,
        };

        [GlobalSetup]
        [Benchmark]
        public void ExecuteCompiledRule()
        {
            var interpreter = new DslEvaluator() { Line = Program.Line };
            var result = interpreter.Eval(Program.Rule, forceReparse: true);
        }
    }
}
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SpracheDsl.Types;

namespace SpracheDsl.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp30)]
    [RPlotExporter, RankColumn]
    public class BenchmarkCompiles
    {
        [Benchmark]
        public void CompileBasicRule()
        {
            var interpreter = new DslEvaluator() { Line = Program.Line };
            var result = interpreter.Eval(Program.Rule, forceReparse: true);
        }
    }
}
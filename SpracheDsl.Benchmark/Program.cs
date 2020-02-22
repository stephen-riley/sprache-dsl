using System;
using BenchmarkDotNet.Running;
using SpracheDsl.Types;

namespace SpracheDsl.Benchmark
{
    public class Program
    {
        // public static string Rule = @"
        //         exclusive( 
        //             fee( $3, unitband(  $4, $15, unitbasis( :mL ) ) ), 
        //             fee( $4, unitband( $15, $35, unitbasis( :mL ) ) ), 
        //             fee( $5, unitband( $35, INF, unitbasis( :mL ) ) )
        //         )";

        public static string Rule = "rate( 6.5%, costbasis() )";

        public static Line Line = new Line
        {
            ItemPrice = 100.00m,
            UnitOfMeasure = new UnitOfMeasure(30m, MeasureAs.mL),
            Quantity = 1,
        };

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkCompiles>();
            var summary2 = BenchmarkRunner.Run<BenchmarkWarmedUp>();
        }
    }
}

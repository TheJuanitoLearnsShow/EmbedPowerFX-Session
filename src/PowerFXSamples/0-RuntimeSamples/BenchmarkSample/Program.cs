using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Security.Cryptography;

namespace BenchmarkSample
{
    public class PowerFxBecnhmark
    {
        [Benchmark]
        public decimal RunPowerFXFormulas()
        {
            var result = SamplePowerFx.GetTestValue();
            return result;
        }

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PowerFxBecnhmark>();
        }
    }
}
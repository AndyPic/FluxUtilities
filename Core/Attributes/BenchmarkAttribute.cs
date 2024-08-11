using System;

namespace Flux.Core.Attributes
{
    /// <summary>
    /// The decorated method will be executed as part of the benchmark group specified by the 
    /// <see cref="GroupName"/> (decorating multiple methods with the same <see cref="GroupName"/>
    /// will run them as part of the same benchmark).
    /// <br></br>
    /// <see cref="Itterations"/> can be set to manually specify the number of itterations to run
    /// the benchmark methods for.
    /// <br></br>
    /// The default itterations are: 1_000.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BenchmarkAttribute : Attribute
    {
        public string GroupName { get; set; } = "Default";
        public int[] Itterations { get; set; } = new int[] { 1_000 };
    }
}
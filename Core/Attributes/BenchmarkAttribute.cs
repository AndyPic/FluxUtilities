using System;
using System.Drawing;

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
    /// <br></br>
    /// See also: <see cref="BenchmarkSetupAttribute"/> and <see cref="BenchmarkCleanupAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BenchmarkAttribute : A_BenchmarkAttribute
    {
        public int[] Itterations { get; set; } = new int[] { 1_000 };
        public Color Color { get; set; } = Color.White;

        public BenchmarkAttribute(string groupName = "Default", params int[] itterations)
        {
            GroupName = string.IsNullOrEmpty(groupName) ? "Default" : groupName;
            Itterations = itterations != null && itterations.Length > 0 ? itterations : new int[] { 1_000 };
        }

        // todo - Add a 'setup' function that can be run before the benchmark
        // to setup any data for the benchmarking
        // some enum options eg. run before each itteration, run once at start
        // run before each method etc.
        // and GroupName to specify which group it should run on
    }
}
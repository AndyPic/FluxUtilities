using System;

namespace Flux.Core.Attributes
{
    /// <summary>
    /// The decorated method will be run as cleanup for Benchmarks with the same 
    /// <see cref="GroupName"/>. Set the <see cref="RunType"/> to define when cleanup is run.
    /// <br></br>
    /// See also: <see cref="BenchmarkAttribute"/> and <see cref="BenchmarkSetupAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BenchmarkCleanupAttribute : A_BenchmarkAttribute
    {
        public enum E_RunType
        {
            /// <summary>
            /// Runs cleanup once at the end of the benchmark group.
            /// </summary>
            OnceAtEnd,
            /// <summary>
            /// Runs cleanup after each itteration.
            /// </summary>
            AfterEachItteration,
            /// <summary>
            /// Runs cleanup after each unique method in this BenchmarkGroup.
            /// </summary>
            AfterEachUniqueMethod
        }
        public E_RunType RunType { get; set; } = E_RunType.OnceAtEnd;
    }
}
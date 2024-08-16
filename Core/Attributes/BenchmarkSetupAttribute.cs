using System;

namespace Flux.Core.Attributes
{
    /// <summary>
    /// The decorated method will be run as setup for Benchmarks with the same 
    /// <see cref="GroupName"/>. Set the <see cref="RunType"/> to define when setup is run.
    /// <br></br>
    /// See also: <see cref="BenchmarkAttribute"/> and <see cref="BenchmarkCleanupAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BenchmarkSetupAttribute : A_BenchmarkAttribute
    {
        public enum E_RunType
        {
            /// <summary>
            /// Runs setup once at the start of the benchmark group.
            /// </summary>
            OnceAtStart,
            /// <summary>
            /// Runs setup before each itteration.
            /// </summary>
            BeforeEachItteration,
            /// <summary>
            /// Runs setup before each unique method in this BenchmarkGroup.
            /// </summary>
            BeforeEachUniqueMethod
        }
        public E_RunType RunType { get; set; } = E_RunType.OnceAtStart;
    }
}
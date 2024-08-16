using System;

namespace Flux.Core.Attributes
{
    public abstract class A_BenchmarkAttribute : Attribute
    {
        public string GroupName { get; set; } = "Default";
    }
}
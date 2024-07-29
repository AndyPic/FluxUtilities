using System;

namespace Flux.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string label;

        public ButtonAttribute(string label = "")
        {
            this.label = label;
        }
    }
}

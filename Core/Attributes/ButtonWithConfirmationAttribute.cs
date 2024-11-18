using System;

namespace Flux.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonWithConfirmationAttribute : Attribute
    {
        public string message;
        public string label;

        public ButtonWithConfirmationAttribute(string message = "Please confirm the action", string label = "")
        {
            this.message = message;
            this.label = label;
        }
    }
}

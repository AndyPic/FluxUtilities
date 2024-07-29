using System;
using UnityEngine;

namespace Flux.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public Type RequiredType { get; private set; }

        public RequireInterfaceAttribute(Type requiredType)
        {
            if (!requiredType.IsInterface)
            {
                throw new ArgumentException("RequiredType must be an interface type.");
            }
            RequiredType = requiredType;
        }
    }
}
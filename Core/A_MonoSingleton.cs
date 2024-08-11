using UnityEngine;

namespace Flux.Core
{
    [DisallowMultipleComponent]
    public abstract class A_MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary> Singleton instance. </summary>
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Duplicate instance of {GetType()} found! Replacing {Instance} with {this}.");
                DestroyImmediate(Instance);
            }

            Instance = this as T;
        }
    }
}
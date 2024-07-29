using UnityEngine;

namespace Flux.State
{
    public abstract class A_StateOperationData : ScriptableObject
    {
        /// <returns> A new instance of the <see cref="A_StateOperation"/>. </returns>
        public abstract A_StateOperation CreateInstance();
    }

    public abstract class A_StateOperation
    {
        /// <summary>
        /// Starts this operation.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Called once per frame while this operation is in progress.
        /// </summary>
        public abstract void Update();

        /// <returns> The current progress (0-1) of this operation. </returns>
        public abstract float GetProgress();
    }
}
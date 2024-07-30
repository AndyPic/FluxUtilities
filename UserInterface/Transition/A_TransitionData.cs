using UnityEngine;

namespace Flux.UserInterface
{
    public abstract class A_TransitionData : ScriptableObject
    {
        [field: Tooltip("Describes the trajectory of the transition.")]
        [field: SerializeField] public AnimationCurve TransitionCurve { get; set; }

        public abstract A_Transition GetInstance(Panel panel);
    }
}
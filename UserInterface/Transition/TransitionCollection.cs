using UnityEngine;

namespace Flux.UserInterface
{
    [CreateAssetMenu(fileName = "new TransitionCollection", menuName = "Flux/Interface/Transitions/Transition Collection")]
    public class TransitionCollection : ScriptableObject
    {
        [field: SerializeField] public A_TransitionData[] Transitions { get; private set; }
    }
}
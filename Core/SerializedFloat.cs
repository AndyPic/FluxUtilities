using UnityEngine;

namespace Flux.Core
{
    [CreateAssetMenu(fileName = "SerializedFloat", menuName = "Flux/Serialization/Float")]
    public class SerializedFloat : ScriptableObject
    {
        [field: SerializeField] public float Value { get; private set; }
    }
}
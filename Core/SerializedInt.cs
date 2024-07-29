using UnityEngine;

namespace Flux.Core
{
    [CreateAssetMenu(fileName = "SerializedInt", menuName = "Flux/Serialization/Int")]
    public class SerializedInt : ScriptableObject
    {
        [field: SerializeField] public int Value { get; private set; }
    }
}
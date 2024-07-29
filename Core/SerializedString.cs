using UnityEngine;

namespace Flux.Core
{
    [CreateAssetMenu(fileName = "SerializedString", menuName = "Flux/Serialization/String")]
    public class SerializedString : ScriptableObject
    {
        [field: SerializeField] public string Value { get; private set; }
    }
}
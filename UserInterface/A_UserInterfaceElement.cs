using UnityEngine;

namespace Flux.UserInterface
{
    /// <summary>
    /// Base type for all user interface elements.
    /// </summary>
    public abstract class A_UserInterfaceElement : MonoBehaviour
    {
        [field: Header("User Interface Element")]
        [field: Tooltip("Should this UI element disable on awake?")]
        [field: SerializeField] public bool StartDisabled { get; set; }

        protected virtual void Awake()
        {
            if (StartDisabled)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
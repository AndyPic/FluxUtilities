using UnityEngine;

namespace Flux.UserInterface
{
    [RequireComponent(typeof(Panel))]
    public class A_PanelComponent : MonoBehaviour
    {
        [SerializeField] protected Panel panel;

        protected virtual void Awake()
        {
            panel ??= GetComponent<Panel>();
        }
    }
}
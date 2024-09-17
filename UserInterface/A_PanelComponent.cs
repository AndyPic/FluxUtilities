using UnityEngine;

namespace Flux.UserInterface
{
    public class A_PanelComponent : MonoBehaviour
    {
        [SerializeField] protected Panel panel;

        protected virtual void Awake()
        {
            panel ??= GetComponent<Panel>();
        }
    }
}
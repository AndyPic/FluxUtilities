using UnityEngine;
using UnityEngine.UI;

namespace Flux.UserInterface
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class A_PanelGraphicComponent : MaskableGraphic
    {
        [SerializeField] protected Panel panel;

        protected override void Awake()
        {
            base.Awake();
            panel ??= GetComponent<Panel>();
        }
    }
}
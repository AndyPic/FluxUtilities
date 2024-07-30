using UnityEngine;

namespace Flux.UserInterface
{
    public class TransitionScale : A_Transition
    {
        protected override A_TransitionData BaseData => data;
        private readonly TransitionScaleData data;

        public TransitionScale(Panel panel, TransitionScaleData data) : base(panel)
        {
            this.data = data;
        }

        public override void SetUpTransitionToOff()
        {
            panel.RectTransform.localScale = Vector3.one;
        }

        public override void SetUpTransitionToOn()
        {
            panel.RectTransform.localScale = Vector3.zero;
        }

        public override void Update(float elapsedTime)
        {
            float value = data.TransitionCurve.Evaluate(elapsedTime);
            panel.RectTransform.localScale = new(value, value, value);
        }
    }
}
namespace Flux.UserInterface
{
    public class TransitionFade : A_Transition
    {
        protected override A_TransitionData BaseData => data;
        private readonly TransitionFadeData data;

        public TransitionFade(Panel panel, TransitionFadeData data) : base(panel)
        {
            this.data = data;
        }

        public override void SetUpTransitionToOff()
        {
            panel.CanvasGroup.alpha = 1;
        }

        public override void SetUpTransitionToOn()
        {
            panel.CanvasGroup.alpha = 0;
        }

        public override void Update(float elapsedTime)
        {
            float value = data.TransitionCurve.Evaluate(elapsedTime);
            panel.CanvasGroup.alpha = value;
        }
    }
}
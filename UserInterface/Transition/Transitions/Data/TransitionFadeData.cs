using UnityEngine;

namespace Flux.UserInterface
{
    [CreateAssetMenu(fileName = "new TransitionFadeData", menuName = "Flux/Interface/Transitions/Fade")]
    public class TransitionFadeData : A_TransitionData
    {
        public override A_Transition GetInstance(Panel panel)
        {
            return new TransitionFade(panel, this);
        }
    }
}
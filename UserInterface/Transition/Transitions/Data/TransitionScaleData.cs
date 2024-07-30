using UnityEngine;

namespace Flux.UserInterface
{
    [CreateAssetMenu(fileName = "new TransitionScaleData", menuName = "Flux/Interface/Transitions/Scale")]
    public class TransitionScaleData : A_TransitionData
    {
        public override A_Transition GetInstance(Panel panel)
        {
            return new TransitionScale(panel, this);
        }
    }
}
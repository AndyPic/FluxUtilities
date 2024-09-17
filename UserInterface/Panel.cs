using UnityEngine;

namespace Flux.UserInterface
{
    [RequireComponent(typeof(CanvasGroup))]
    public partial class Panel : A_UserInterfaceElement
    {
        [field: Header("Panel")]
        [field: Tooltip("The canvas group of this panel. Note: If left blank will get on awake.")]
        [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }

        [field: SerializeField] public RectTransform RectTransform { get; private set; }

        [field: Tooltip("The collection of transitions this panel will use when enabling / disabling.")]
        [field: SerializeField] public TransitionCollection Transitions { get; private set; }



        private TransitionHandler transitionHandler;

        protected override void Awake()
        {
            base.Awake();
            transitionHandler = new(Transitions);
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            transitionHandler.Update(deltaTime);
        }

        public void Toggle()
        {
            switch (transitionHandler.TransitionState)
            {
                case E_TransitionState.TransitioningOn:
                    Disable();
                    break;

                case E_TransitionState.TransitioningOff:
                    Enable();
                    break;

                default: // ie. E_TransitionState.None
                    if (gameObject.activeSelf)
                        Disable();
                    else
                        Enable();
                    break;
            }
        }

        public void Enable()
        {
            transitionHandler.TransitionToOn(this);
        }

        public void Disable()
        {
            transitionHandler.TransitionToOff(this);
        }
    }
}
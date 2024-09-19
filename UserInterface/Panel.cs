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

        [field: Tooltip("A UNIVERSALLY UNIQUE IDENTIFIER for this panel.")]
        [field: SerializeField] public uint UUID { get; private set; } = uint.MinValue;

        [field: SerializeField] public Canvas Canvas { get; private set; }

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

        private Vector2 GetScaledResolution()
        {
            return new Vector2(Screen.width / Canvas.scaleFactor, Screen.height / Canvas.scaleFactor);
        }

        public void SetPositionWithinScreen(Vector2 position, float screenPadding = 0)
        {
            var halfScaledResolution = GetScaledResolution() / 2;
            var target = RectTransform;

            // Update target position
            target.anchoredPosition = GetPositionInRect(target, position, halfScaledResolution, screenPadding);
        }

        private Vector2 GetPositionInRect(RectTransform target, Vector2 position, Vector2 halfSize, float padding = 0)
        {
#if UNITY_EDITOR
            // Validate the Anchor and Pivot
            if (target.anchorMin.x != 0.5f || target.anchorMax.x != 0.5f || target.anchorMin.y != 0.5f || target.anchorMax.y != 0.5f)
            {
                Debug.LogError("Invalid Anchor or Pivot (Must be 0.5)");
            }
#endif

            // Calculate min / max x and y
            var maxX = halfSize.x - (target.rect.width * 0.5f) - padding;
            var maxY = halfSize.y - (target.rect.height * 0.5f) - padding;

            // Lock position to screen
            position.x = Mathf.Clamp(position.x, -maxX, maxX);
            position.y = Mathf.Clamp(position.y, -maxY, maxY);

            return position;
        }
    }
}
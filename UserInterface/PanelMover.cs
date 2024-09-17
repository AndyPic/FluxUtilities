using Flux.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Flux.UserInterface
{
    public class PanelMover : A_PanelComponent, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [field: Tooltip("A UNIVERSALLY UNIQUE IDENTIFIER for this element. (Used to dientify the panel when saving the position between sessions)")]
        [field: SerializeField] public uint UUID { get; private set; } = uint.MinValue;

        [SerializeField] private Canvas canvas;

        [Tooltip("Should the Graphics Raycaster on the canvas be disabled while moving? Note: This prevents unwanted mouse-over events from the invisible cursor while draging.)")]
        [SerializeField] private bool disableCanvasRaycasterWhileMoving = true;

        private CursorLockMode startLockMode;
        private Vector2 cursorStartPosition;
        private Vector2 targetStartPosition;

        private bool isMoving = false;

        private string GetKey()
        {
            return $"{GetType()}:{UUID}";
        }

        protected override void Awake()
        {
            base.Awake();

            // Try load position
            if (PreferenceSaver.Load(GetKey(), out Vector2 pos))
            {
                SetPositionWithinScreen(pos);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Cache the starting state
            startLockMode = Cursor.lockState;
            targetStartPosition = panel.RectTransform.anchoredPosition;
            cursorStartPosition = eventData.position;

            // Confine & make invisible while dragging
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            // Set as last sibling (So it is ontop)
            panel.RectTransform.SetAsLastSibling();

            // Disable graphics raycast (if need to)
            if (disableCanvasRaycasterWhileMoving)
            {
                if (canvas.TryGetComponent(out GraphicRaycaster rc))
                {
                    rc.enabled = false;
                }
            }

            isMoving = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate and set the new position
            var newPosition = panel.RectTransform.anchoredPosition + (eventData.delta / canvas.scaleFactor);
            SetPositionWithinScreen(newPosition);
        }

        private Vector2 GetScaledResolution()
        {
            return new Vector2(Screen.width / canvas.scaleFactor, Screen.height / canvas.scaleFactor);
        }

        private void SetPositionWithinScreen(Vector2 position, float screenPadding = 0)
        {
            var halfScaledResolution = GetScaledResolution() / 2;
            var target = panel.RectTransform;

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

        public void OnEndDrag(PointerEventData eventData)
        {
            // Snap to nearest full pixel
            panel.RectTransform.anchoredPosition = new Vector2(Mathf.Round(panel.RectTransform.anchoredPosition.x), Mathf.Round(panel.RectTransform.anchoredPosition.y));

            // Reset cursor to cached state
            Cursor.lockState = startLockMode;
            Cursor.visible = true;

            // Move back to start position
            var delta = panel.RectTransform.anchoredPosition - targetStartPosition;
            delta *= canvas.scaleFactor;
            var newCursorPosition = cursorStartPosition + delta;

            Mouse.current.WarpCursorPosition(newCursorPosition);

            // Re-enable graphics raycaster (if there is one)
            if (canvas.TryGetComponent(out GraphicRaycaster rc))
            {
                rc.enabled = true;
            }

            // Save new position
            PreferenceSaver.Save(GetKey(), panel.RectTransform.anchoredPosition);

            isMoving = false;
        }

        private void OnDisable()
        {
            // If the game object gets disabled mid-drag, manually end the drag.
            if (isMoving)
            {
                OnEndDrag(null);
            }
        }
    }
}
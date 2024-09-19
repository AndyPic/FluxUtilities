using Flux.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Flux.UserInterface
{
    public class PanelMover : A_PanelComponent, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [Tooltip("Should the Graphics Raycaster on the canvas be disabled while moving? Note: This prevents unwanted mouse-over events from the invisible cursor while draging.)")]
        [SerializeField] private bool disableCanvasRaycasterWhileMoving = true;

        private CursorLockMode startLockMode;
        private Vector2 cursorStartPosition;
        private Vector2 targetStartPosition;

        private bool isMoving = false;

        protected override void Awake()
        {
            base.Awake();

            // Try load position
            if (PreferenceSaver.Load($"{panel.UUID}:position", out Vector2 pos))
            {
                panel.SetPositionWithinScreen(pos);
            }
        }

        private void OnDisable()
        {
            // If the game object gets disabled mid-drag, manually end the drag.
            if (isMoving)
            {
                OnEndDrag(null);
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
                if (panel.Canvas.TryGetComponent(out GraphicRaycaster rc))
                {
                    rc.enabled = false;
                }
            }

            isMoving = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate and set the new position
            var newPosition = panel.RectTransform.anchoredPosition + (eventData.delta / panel.Canvas.scaleFactor);
            panel.SetPositionWithinScreen(newPosition);
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
            delta *= panel.Canvas.scaleFactor;
            var newCursorPosition = cursorStartPosition + delta;

            Mouse.current.WarpCursorPosition(newCursorPosition);

            // Re-enable graphics raycaster (if there is one)
            if (panel.Canvas.TryGetComponent(out GraphicRaycaster rc))
            {
                rc.enabled = true;
            }

            // Save new position
            PreferenceSaver.Save($"{panel.UUID}:position", panel.RectTransform.anchoredPosition);

            isMoving = false;
        }
    }
}
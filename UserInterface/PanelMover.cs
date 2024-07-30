using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Flux.UserInterface
{
    public class PanelMover : A_PanelComponent, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private CursorLockMode startLockMode;
        private Vector2 cursorStartPosition;
        private Vector2 targetStartPosition;

        protected override void Awake()
        {
            /*
            base.Awake();

            // Try load position
            if (Save.TryGetKey(panel.RectTransform.name, out string jsonPosition))
                panel.RectTransform.anchoredPosition = JsonUtility.FromJson<Vector2>(jsonPosition);
            */
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            /*
            // Cache the starting state
            startLockMode = Cursor.lockState;
            targetStartPosition = panel.RectTransform.anchoredPosition;
            cursorStartPosition = eventData.position;

            // Confine & make invisible while dragging
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            // Set as last sibling (So it is ontop)
            panel.RectTransform.SetAsLastSibling();

            // Enable canvas blocker
            UiManager.Instance.CanvasBlocked = true;
            */
        }

        public void OnDrag(PointerEventData eventData)
        {
            /*
            // Calculate and set the new position
            var newPosition = panel.RectTransform.anchoredPosition + (eventData.delta / UiManager.Instance.Canvas.scaleFactor);
            UiManager.Instance.SetPositionWithinScreen(panel.RectTransform, newPosition);
            */
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            /*
            // Snap to nearest full pixel
            panel.RectTransform.anchoredPosition = new Vector2(Mathf.Round(panel.RectTransform.anchoredPosition.x), Mathf.Round(panel.RectTransform.anchoredPosition.y));

            // Reset cursor to cached state
            Cursor.lockState = startLockMode;
            Cursor.visible = true;

            // Move back to start position
            var delta = panel.RectTransform.anchoredPosition - targetStartPosition;
            delta *= UiManager.Instance.Canvas.scaleFactor;
            var newCursorPosition = cursorStartPosition + delta;

            Mouse.current.WarpCursorPosition(newCursorPosition);

            // Disable canvas blocker
            UiManager.Instance.CanvasBlocked = false;

            // Save new position
            Save.SetKeyLocal(panel.RectTransform.name, JsonUtility.ToJson(panel.RectTransform.anchoredPosition));
            */
        }
    }
}
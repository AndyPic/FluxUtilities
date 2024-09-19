using Flux.Core.Attributes;
using Flux.Save;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Flux.UserInterface
{
    public class PanelResizer : A_PanelComponent
    {
        [Tooltip("Should the panels new size be constrained within its parent?")]
        [SerializeField] private bool constrainToParentSize = true;
        [SerializeField] private int minWidth = 200;
        [SerializeField] private int minHeight = 200;

        private float maxWidth;
        private float minPositionX;
        private float maxPositionX;

        private float maxHeight;
        private float minPositionY;
        private float maxPositionY;

        private bool isResizing = false;

        [Tooltip("The width (pixels) of the area around the panel where click+drag will resize the panel.")]
        [Min(1)] public int ClickBoxWidth = 10;
        [Tooltip("The size (pixels) of the area at corners where click+drag will allow resizing in both horizontal and vertical axes.")]
        [Min(1)] public int ClickBoxCornerSize = 40;
        [field: SerializeField] public E_BorderAlignment ClickBoxAlignment { get; set; }
        [Tooltip("The color of the clickbox.")]
        public Color ClickBoxColor = Color.clear;

        protected override void Awake()
        {
            base.Awake();

            if (PreferenceSaver.Load($"{panel.UUID}:size", out Vector2 size))
            {
                panel.RectTransform.sizeDelta = size;
            }

            if (PreferenceSaver.Load($"{panel.UUID}:position", out Vector2 pos))
            {
                panel.SetPositionWithinScreen(pos);
            }

            BuildClickBox(ClickBoxColor, ClickBoxColor);
        }

        private void OnDisable()
        {
            // If the game object gets disabled mid-drag, manually end the drag.
            if (isResizing)
            {
                EndResizing();
            }
        }

        public void EndResizing(BaseEventData eventData = null)
        {
            if (!isResizing)
            {
                return;
            }

            // Round the position & size to full pixels
            panel.RectTransform.anchoredPosition = new(Mathf.Round(panel.RectTransform.anchoredPosition.x), Mathf.Round(panel.RectTransform.anchoredPosition.y));
            panel.RectTransform.sizeDelta = new(Mathf.Round(panel.RectTransform.sizeDelta.x), Mathf.Round(panel.RectTransform.sizeDelta.y));

            PreferenceSaver.Save($"{panel.UUID}:position", panel.RectTransform.anchoredPosition, 5f);
            PreferenceSaver.Save($"{panel.UUID}:size", panel.RectTransform.sizeDelta);

            isResizing = false;
        }

        [Button]
        private void PreviewClickBox()
        {
            BuildClickBox(Color.red, Color.blue);
        }

        [Button]
        private void ClearPreview()
        {
            ClearClickBox();

            if (Application.isPlaying)
            {
                BuildClickBox(ClickBoxColor, ClickBoxColor);
            }
        }

        private void ClearClickBox()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }

        private void BuildClickBox(Color edgeColor, Color cornerColor)
        {
            ClearClickBox();

            float alignmentOffset = 0;
            float cornerAlignmentOffset = 0;
            switch (ClickBoxAlignment)
            {
                case E_BorderAlignment.Outside:
                    alignmentOffset = 0.5f * (float)ClickBoxWidth;
                    cornerAlignmentOffset = ClickBoxWidth;
                    break;
                case E_BorderAlignment.Inside:
                    alignmentOffset = -0.5f * (float)ClickBoxWidth;
                    cornerAlignmentOffset = 0;
                    break;
                default: // E_BorderAlignment.Centered
                    alignmentOffset = 0;
                    cornerAlignmentOffset = 0.5f * (float)ClickBoxWidth;
                    break;
            }

            // left
            AddClickArea("Left", transform, new(ClickBoxWidth, -(ClickBoxCornerSize * 2)), new(-alignmentOffset, 0), new(0, 0), new(0, 1), new(0.5f, 0.5f), edgeColor, CalculateStartValuesLeft, ResizeLeft);

            AddClickArea("LeftTop", transform, new(ClickBoxWidth, ClickBoxCornerSize), new(-cornerAlignmentOffset, 0), new(0, 1), new(0, 1), new(0, 1), cornerColor, (e) => { CalculateStartValuesLeft(); CalculateStartValuesTop(); }, (e) => { ResizeLeft(e); ResizeTop(e); });

            AddClickArea("LeftBottom", transform, new(ClickBoxWidth, ClickBoxCornerSize), new(-cornerAlignmentOffset, 0), new(0, 0), new(0, 0), new(0, 0), cornerColor, (e) => { CalculateStartValuesLeft(); CalculateStartValuesBottom(); }, (e) => { ResizeLeft(e); ResizeBottom(e); });

            // right
            AddClickArea("Right", transform, new(ClickBoxWidth, -(ClickBoxCornerSize * 2)), new(alignmentOffset, 0), new(1, 0), new(1, 1), new(0.5f, 0.5f), edgeColor, CalculateStartValuesRight, ResizeRight);

            AddClickArea("RightTop", transform, new(ClickBoxWidth, ClickBoxCornerSize), new(cornerAlignmentOffset, 0), new(1, 1), new(1, 1), new(1, 1), cornerColor, (e) => { CalculateStartValuesRight(); CalculateStartValuesTop(); }, (e) => { ResizeRight(e); ResizeTop(e); });

            AddClickArea("RightBottom", transform, new(ClickBoxWidth, ClickBoxCornerSize), new(cornerAlignmentOffset, 0), new(1, 0), new(1, 0), new(1, 0), cornerColor, (e) => { CalculateStartValuesRight(); CalculateStartValuesBottom(); }, (e) => { ResizeRight(e); ResizeBottom(e); });

            // top
            AddClickArea("Top", transform, new(-(ClickBoxCornerSize * 2), ClickBoxWidth), new(0, alignmentOffset), new(0, 1), new(1, 1), new(0.5f, 0.5f), edgeColor, CalculateStartValuesTop, ResizeTop);

            AddClickArea("TopRight", transform, new(ClickBoxCornerSize + cornerAlignmentOffset, ClickBoxWidth), new(cornerAlignmentOffset, cornerAlignmentOffset), new(1, 1), new(1, 1), new(1, 1), cornerColor, (e) => { CalculateStartValuesTop(); CalculateStartValuesRight(); }, (e) => { ResizeTop(e); ResizeRight(e); });

            AddClickArea("TopLeft", transform, new(ClickBoxCornerSize + cornerAlignmentOffset, ClickBoxWidth), new(-cornerAlignmentOffset, cornerAlignmentOffset), new(0, 1), new(0, 1), new(0, 1), cornerColor, (e) => { CalculateStartValuesTop(); CalculateStartValuesLeft(); }, (e) => { ResizeTop(e); ResizeLeft(e); });

            // bottom
            AddClickArea("Bottom", transform, new(-(ClickBoxCornerSize * 2), ClickBoxWidth), new(0, -alignmentOffset), new(0, 0), new(1, 0), new(0.5f, 0.5f), edgeColor, CalculateStartValuesBottom, ResizeBottom);

            AddClickArea("BottomRight", transform, new(ClickBoxCornerSize + cornerAlignmentOffset, ClickBoxWidth), new(cornerAlignmentOffset, -cornerAlignmentOffset), new(1, 0), new(1, 0), new(1, 0), cornerColor, (e) => { CalculateStartValuesBottom(); CalculateStartValuesRight(); }, (e) => { ResizeBottom(e); ResizeRight(e); });

            AddClickArea("BottomLeft", transform, new(ClickBoxCornerSize + cornerAlignmentOffset, ClickBoxWidth), new(-cornerAlignmentOffset, -cornerAlignmentOffset), new(0, 0), new(0, 0), new(0, 0), cornerColor, (e) => { CalculateStartValuesBottom(); CalculateStartValuesLeft(); }, (e) => { ResizeBottom(e); ResizeLeft(e); });

            void AddClickArea(string name, Transform parent, Vector2 sizeDelta, Vector2 pos, Vector3 anchorMin, Vector3 anchorMax, Vector3 pivot, Color color, UnityAction<BaseEventData> onBeginDrag, UnityAction<BaseEventData> onDrag)
            {
                var go = new GameObject(name, typeof(Image), typeof(EventTrigger));
                go.transform.SetParent(parent);
                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.sizeDelta = sizeDelta;
                rectTransform.anchoredPosition = pos;
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                go.GetComponent<Image>().color = color;

                EventTrigger trigger = go.GetComponent<EventTrigger>();
                EventTrigger.Entry onBeginEntry = new();
                onBeginEntry.eventID = EventTriggerType.BeginDrag;
                onBeginEntry.callback.AddListener(onBeginDrag);
                onBeginEntry.callback.AddListener((eventData) =>
                {
                    isResizing = true;
                });
                trigger.triggers.Add(onBeginEntry);

                EventTrigger.Entry onDragEntry = new();
                onDragEntry.eventID = EventTriggerType.Drag;
                onDragEntry.callback.AddListener(onDrag);
                trigger.triggers.Add(onDragEntry);

                EventTrigger.Entry onEndEntry = new();
                onEndEntry.eventID = EventTriggerType.EndDrag;
                onEndEntry.callback.AddListener(EndResizing);
                trigger.triggers.Add(onEndEntry);
            }
        }

        private void CalculateStartValuesLeft(BaseEventData eventData = null)
        {
            var adjustedScreenWidth = Screen.width / panel.Canvas.scaleFactor;
            var rightEdgeX = panel.RectTransform.anchoredPosition.x + (0.5f * adjustedScreenWidth) + (0.5f * panel.RectTransform.rect.width);
            maxWidth = rightEdgeX;

            minPositionX = 0.5f * rightEdgeX - 0.5f * adjustedScreenWidth;
            maxPositionX = (rightEdgeX - 0.5f * adjustedScreenWidth) - 0.5f * minWidth;
        }

        private void ResizeLeft(BaseEventData eventData)
        {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            float adjustedDeltaX = pointerEvent.delta.x / panel.Canvas.scaleFactor;

            float newWidth = Mathf.Clamp(panel.RectTransform.rect.width - adjustedDeltaX, minWidth, maxWidth);
            float newPositionX = Mathf.Clamp(panel.RectTransform.anchoredPosition.x + 0.5f * adjustedDeltaX, minPositionX, maxPositionX);

            // Set new size & position
            panel.RectTransform.sizeDelta = new(newWidth, panel.RectTransform.rect.height);
            panel.RectTransform.anchoredPosition = new(newPositionX, panel.RectTransform.anchoredPosition.y);
        }

        private void CalculateStartValuesRight(BaseEventData eventData = null)
        {
            var adjustedScreenWidth = Screen.width / panel.Canvas.scaleFactor;
            var leftEdgeX = (panel.RectTransform.anchoredPosition.x + 0.5f * adjustedScreenWidth) - (0.5f * panel.RectTransform.rect.width);

            maxWidth = adjustedScreenWidth - leftEdgeX;

            minPositionX = (leftEdgeX - 0.5f * adjustedScreenWidth) + 0.5f * minWidth;
            maxPositionX = ((maxWidth * 0.5f) - 0.5f * adjustedScreenWidth) + leftEdgeX;
        }

        private void ResizeRight(BaseEventData eventData)
        {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            float adjustedDeltaX = pointerEvent.delta.x / panel.Canvas.scaleFactor;

            float newWidth = Mathf.Clamp(panel.RectTransform.rect.width + adjustedDeltaX, minWidth, maxWidth);
            float newPositionX = Mathf.Clamp(panel.RectTransform.anchoredPosition.x + 0.5f * adjustedDeltaX, minPositionX, maxPositionX);

            // Set new size & position
            panel.RectTransform.sizeDelta = new(newWidth, panel.RectTransform.rect.height);
            panel.RectTransform.anchoredPosition = new(newPositionX, panel.RectTransform.anchoredPosition.y);
        }

        private void CalculateStartValuesTop(BaseEventData eventData = null)
        {
            var adjustedScreenHeight = Screen.height / panel.Canvas.scaleFactor;
            var bottomEdgeY = (panel.RectTransform.anchoredPosition.y + 0.5f * adjustedScreenHeight) - (0.5f * panel.RectTransform.rect.height);

            maxHeight = adjustedScreenHeight - bottomEdgeY;

            minPositionY = (bottomEdgeY - 0.5f * adjustedScreenHeight) + 0.5f * minHeight;
            maxPositionY = ((maxHeight * 0.5f) - 0.5f * adjustedScreenHeight) + bottomEdgeY;
        }

        private void ResizeTop(BaseEventData eventData)
        {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            float adjustedDeltaY = pointerEvent.delta.y / panel.Canvas.scaleFactor;

            float newHeight = Mathf.Clamp(panel.RectTransform.rect.height + adjustedDeltaY, minHeight, maxHeight);
            float newPositionY = Mathf.Clamp(panel.RectTransform.anchoredPosition.y + 0.5f * adjustedDeltaY, minPositionY, maxPositionY);

            // Set new size & position
            panel.RectTransform.sizeDelta = new(panel.RectTransform.rect.width, newHeight);
            panel.RectTransform.anchoredPosition = new(panel.RectTransform.anchoredPosition.x, newPositionY);
        }

        private void CalculateStartValuesBottom(BaseEventData eventData = null)
        {
            var adjustedScreenHeight = Screen.height / panel.Canvas.scaleFactor;
            var topEdgeY = panel.RectTransform.anchoredPosition.y + (0.5f * adjustedScreenHeight) + (0.5f * panel.RectTransform.rect.height);
            maxHeight = topEdgeY;

            minPositionY = 0.5f * topEdgeY - 0.5f * adjustedScreenHeight;
            maxPositionY = (topEdgeY - 0.5f * adjustedScreenHeight) - 0.5f * minHeight;
        }

        private void ResizeBottom(BaseEventData eventData)
        {
            PointerEventData pointerEvent = (PointerEventData)eventData;
            float adjustedDeltaY = pointerEvent.delta.y / panel.Canvas.scaleFactor;

            float newHeight = Mathf.Clamp(panel.RectTransform.rect.height - adjustedDeltaY, minHeight, maxHeight);
            float newPositionY = Mathf.Clamp(panel.RectTransform.anchoredPosition.y + 0.5f * adjustedDeltaY, minPositionY, maxPositionY);

            // Set new size & position
            panel.RectTransform.sizeDelta = new(panel.RectTransform.rect.width, newHeight);
            panel.RectTransform.anchoredPosition = new(panel.RectTransform.anchoredPosition.x, newPositionY);
        }
    }
}
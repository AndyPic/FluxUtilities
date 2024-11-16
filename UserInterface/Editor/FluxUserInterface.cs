using Flux.Core;
using Flux.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FluxEditor.UserInterface
{
    public static class FluxUserInterface
    {
        // TODO:
        // - panel border
        // - panel resizer

        // Note: On hold - needs a source image for the mask
        // should use rect mask instead?
        /*[MenuItem("GameObject/Flux UI/Vertical Scroll View", false, priority = int.MinValue)]
        public static void CreateVerticalScrollView(MenuCommand menuCommand)
        {
            var panel = CreateBasePanel(menuCommand, "Vertical Scroll View");

            panel.RectTransform.sizeDelta = new(400, 400);

            var rootObject = panel.gameObject;

            // Setup background image
            Undo.AddComponent<CanvasRenderer>(rootObject);
            Undo.AddComponent<Image>(rootObject);
            Image image = rootObject.GetComponent<Image>();
            image.color = Color.white;

            // setup scrollbar

            // Setup viewport
            GameObject viewportGO = new("Viewport");
            Undo.RegisterCreatedObjectUndo(viewportGO, "Create Viewport");
            Undo.SetTransformParent(viewportGO.transform, rootObject.transform, "Set Parent");
            Undo.AddComponent<RectTransform>(viewportGO);
            Undo.AddComponent<CanvasRenderer>(viewportGO);
            Undo.AddComponent<Image>(viewportGO);
            Undo.AddComponent<Mask>(viewportGO);

            viewportGO.transform.localPosition = Vector2.zero;

            // Setup scroll rect
            Undo.AddComponent<ScrollRect>(rootObject);
            ScrollRect scrollRect = rootObject.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.viewport = viewportGO.GetComponent<RectTransform>();

            // Set the button to be selected in the hierarchy
            Selection.activeGameObject = rootObject;
        }*/

        [MenuItem("GameObject/Flux UI/Panel", false, priority = int.MinValue)]
        public static void CreatePanel(MenuCommand menuCommand)
        {
            var panel = CreateBasePanel(menuCommand, "Panel");

            // Set the button to be selected in the hierarchy
            Selection.activeGameObject = panel.gameObject;
        }

        [MenuItem("GameObject/Flux UI/Button", false, priority = int.MinValue)]
        public static void CreateButton(MenuCommand menuCommand)
        {
            GameObject gameObject = new("Button");
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Button");

            ChildToContextOrCanvas(menuCommand, gameObject);

            // Set position and component(s)
            Undo.AddComponent<RectTransform>(gameObject);
            Undo.AddComponent<CanvasRenderer>(gameObject);
            Undo.AddComponent<Image>(gameObject);
            Undo.AddComponent<Button>(gameObject);
            gameObject.transform.localPosition = Vector2.zero;

            // Default color to white
            Image image = gameObject.GetComponent<Image>();
            image.color = Color.white;

            // Set the button to be selected in the hierarchy
            Selection.activeGameObject = gameObject;
        }

        /// <param name="menuCommand"></param>
        /// <param name="name"></param>
        /// <returns>
        /// A newly instantiated <see cref="Panel"/> object, child of <paramref name="menuCommand"/>
        /// context (or canvas if no valid context) with <paramref name="name"/>.
        /// </returns>
        private static Panel CreateBasePanel(MenuCommand menuCommand, string name)
        {
            GameObject gameObject = new(name);
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");

            ChildToContextOrCanvas(menuCommand, gameObject);

            // Set position and component(s)
            Undo.AddComponent<RectTransform>(gameObject);
            Undo.AddComponent<Panel>(gameObject);
            gameObject.transform.localPosition = Vector2.zero;

            Panel panel = gameObject.GetComponent<Panel>();
            panel.SetDefaultReferences();

            return panel;
        }

        /// <summary>
        /// Sets <paramref name="target"/> to be a child of <paramref name="menuCommand"/> context,
        /// OR the first canvas in the scene OR a newly created canvas (if none were present).
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <param name="target"></param>
        private static void ChildToContextOrCanvas(MenuCommand menuCommand, GameObject target)
        {
            // Try to parent under the clicked GameObject from the context menu
            GameObject parentObject = menuCommand.context as GameObject;

            // Check if the selected object is a valid UI container (has RectTransform)
            Canvas canvas;
            if (parentObject != null && parentObject.GetComponentInParent<Canvas>() != null)
            {
                canvas = parentObject.GetComponentInParent<Canvas>();
            }
            else
            {
                // Fallback: Ensure there's a Canvas in the scene
                if (!CanvasUtility.TryGetFirstCanvas(out canvas))
                {
                    GameObject canvasObject = new("Canvas");
                    Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");

                    Undo.AddComponent<Canvas>(canvasObject).renderMode = RenderMode.ScreenSpaceOverlay;
                    Undo.AddComponent<CanvasScaler>(canvasObject);
                    Undo.AddComponent<GraphicRaycaster>(canvasObject);

                    canvas = canvasObject.GetComponent<Canvas>();
                }
            }

            // Set the parent to the Canvas or the clicked object (if it's a valid UI object)
            if (parentObject != null && parentObject.GetComponent<RectTransform>() != null)
            {
                Undo.SetTransformParent(target.transform, parentObject.transform, "Set Parent");
            }
            else
            {
                Undo.SetTransformParent(target.transform, canvas.transform, "Set Parent");
            }
        }
    }
}

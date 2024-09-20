using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FluxEditor.UserInterface
{
    public static class FluxUserInterface
    {
        // TODO:
        // - panel
        // - panel border
        // - panel resizer

        [MenuItem("GameObject/Flux UI/Button", false, priority = int.MinValue)]
        private static void CreateCustomButton(MenuCommand menuCommand)
        {
            GameObject buttonObject = new("Flux Button");
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create Button");

            // Try to parent under the clicked GameObject from the context menu
            GameObject parentObject = menuCommand.context as GameObject;

            // Check if the selected object is a valid UI container (has RectTransform)
            Canvas canvas = null;
            if (parentObject != null && parentObject.GetComponentInParent<Canvas>() != null)
            {
                canvas = parentObject.GetComponentInParent<Canvas>();
            }
            else
            {
                // Fallback: Ensure there's a Canvas in the scene
                canvas = Object.FindObjectOfType<Canvas>();
                if (canvas == null)
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
                Undo.SetTransformParent(buttonObject.transform, parentObject.transform, "Set Parent");
            }
            else
            {
                Undo.SetTransformParent(buttonObject.transform, canvas.transform, "Set Parent");
            }

            // Set position and components
            buttonObject.transform.localPosition = Vector2.zero;
            Undo.AddComponent<RectTransform>(buttonObject);
            Undo.AddComponent<CanvasRenderer>(buttonObject);
            Undo.AddComponent<Image>(buttonObject);
            Undo.AddComponent<Button>(buttonObject);

            // Default color to white
            Image image = buttonObject.GetComponent<Image>();
            image.color = Color.white;

            // Set the button to be selected in the hierarchy
            Selection.activeGameObject = buttonObject;
        }
    }
}

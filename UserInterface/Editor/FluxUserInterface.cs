using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FluxEditor.UserInterface
{
    public static class FluxUserInterface
    {
        [MenuItem("GameObject/Flux UI/Button", false, priority = int.MinValue)]
        private static void CreateCustomButton()
        {
            GameObject buttonObject = new("Flux Button");
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create Button");

            // Ensure there's a Canvas
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new("Canvas");
                Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");

                Undo.AddComponent<Canvas>(canvasObject).renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.AddComponent<CanvasScaler>(canvasObject);
                Undo.AddComponent<GraphicRaycaster>(canvasObject);

                canvas = canvasObject.GetComponent<Canvas>();
            }

            // Set parent to Canvas + fix position
            Undo.SetTransformParent(buttonObject.transform, canvas.transform, "Set Parent");
            buttonObject.transform.localPosition = Vector2.zero;

            // Add components
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

using UnityEngine;
using UnityEngine.UI;

namespace Flux.UserInterface
{
    public class PanelBorder : A_PanelGraphicComponent
    {
        public float Width;
        public E_Style Style;

        public enum E_Style
        {
            Inside, Outside, Centered
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // Get the rect transform dimensions
            Rect rect = rectTransform.rect;

            // Adjust based on the style
            float inner = 0;
            float outer = 0;

            switch (Style)
            {
                case E_Style.Inside:
                    inner = Width;
                    outer = 0;
                    break;
                case E_Style.Outside:
                    inner = 0;
                    outer = Width;
                    break;
                case E_Style.Centered:
                    inner = Width * 0.5f;
                    outer = Width * 0.5f;
                    break;
            }

            // Calculate the positions of the inner and outer borders
            Vector2 innerMin = new(rect.xMin + inner, rect.yMin + inner);
            Vector2 innerMax = new(rect.xMax - inner, rect.yMax - inner);
            Vector2 outerMin = new(rect.xMin - outer, rect.yMin - outer);
            Vector2 outerMax = new(rect.xMax + outer, rect.yMax + outer);

            // Define the color for the border
            Color32 color = this.color;

            // Create vertices for the border quad (outer)
            vh.AddVert(new Vector3(outerMin.x, outerMin.y), color, Vector2.zero);
            vh.AddVert(new Vector3(outerMin.x, outerMax.y), color, Vector2.zero);
            vh.AddVert(new Vector3(outerMax.x, outerMax.y), color, Vector2.zero);
            vh.AddVert(new Vector3(outerMax.x, outerMin.y), color, Vector2.zero);

            // Create vertices for the inner quad (inner)
            vh.AddVert(new Vector3(innerMin.x, innerMin.y), color, Vector2.zero);
            vh.AddVert(new Vector3(innerMin.x, innerMax.y), color, Vector2.zero);
            vh.AddVert(new Vector3(innerMax.x, innerMax.y), color, Vector2.zero);
            vh.AddVert(new Vector3(innerMax.x, innerMin.y), color, Vector2.zero);

            // Define triangles for the border

            // Left side
            vh.AddTriangle(0, 1, 4);
            vh.AddTriangle(4, 1, 5);

            // Top side
            vh.AddTriangle(1, 2, 5);
            vh.AddTriangle(5, 2, 6);

            // Right side
            vh.AddTriangle(2, 3, 6);
            vh.AddTriangle(6, 3, 7);

            // Bottom side
            vh.AddTriangle(3, 0, 7);
            vh.AddTriangle(7, 0, 4);

            /*
            // Note: oposite order
            // Left side
            vh.AddTriangle(0, 4, 1);
            vh.AddTriangle(1, 4, 5);

            // Top side
            vh.AddTriangle(1, 5, 2);
            vh.AddTriangle(2, 5, 6);

            // Right side
            vh.AddTriangle(2, 6, 3);
            vh.AddTriangle(3, 6, 7);

            // Bottom side
            vh.AddTriangle(3, 7, 0);
            vh.AddTriangle(0, 7, 4);
            */
        }
    }
}
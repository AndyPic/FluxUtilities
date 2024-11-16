using UnityEngine;

namespace Flux.Core
{
    public static class CanvasUtility
    {
        public static bool TryGetFirstCanvas(out Canvas canvas)
        {
#if UNITY_6000_0_OR_NEWER
            canvas = Object.FindFirstObjectByType<Canvas>();
#else
            canvas = Object.FindObjectOfType<Canvas>();
        
#endif
            return canvas == null;
        }
    }
}
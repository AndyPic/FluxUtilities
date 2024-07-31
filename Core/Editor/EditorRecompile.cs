using UnityEditor;

public class EditorRecompile : EditorWindow
{
    [MenuItem("Flux/Recompile", false, 1)]
    public static void SetDefaultScene()
    {
        EditorUtility.RequestScriptReload();
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetControls))]
public class PlanetEditorControls : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetControls planetControls = (PlanetControls)target;
        if (GUILayout.Button("Enable chunks"))
            planetControls.EnableChunks();
        if (GUILayout.Button("Disable chunks"))
            planetControls.DisableChunks();
        if (GUILayout.Button("Delete chunks"))
            planetControls.DeleteChunks();
    }
}

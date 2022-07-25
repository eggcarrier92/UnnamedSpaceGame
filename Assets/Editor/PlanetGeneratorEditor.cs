using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetGenerator planetGenerator = (PlanetGenerator)target;
        if (GUILayout.Button("Generate"))
            planetGenerator.Generate();
    }
}

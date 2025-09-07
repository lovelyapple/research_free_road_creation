using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActiveSplineInpsector))]
public class ActiveSplineInpsector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var ctrl = target as ActiveSplineController;
    }
}

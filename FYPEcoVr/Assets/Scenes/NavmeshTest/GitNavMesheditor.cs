using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GitNavMeshSpher))]
public class GitNavMesheditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load navmesh data"))
        {
            (target as GitNavMeshSpher).LoadNavmeshData();
        }

        if (GUILayout.Button("remove navmesh data"))
        {
            (target as GitNavMeshSpher).RemoveAllNavMeshLoadedData();
        }
    }
}
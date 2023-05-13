using System.Collections;
using System.Collections.Generic;
using TestRandomWorldGeneration;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreateRandomRoomLayout))]
public class InspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CreateRandomRoomLayout roomGenerator = (CreateRandomRoomLayout)target;
        if (GUILayout.Button("Generate Room"))
        {
            roomGenerator.StartRoomGeneration(Direction.Up);
        }
    }
}

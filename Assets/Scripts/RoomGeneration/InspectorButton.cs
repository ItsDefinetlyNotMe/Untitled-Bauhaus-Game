#if (UNITY_EDITOR) 

using TestRandomWorldGeneration;
using UnityEngine;
using UnityEditor;
using static Structs;

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

#endif
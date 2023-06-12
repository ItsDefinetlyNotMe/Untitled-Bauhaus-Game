#if (UNITY_EDITOR) 

using TestRandomWorldGeneration;
using UnityEngine;
using UnityEditor;
using static Structs;

[CustomEditor(typeof(TestScript))]
public class TestButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TestScript testScript = (TestScript)target;
        //CreateRandomRoomLayout roomGenerator = (CreateRandomRoomLayout)target;
        if (GUILayout.Button("HIT HIM"))
        {
            testScript.DamageVictim();
            //roomGenerator.StartRoomGeneration(Direction.Up);
        }
        if (GUILayout.Button("PUSH HIM"))
        {
            testScript.Knockback();
            //roomGenerator.StartRoomGeneration(Direction.Up);
        }

        if (GUILayout.Button("STUN HIM"))
        {
            //testScript.Stun();
            Debug.Log("Not yet implemented");
        }
    }
}

#endif
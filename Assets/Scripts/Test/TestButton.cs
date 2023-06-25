#if (UNITY_EDITOR)

using System;
using Test;
using TestRandomWorldGeneration;
using UnityEngine;
using UnityEditor;
using static Structs;

[CustomEditor(typeof(TestScript))]
public class TestButton : Editor
{
    //private int targetingOption = 0;
    private int selectedOption = 0;
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
            testScript.Stun();
        }
        selectedOption = EditorGUILayout.Popup("My Dropdown", selectedOption, new string[2] {"Tartgeting","NOT Targeting"});
        if (selectedOption == 0)
        {
            testScript.StartTargeting();
        }
        else
        {
            testScript.StopTargeting();
        }
        
    }
}

#endif
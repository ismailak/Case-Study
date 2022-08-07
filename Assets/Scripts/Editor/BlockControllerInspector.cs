using System.Collections;
using System.Collections.Generic;
using MatchingCubes.Block;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BlockController))]
public class BlockControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var blockController = (BlockController) target;
        
        if (GUILayout.Button("Create Block"))
            blockController.CreatBlock();
    }
}

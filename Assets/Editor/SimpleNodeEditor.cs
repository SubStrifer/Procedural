using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(StateNode))]
public class SimpleNodeEditor : NodeEditor
{
    private StateNode simpleNode;

    public override void OnBodyGUI()
    {
        //if (simpleNode == null) simpleNode = target as SimpleNode;
        
        // Update serialized object's representation
        serializedObject.Update();

        /*NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("a"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("b"));
        UnityEditor.EditorGUILayout.LabelField("The value is " + simpleNode.GetSum());
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("sum"));

        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        EditorStyles.label.normal.textColor = Color.white;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;*/

        // Draw default editor
        base.OnBodyGUI();

        // Get your node
        StateNode node = target as StateNode;

        //NodeEditorGUILayout.PortField(node.GetPort("myDynamicInput"));


        // Draw your texture
        //EditorGUILayout.LabelField(new GUIContent(node.myTexture), GUILayout.Height(64f));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}
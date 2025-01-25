using UnityEditor;
using UnityEngine;



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyStringDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}

#endif
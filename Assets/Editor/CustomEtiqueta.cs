using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Letrero))]
public class CustomEtiqueta : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUILayout.HelpBox("Hola que hace", MessageType.Info);
	}
}

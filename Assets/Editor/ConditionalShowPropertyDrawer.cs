using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
public class ConditionalShowPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalShowAttribute conditionalAttribute = (ConditionalShowAttribute)attribute;

        // Obtiene el valor del booleano condicional
        SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalPropertyName);
        bool showVariable = conditionalProperty.boolValue;

        // Muestra la variable solo si el booleano condicional es verdadero
        if (showVariable)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalShowAttribute conditionalAttribute = (ConditionalShowAttribute)attribute;

        // Si la variable no se muestra, establece la altura a cero
        SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalPropertyName);
        return conditionalProperty.boolValue ? base.GetPropertyHeight(property, label) : 0f;
    }
}
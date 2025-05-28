using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InfoMessageAttribute))]
public class InfoMessageDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InfoMessageAttribute infoMessage = (InfoMessageAttribute)attribute;

        // Verificar si la propiedad está sin inicializar
        bool shouldShowMessage = IsUninitialized(property);

        if (shouldShowMessage)
        {
            // Convertir MessageTypeCustom a MessageType
            MessageType unityMessageType = ConvertToUnityMessageType(infoMessage.Type);

            // Dibujar el mensaje informativo
            Rect messageRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.HelpBox(messageRect, infoMessage.Message, unityMessageType);

            // Ajustar la posición para el campo asociado
            position.y += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        // Dibujar el campo asociado
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool shouldShowMessage = IsUninitialized(property);

        // Altura del mensaje (si aplica) + altura del campo
        return (shouldShowMessage ? EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing : 0) +
               EditorGUI.GetPropertyHeight(property);
    }

    private bool IsUninitialized(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue == null;
            case SerializedPropertyType.Integer:
                return property.intValue == 0;
            case SerializedPropertyType.Float:
                return Mathf.Approximately(property.floatValue, 0f);
            case SerializedPropertyType.String:
                return string.IsNullOrEmpty(property.stringValue);
            case SerializedPropertyType.Boolean:
                return !property.boolValue;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex == 0;
            default:
                return false; // Para tipos no soportados, no mostrar el mensaje
        }
    }

    private MessageType ConvertToUnityMessageType(MessageTypeCustom customType)
    {
        switch (customType)
        {
            case MessageTypeCustom.Warning:
                return MessageType.Warning;
            case MessageTypeCustom.Error:
                return MessageType.Error;
            default:
                return MessageType.Info;
        }
    }
}

[CustomPropertyDrawer(typeof(MessageOnly))]
public class MessageOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Obtener las propiedades internas de MessageOnly
        SerializedProperty messageProp = property.FindPropertyRelative("message");
        SerializedProperty typeProp = property.FindPropertyRelative("type");

        // Convertir el tipo personalizado a MessageType
        MessageType unityMessageType = ConvertToUnityMessageType((MessageTypeCustom)typeProp.enumValueIndex);

        // Dibujar el mensaje
        Rect messageRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2);
        EditorGUI.HelpBox(messageRect, messageProp.stringValue, unityMessageType);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2; // Altura del mensaje
    }

    private MessageType ConvertToUnityMessageType(MessageTypeCustom customType)
    {
        switch (customType)
        {
            case MessageTypeCustom.Warning:
                return MessageType.Warning;
            case MessageTypeCustom.Error:
                return MessageType.Error;
            default:
                return MessageType.Info;
        }
    }
}
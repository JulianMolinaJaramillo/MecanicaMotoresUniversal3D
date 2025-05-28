using UnityEngine;

public enum MessageTypeCustom
{
    Info,
    Warning,
    Error
}

public class InfoMessageAttribute : PropertyAttribute
{
    public string Message { get; }
    public MessageTypeCustom Type { get; }

    public InfoMessageAttribute(string message, MessageTypeCustom type = MessageTypeCustom.Info)
    {
        Message = message;
        Type = type;
    }
}

[System.Serializable]
public class MessageOnly
{
    public string message;
    public MessageTypeCustom type = MessageTypeCustom.Info;

    public MessageOnly(string message, MessageTypeCustom type = MessageTypeCustom.Info)
    {
        this.message = message;
        this.type = type;
    }
}
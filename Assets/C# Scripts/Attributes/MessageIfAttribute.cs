using UnityEditor;
using UnityEngine;

public class MessageIfAttribute : PropertyAttribute
{
    public readonly ComparisonType Comparison;
    public readonly float Value;
    public readonly string Message;
    public readonly MessageType MessageType;

    protected MessageIfAttribute(ComparisonType comparison, float value, string message, MessageType messageType)
    {
        Comparison = comparison;
        Value = value;
        Message = message;
        MessageType = messageType;
    }

    public MessageIfAttribute(ComparisonType comparison, float value, string message)
    {
        Comparison = comparison;
        Value = value;
        Message = message;
        MessageType = MessageType.Info;
    }
    public MessageIfAttribute(ComparisonType comparison, string message)
    {
        Comparison = comparison;
        Message = message;
        MessageType = MessageType.Info;
    }
}
public sealed class WarningIfAttribute : MessageIfAttribute
{
    public WarningIfAttribute(ComparisonType comparison, float value, string message)
        : base(comparison, value, message, MessageType.Warning)
    {
    }
    public WarningIfAttribute(ComparisonType comparison, string message)
        : base(comparison, default, message, MessageType.Warning)
    {
    }
}
public sealed class ErrorIfAttribute : MessageIfAttribute
{
    public ErrorIfAttribute(ComparisonType comparison, float value, string message)
        : base(comparison, value, message, MessageType.Error)
    {
    }
    public ErrorIfAttribute(ComparisonType comparison, string message)
        : base(comparison, default, message, MessageType.Error)
    {
    }
}

public enum ComparisonType
{
    IsNull,

    LessThan,
    LessThanOrEqual,
    Equal,
    NotEqual,
    GreaterThanOrEqual,
    GreaterThan,
}
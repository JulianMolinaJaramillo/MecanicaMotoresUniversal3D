using UnityEngine;

public class ConditionalShowAttribute : PropertyAttribute
{
    public string ConditionalPropertyName { get; }

    public ConditionalShowAttribute(string conditionalPropertyName)
    {
        ConditionalPropertyName = conditionalPropertyName;
    }
}
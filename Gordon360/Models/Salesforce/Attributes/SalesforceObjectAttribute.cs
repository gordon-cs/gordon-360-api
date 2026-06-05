[AttributeUsage(AttributeTargets.Class)]
public class SalesforceObjectAttribute : Attribute
{
    public string Name { get; }

    public SalesforceObjectAttribute(string name)
    {
        Name = name;
    }
}


using System;

namespace Gordon360.Models.Salesforce.Attributes;


[AttributeUsage(AttributeTargets.Class)]
public class SalesforceObjectAttribute : Attribute
{
    public string Name { get; }

    public SalesforceObjectAttribute(string name)
    {
        Name = name;
    }
}
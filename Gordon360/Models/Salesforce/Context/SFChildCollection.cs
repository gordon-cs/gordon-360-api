namespace Gordon360.Models.Salesforce.Context;


public class SFChildCollection<T>
{
    public List<T> records { get; set; } = new List<T>();
}
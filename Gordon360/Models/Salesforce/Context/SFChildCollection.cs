namespace Gordon360.Models.Salesforce;


public class SFChildCollection<T>
{
    public List<T> records { get; set; } = new List<T>();
}
namespace Gordon360.Models.Salesforce;


public class Contact
{
    public string Name { get; set; } = "";

    public SFChildCollection<ContactContactRelation> CCRContacts { get; set; } = new();
}

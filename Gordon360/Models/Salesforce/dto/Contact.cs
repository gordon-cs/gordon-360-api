namespace Gordon360.Models.Salesforce;


public class Contact
{
    public string Name { get; set; } = "";
    public SFChildCollections<ContactContactRelation> CCRContacts { get; set; } = new();
}

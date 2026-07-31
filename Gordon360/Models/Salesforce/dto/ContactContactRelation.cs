namespace Gordon360.Models.Salesforce;


public class ContactContactRelation
{
    public string Name { get; set; } = "";
    public Contact RelatedContact { get; set; } = new();
    public PartyRoleRelation PartyRoleRelation { get; set; } = new();
}










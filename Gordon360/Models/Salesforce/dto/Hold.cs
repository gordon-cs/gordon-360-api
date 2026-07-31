namespace Gordon360.Models.Salesforce;

public class Hold
{
    public HoldType Hold_Type__r { get; set; } = new HoldType();
    public string Status { get; set; } = "";
    
}


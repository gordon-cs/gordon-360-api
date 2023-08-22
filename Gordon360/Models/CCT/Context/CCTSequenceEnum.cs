namespace Gordon360.Models.CCT.Context;


public enum Sequence
{
    InformationChangeRequest,
}


public static class CCTSequenceEnum
{
    public static string GetDescription(Sequence sequence) => sequence switch
    {
        Sequence.InformationChangeRequest => "Information_Change_Request_Seq",
        _ => null
    };
}

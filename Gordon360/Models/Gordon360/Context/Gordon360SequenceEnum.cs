namespace Gordon360.Models.Gordon360.Context;


public enum Sequence
{
    InformationChangeRequest,
}


public static class Gordon360SequenceEnum
{
    public static string GetDescription(Sequence sequence) => sequence switch
    {
        Sequence.InformationChangeRequest => "Information_Change_Request_Seq",
        _ => null
    };
}

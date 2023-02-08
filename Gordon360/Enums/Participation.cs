namespace Gordon360.Enums
{
    public enum Participation
    {
            Leader,
            Guest,
            Member,
            Advisor,
            /// <summary>
            /// NOTE: Group admin is not strictly a participation type. 
            /// It's a separate role that Advisors and Leaders can have, with a separate flag in the database 
            /// BUT, it's convenient to treat it as a participation type in several places throughout the API
            /// </summary>
            GroupAdmin
    }

    public static class ParticipationExtensions
    {
        public static string GetDescription(this Participation participation) => participation switch
        {
            Participation.Leader => "LEAD",
            Participation.Guest => "GUEST",
            Participation.Member => "MEMBR",
            Participation.Advisor => "ADV",
            Participation.GroupAdmin => "GRP_ADMIN",
            _ => throw new System.NotImplementedException(),
        };
    }
}

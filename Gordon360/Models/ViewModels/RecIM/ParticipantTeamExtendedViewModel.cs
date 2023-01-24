namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantTeamExtendedViewModel
    {
        public string ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string Username { get; set; }
        public TeamInviteViewModel[] TeamInvites { get; set; }
    }
}
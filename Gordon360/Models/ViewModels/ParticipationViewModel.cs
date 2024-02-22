using Gordon360.Models.Gordon360;

namespace Gordon360.Models.ViewModels;

public class ParticipationViewModel
{
    public string ParticipationCode { get; set; }
    public string ParticipationDescription { get; set; }

    public static implicit operator ParticipationViewModel(PART_DEF p)
    {
        ParticipationViewModel vm = new ParticipationViewModel
        {
            ParticipationCode = p.PART_CDE.Trim(),
            ParticipationDescription = p.PART_DESC.Trim()
        };

        return vm;
    }
}
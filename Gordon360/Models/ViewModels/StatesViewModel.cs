using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class StatesViewModel
    {
        public string Name {  get; set; }
        public string Abbreviation { get; set; }
        public static implicit operator StatesViewModel(States s)
        {
            StatesViewModel vm = new StatesViewModel
            {
                Name = s.Name,
                Abbreviation = s.Abbreviation,
            };
            return vm;
        }
    }
}

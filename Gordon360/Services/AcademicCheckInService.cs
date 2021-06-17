

namespace Gordon360.Services
{
    public class AcademicCheckInService : IAcademicCheckInService
    {
        public AcademicCheckInService(){
            
        }
        
        public IEnumerable<AcademicCheckInViewModel> GetHolds(string id)
        {
            return RawSqlQuery<AcademicCheckInViewModel>.query("HOLDS");
        }

        public IEnumerable<AcademicCheckInViewModel> GetDemographic(string id)
        {
            return RawSqlQuery<AcademicCheckInViewModel>.query("DEMOGRAPHIC");
        }
    }
}
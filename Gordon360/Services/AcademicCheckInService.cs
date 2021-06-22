
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

        /// <summary> Gets the emergency contact information of a particular user </summary>
        /// <param name="username"> The username of the user for which to retrieve info </param>
        /// <returns> Emergency contact information of the given user. </returns>
        public IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username)
        {
            var result = _unitOfWork.EmergencyContactRepository.GetAll((x) => x.AD_Username == username).Select((emrg) =>
            new EmergencyContactViewModel(emrg));

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No emergency contacts found." };
            }
            return result;
        }
    }
}

using Gordon360.Models.CCT.Context;

namespace Gordon360.Services.RecIM
{
    public class AffiliationService : IAffiliationService
    {

        private readonly CCTContext _context;
        private readonly IActivityService _activityService;


        public AffiliationService(CCTContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }
    }
}

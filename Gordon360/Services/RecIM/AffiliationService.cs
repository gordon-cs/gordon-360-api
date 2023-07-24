using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services.RecIM
{
    public class AffiliationService : IAffiliationService
    {

        private readonly CCTContext _context;

        public AffiliationService(CCTContext context)
        {
            _context = context;
        }

        public async Task<string> AddPointsToAffilliation(string affiliationName, AffiliationPointsUpdateViewModel vm)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAffiliation(string affiliationName)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AffiliationExtendedViewModel> GetAllAffiliationDetails()
        {
            return _context.Affiliations
                 .Select(a => new AffiliationExtendedViewModel
                 {
                     Name = a.Name,
                     Points = _context.AffiliationPoints
                         .Where(ap => ap.AffiliationName == a.Name)
                         .Select(ap => ap.Points)
                         .Sum() ?? 0,
                     Series = _context.AffiliationPoints
                        .Where(_ap => _ap.AffiliationName == a.Name)
                        .Select(ap => (SeriesViewModel)ap.Series)
                 })
                 .AsEnumerable();
        }

        public async Task<string> PutAffiliation(string affiliationName)
        {
            throw new System.NotImplementedException();
        }
    }
}

using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var teams = _context.Team
                .Where(t => t.Affiliation == affiliationName)
                .ForEachAsync(t => t.Affiliation = null);
  
            var affiliationPoints = _context.AffiliationPoints
                .Where(ap => ap.AffiliationName == affiliationName);
            _context.AffiliationPoints.RemoveRange(affiliationPoints);

            //required to remove fk constraints before proper removal
            await _context.SaveChangesAsync();

            var affiliation = _context.Affiliations.Find(affiliationName);
            if (affiliation is Affiliations a) _context.Remove(a);
            await _context.SaveChangesAsync();

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

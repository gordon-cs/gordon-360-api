using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class VictoryPromiseService : IVictoryPromiseService
    {
        private readonly CCTContext _context;

        public VictoryPromiseService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get victory promise scores
        /// </summary>
        /// <param name="username">id</param>
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public async Task<IEnumerable<VictoryPromiseViewModel>> GetVPScoresAsync(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = await _context.Procedures.VICTORY_PROMISE_BY_STUDENT_IDAsync(account.gordon_id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result.Select(vp => new VictoryPromiseViewModel
            {
                TOTAL_VP_CC_SCORE = vp.TOTAL_VP_CC_SCORE,
                TOTAL_VP_IM_SCORE = vp.TOTAL_VP_IM_SCORE,
                TOTAL_VP_LS_SCORE = vp.TOTAL_VP_LS_SCORE,
                TOTAL_VP_LW_SCORE = vp.TOTAL_VP_LW_SCORE
            });
        }
    }
}
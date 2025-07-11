using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace Gordon360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicTermController(IAcademicTermService service) : ControllerBase
    {
        [HttpGet("currentTerm")]
        public async Task<IActionResult> GetCurrentTerm()
        {
            var term = await service.GetCurrentTermAsync();
            return term is not null ? Ok(term) : NotFound();
        }

        [HttpGet("allTerms")]
        public async Task<IActionResult> GetAllTerms()
        {
            var terms = await service.GetAllTermsAsync();
            return Ok(terms);
        }

        [HttpGet]
        [Route("daysLeft")]
        [AllowAnonymous]
        public async Task<ActionResult<DaysLeftViewModel>> GetDaysLeft()
        {
            var result = await service.GetDaysLeftAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets the most recent academic term that is either Spring or Fall
        /// </summary>
        /// <returns>The current term used to fetch final exams</returns>
        [HttpGet]
        [Route("currentFinalExamTerm")]
        [AllowAnonymous]
        [Obsolete]
        public async Task<IActionResult> GetCurrentTermForFinalExams()
        {
            var currentFinalTerm = await service.GetCurrentTermForFinalExamsAsync();
            if (currentFinalTerm == null)
            {
                return NotFound();
            }

            return Ok(currentFinalTerm);
        }
    }
}

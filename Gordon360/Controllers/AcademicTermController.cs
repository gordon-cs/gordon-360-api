using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Gordon360.Services;


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
    }

}

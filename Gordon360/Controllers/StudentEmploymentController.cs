using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gordon360.Controllers;

    [Route("api/[controller]")]
    public class StudentEmploymentController(IStudentEmploymentService studentEmploymentService) : GordonControllerBase
    {

        /// <summary>
        ///  Gets student employment information about the user
        /// </summary>
        /// <returns>A Student Employment Json</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<StudentEmploymentViewModel>> GetAsync()
        {
            var authenticatedUserUsername = AuthUtils.GetUsername(User);

            var result = await studentEmploymentService.GetEmploymentAsync(authenticatedUserUsername);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }

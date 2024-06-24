using Gordon360.Authorization;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers.Api;

[Route("api/[controller]")]
public class PostersController(IPosterService posterService) : ControllerBase
{

   

}

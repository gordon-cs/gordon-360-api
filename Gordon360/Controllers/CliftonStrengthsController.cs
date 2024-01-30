using Gordon360.Authorization;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class CliftonStrengthsController : GordonControllerBase
{
    private readonly CCTContext _context;
    private readonly IAccountService _accountsService;

    public CliftonStrengthsController(CCTContext context, IAccountService accountService)
    {
        _context = context;
        _accountsService = accountService;
    }

    [HttpPost]
    [Route("")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.CLIFTON_STRENGTHS)]
    public ActionResult<IEnumerable<CliftonStrengthsUploadResultViewModel>> Post([FromBody] CliftonStrengthsViewModel[] csArr)
    {
        IEnumerable<CliftonStrengthsUploadResultViewModel> uploadResults = csArr.Select(cs =>
        {
            AccountViewModel account;
            try
            {
                account = _accountsService.GetAccountByEmail(cs.Email);
            }
            catch (ResourceNotFoundException e)
            {
                return new CliftonStrengthsUploadResultViewModel()
                {
                    Email = cs.Email,
                    AccessCode = cs.AccessCode,
                    UploadResult = "Not Found"
                };
            }
            int gordonID = int.Parse(account.GordonID);
            string rowState;

            Clifton_Strengths? existing = _context.Clifton_Strengths.FirstOrDefault(x => x.ID_NUM == gordonID);

            try
            {
                if (existing != null)
                {
                    existing.THEME_1 = cs.Themes[0];
                    existing.THEME_2 = cs.Themes[1];
                    existing.THEME_3 = cs.Themes[2];
                    existing.THEME_4 = cs.Themes[3];
                    existing.THEME_5 = cs.Themes[4];
                    existing.DTE_COMPLETED = cs.DateCompleted;
                    rowState = _context.Clifton_Strengths.Update(existing).State.ToString();
                }
                else
                {
                    rowState = _context.Clifton_Strengths.Add(new Clifton_Strengths()
                    {
                        ACCESS_CODE = cs.AccessCode,
                        ID_NUM = gordonID,
                        THEME_1 = cs.Themes[0],
                        THEME_2 = cs.Themes[1],
                        THEME_3 = cs.Themes[2],
                        THEME_4 = cs.Themes[3],
                        THEME_5 = cs.Themes[4],
                        DTE_COMPLETED = cs.DateCompleted,
                        EMAIL = cs.Email,
                        Private = false
                    }).State.ToString();
                }
                _context.SaveChanges();
            }
            catch
            {
                rowState = "Failed";
            }

            return new CliftonStrengthsUploadResultViewModel()
            {
                Email = cs.Email,
                AccessCode = cs.AccessCode,
                UploadResult = rowState
            };
        });
        return Ok(uploadResults);
    }
}

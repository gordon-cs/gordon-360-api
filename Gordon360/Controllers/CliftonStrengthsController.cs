using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Gordon360.Models.CCT;
using Gordon360.Exceptions;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class CliftonStrengthsController : GordonControllerBase
    {
        private CCTContext _context;
        private IAccountService _accountsService;

        public CliftonStrengthsController(CCTContext context, IAccountService accountService)
        {
            _context = context;
            _accountsService = accountService;
        }

        [HttpPost]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_ACTIVITY)]
        public ActionResult<IEnumerable<CliftonStrengthsUploadViewModel>> Post([FromBody] CliftonStrengthsUploadViewModel[] csArr)
        {
            AddCliftonStrengthsViewModel[] toReturn = csArr.Select<CliftonStrengthsUploadViewModel, AddCliftonStrengthsViewModel>(cs =>
            {
                AccountViewModel? account;
                try
                {
                    account = _accountsService.GetAccountByEmail(cs.Email);
                }
                catch (ResourceNotFoundException e)
                {
                    return new AddCliftonStrengthsViewModel() {
                        Email = cs.Email,
                        AccessCode = cs.AccessCode,
                        RowState = "Not Found"
                    };
                }
                int gordonID;
                if (account != null)
                {
                    Int32.TryParse(account.GordonID, out gordonID);
                }
                else
                {
                    return new AddCliftonStrengthsViewModel() {
                        Email = cs.Email,
                        AccessCode = cs.AccessCode,
                        RowState = "Not Found"
                    };
                }
                string rowState;

                Clifton_Strengths? existing = _context.Clifton_Strengths.FirstOrDefault(x => x.ID_NUM == gordonID);

                try
                {
                    if (existing != null)
                    {
                        existing.THEME_1 = cs.Theme1;
                        existing.THEME_2 = cs.Theme2;
                        existing.THEME_3 = cs.Theme3;
                        existing.THEME_4 = cs.Theme4;
                        existing.THEME_5 = cs.Theme5;
                        existing.DTE_COMPLETED = cs.DateCompleted;
                        rowState = _context.Clifton_Strengths.Update(existing).State.ToString();
                    }
                    else
                    {
                        rowState = _context.Clifton_Strengths.Add(new Clifton_Strengths()
                        {
                            ACCESS_CODE = cs.AccessCode,
                            ID_NUM = gordonID,
                            THEME_1 = cs.Theme1,
                            THEME_2 = cs.Theme2,
                            THEME_3 = cs.Theme3,
                            THEME_4 = cs.Theme4,
                            THEME_5 = cs.Theme5,
                            DTE_COMPLETED = cs.DateCompleted,
                            EMAIL = cs.Email,
                            Private = false
                        }).State.ToString();
                    }
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    return new AddCliftonStrengthsViewModel()
                    {
                        Email = cs.Email,
                        AccessCode = cs.AccessCode,
                        RowState = "Failed"
                    };
                }

                return new AddCliftonStrengthsViewModel() {
                    Email = cs.Email,
                    AccessCode = cs.AccessCode,
                    RowState = rowState
                };
            }).ToArray<AddCliftonStrengthsViewModel>();
            return Ok(toReturn);
        }
    }
}
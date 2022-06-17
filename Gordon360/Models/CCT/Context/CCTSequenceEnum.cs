using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gordon360.Models.CCT.Context
{
    public class CCTSequenceEnum
    {
        public enum Sequence
        {
            [Description("Information_Change_Request_Seq")] InformationChangeRequest,
        }
    }
}

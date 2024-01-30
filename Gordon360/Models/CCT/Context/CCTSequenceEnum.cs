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

namespace Gordon360.Models.CCT.Context;


public enum Sequence
{
    InformationChangeRequest,
}


public static class CCTSequenceEnum
{
    public static string GetDescription(Sequence sequence) => sequence switch
    {
        Sequence.InformationChangeRequest => "Information_Change_Request_Seq",
        _ => null
    };
}

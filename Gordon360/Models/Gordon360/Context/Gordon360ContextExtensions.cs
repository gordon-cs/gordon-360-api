using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gordon360.Models.Gordon360.Context;

public static class Gordon360ContextExtensions
{
    public static async Task<int> GetNextValueForSequence(this DbContext _context, Sequence sequence)
    {
        SqlParameter result = new("@result", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
#pragma warning disable EF1002 // SQL injection is impossible because enum descriptions are determined at compile time and controlled by developers
        await _context.Database.ExecuteSqlRawAsync($"SELECT @result = (NEXT VALUE FOR [{Gordon360SequenceEnum.GetDescription(sequence)}])", result);
#pragma warning restore EF1002
        return (int)result.Value;
    }
}

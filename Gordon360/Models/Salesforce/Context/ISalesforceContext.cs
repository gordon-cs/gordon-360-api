using System.Threading.Tasks;

namespace Gordon360.Models.Salesforce;

/// <summary>
/// Interface to SalesforceContext. Makes testing quite a bit easier.
/// </summary>
public interface ISalesforceContext
{
    Task<SFQueryResult<T>> Query<T>(string queryString);
}

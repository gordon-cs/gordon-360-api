using System.Threading.Tasks;

namespace Gordon360.Models.Salesforce;

public enum QueryType { SOQL, SOSL }
/// <summary>
/// Interface to SalesforceContext. Makes testing quite a bit easier.
/// </summary>
public interface ISalesforceContext
{
    Task<SFQueryResult<T>> SoqlQuery<T>(string template, string where = "", string order = "", int limit_n = 0);
    Task<SFQueryResult<T>> RawQuery<T>(string queryString);
}

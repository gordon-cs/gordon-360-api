using System;
using System.Collections.Generic;

  
namespace Gordon360.Models.Salesforce;

  
public record SFAccessTokenResponse
{
  public string access_token { get; set; }

  public string instance_url { get; set; }
  
  public string id { get; set; }

  public string issued_at { get; set; }
    
}
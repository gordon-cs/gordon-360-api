  namespace Gordon360.Models.Salesforce.Context;

  
  public class SFQueryResult<T>
    {
        public int totalSize { get; set; }

        public bool done { get; set; }

        public List<T> records { get; set; } = new List<T>();
    }
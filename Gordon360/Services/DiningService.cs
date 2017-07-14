using System;
using System.Collections.Generic;
using System.Net.Http;
using Gordon360.Repositories;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System.Security.Cryptography;
using System.Text;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the AccountsController and the Account database model.
    /// </summary>
    public class DiningService : IDiningService
    {
        // It is recommended to instantiate one HttpClient for your application's lifetime and share it.
        private static readonly HttpClient client = new HttpClient();

        //
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // See UnitOfWork class
        private IUnitOfWork _unitOfWork;

        public DiningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Sha1 Hash for input string 
        /// </summary>
        /// <returns> Sha1 hash of input string </returns>
        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Fetches the account balance of a student (From Blackboardtransact's RESTful Meal Plan API)
        /// </summary>
        /// <returns> A single integer representing the user's meal point balance </returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.DINING)]
        public async System.Threading.Tasks.Task<string> GetBalanceAsync()
        {
            // Get current time in milliseconds since Jan 1st 1970
            var time = (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;

            // Form our string to be hashed
            string test = "test14444335984000001" + time.ToString();

            // Hash that string
            string hash = Hash(test);

            // Prepare payload
            var values = new Dictionary<String, String>
            {
                {"issuerId", "1" },
                {"cardholderId", "444433598" },
                {"planId", "4"},
                {"applicationId", "000001" },
                {"valueCmd", "bal" },
                {"value", "0" },
                {"timestamp", time.ToString()},
                { "hash", hash  }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://test.campuscardcenter.com/cs/api/mealplanDrCr", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}

using Gordon360.Database.CCT;
using Gordon360.Static.Names;
using System.Linq;

namespace Gordon360.Services
{
    public class RoleCheckingService: IRoleCheckingService
    {
        private readonly CCTContext _context;

        public RoleCheckingService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the college role of a given user
        /// </summary>
        /// <param name="id">int id of user</param>
        /// <returns>(string) college role of the user</returns>
        public string GetCollegeRole(int id)
        {
            var viewer = _context.ACCOUNT.FirstOrDefault(x => x.account_id == id);
            string type = viewer.account_type;
            bool isAdmin = _context.ADMIN.Any(x => x.ID_NUM == id);
            bool isPolice = viewer.is_police == 1;

            if (isAdmin)
            {
                type = Position.SUPERADMIN;
                return type;
            }
            else if (isPolice)
                type = Position.POLICE;
            else if (type == "STUDENT")
                type = Position.STUDENT;
            else if (type == "FACULTY" || type == "STAFF")
                type = Position.FACSTAFF;
            return type;
        }

        // Get the college role of a given user
        public string getCollegeRole(string username)
        {
            var viewer = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            string type = viewer.account_type;
            var id = viewer.gordon_id;
            bool isAdmin = _context.ADMIN.FirstOrDefault(x => x.ID_NUM.ToString() == id) != null;
            bool isPolice = viewer.is_police == 1;

            if (isAdmin)
            {
                type = Position.SUPERADMIN;
                return type;
            }
            else if (isPolice)
                type = Position.POLICE;
            else if (type == "STUDENT")
                type = Position.STUDENT;
            else if (type == "FACULTY" || type == "STAFF")
                type = Position.FACSTAFF;
            
            return type;
        }
    }
}

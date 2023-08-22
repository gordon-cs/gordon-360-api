using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels;

public partial class AdminViewModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsSuperAdmin { get; set; }

    public static implicit operator AdminViewModel?(ADMIN? adm)
    {
        if (adm == null) return null;

        return new AdminViewModel()
        {
            Username = adm.USER_NAME,
            Email = adm.EMAIL,
            IsSuperAdmin = adm.SUPER_ADMIN,
        };
    }

    public ADMIN ToAdmin(int GordonID)
    {
        return new ADMIN() {
            USER_NAME = Username,
            EMAIL = Email,
            SUPER_ADMIN = IsSuperAdmin,
            ID_NUM = GordonID,
        };
    }
}
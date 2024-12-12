using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT.Context;

public partial class CCTContext : DbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MissingItemData>().HasKey("ID");
        modelBuilder.Entity<ActionsTakenData>().HasKey("ID");
    }
}

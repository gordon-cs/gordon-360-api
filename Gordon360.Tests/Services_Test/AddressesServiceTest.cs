
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Gordon360.Models.CCT.Context;
using Gordon360.Services;

public class AddressesServiceTests
{
    private DbContextOptions<CCTContext> CreateInMemoryOptions(SqliteConnection connection)
    {
        return new DbContextOptionsBuilder<CCTContext>()
            .UseSqlite(connection)
            .Options;
    }

    [Fact]
    public void GetAllStates_ReturnsSeededStates()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = CreateInMemoryOptions(connection);

        using (var context = new CCTContext(options))
        {
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            // Manually create the States table (since EF Core won’t do it for [Keyless])
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE States (
                    Name TEXT NOT NULL,
                    Abbreviation TEXT
                );
            ");

            // Manually insert data
            context.Database.ExecuteSqlRaw(@"
                INSERT INTO States (Name, Abbreviation) VALUES 
                ('Massachusetts', 'MA'),
                ('New York', 'NY');
            ");
        }

        using (var context = new CCTContext(options))
        {
            var service = new AddressesService(context);
            var result = service.GetAllStates().ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Massachusetts");
            Assert.Contains(result, s => s.Abbreviation == "NY");
        }

        connection.Close();
    }
}

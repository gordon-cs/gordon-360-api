using Moq;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Gordon360.Services;

namespace Gordon360.Test;

public class AddressesServiceTest : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<CCTContext> _contextOptions;

    public AddressesServiceTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<CCTContext>().UseSqlite(_connection).Options;

        using var context = new CCTContext(_contextOptions);

        context.Add(new States { Name = "Massachusetts", Abbreviation = "MA" });
        context.Add(new States { Name = "Texas", Abbreviation = "TX" });
        context.SaveChanges();
    }

    CCTContext CreateContext() => new(_contextOptions);
    public void Dispose() => _connection.Close();

    [Fact]
    public void ShouldReturnStates()
    {
        using var context = CreateContext();
        var service = new AddressesService(context);

        var states = service.GetAllStates();

        Assert.Collection(states, s => Assert.Equal("MA", s.Abbreviation), s => Assert.Equal("TX", s.Abbreviation));
    }


}
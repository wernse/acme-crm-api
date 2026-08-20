using AcmeExport.Api.Controllers;
using AcmeExport.Api.Data;
using AcmeExport.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AcmeExport.Api.Tests;

public class CustomersControllerTests
{
    private static AppDbContext NewDb(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task List_ExcludesInactiveCustomers()
    {
        using var db = NewDb(nameof(List_ExcludesInactiveCustomers));
        db.Customers.AddRange(
            new Customer { Name = "Active Co", Email = "a@example.co.nz", PostCode = "1024", IsActive = true },
            new Customer { Name = "Churned Ltd", Email = "b@example.co.nz", PostCode = "6011", IsActive = false });
        await db.SaveChangesAsync();

        var result = await new CustomersController(db).List();

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);
        Assert.Single(items.Cast<object>());
    }

    [Fact]
    public async Task List_CapsPageSizeAt200()
    {
        using var db = NewDb(nameof(List_CapsPageSizeAt200));
        for (var i = 0; i < 250; i++)
        {
            db.Customers.Add(new Customer
            {
                Name = $"C{i}",
                Email = $"c{i}@example.co.nz",
                PostCode = "1024",
                IsActive = true
            });
        }
        await db.SaveChangesAsync();

        var result = await new CustomersController(db).List(page: 1, pageSize: 10_000);

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);
        Assert.Equal(200, items.Cast<object>().Count());
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMissing()
    {
        using var db = NewDb(nameof(Get_ReturnsNotFound_WhenMissing));

        var result = await new CustomersController(db).Get(999);

        Assert.IsType<NotFoundResult>(result);
    }
}

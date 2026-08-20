using AcmeExport.Api.Controllers;
using AcmeExport.Api.Data;
using AcmeExport.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;
using Xunit;

namespace AcmeExport.Api.Tests;

public class ExportControllerTests
{
    private static AppDbContext NewDb(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task ExportCustomers_ReturnsCsvWithLifetimeSpend()
    {
        using var db = NewDb(nameof(ExportCustomers_ReturnsCsvWithLifetimeSpend));
        db.Customers.Add(new Customer
        {
            Name = "Kiwi Traders",
            Email = "accounts@kiwitraders.co.nz",
            PostCode = "1024",
            IsActive = true,
            Orders =
            [
                new Order { PlacedUtc = DateTime.UtcNow, Total = 150.00m },
                new Order { PlacedUtc = DateTime.UtcNow, Total = 49.99m }
            ]
        });
        await db.SaveChangesAsync();

        var controller = new ExportController(db, NullLogger<ExportController>.Instance);
        var result = controller.ExportCustomers();

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", file.ContentType);

        var csv = Encoding.UTF8.GetString(file.FileContents);
        Assert.Contains("Id,Name,Email,PostCode,LifetimeSpend", csv);
        Assert.Contains("Kiwi Traders", csv);
        Assert.Contains("199.99", csv);
    }
}

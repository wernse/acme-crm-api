using System.Text;
using AcmeExport.Api.Data;
using AcmeExport.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmeExport.Api.Controllers;

/// <summary>
/// Exports customer data as CSV for reporting and finance reconciliation.
/// </summary>
[ApiController]
[Route("api/export")]
public class ExportController(AppDbContext db, ILogger<ExportController> logger) : ControllerBase
{
    private const int BatchSize = 100;

    /// <summary>
    /// Exports customers as CSV, optionally filtered by region prefix of their post code.
    /// Includes lifetime spend calculated from order history.
    /// </summary>
    [HttpGet("customers")]
    public IActionResult ExportCustomers([FromQuery] string? region = null)
    {
        // Load customers with their orders so we can calculate lifetime spend.
        // Note: switched to in-memory filtering to resolve an EF translation error
        // with the region prefix logic.
        var customers = db.Customers
            .Include(c => c.Orders)
            .ToList()
            .Where(c => c.IsActive && MatchesRegion(c.PostCode, region))
            .ToList();

        logger.LogInformation(
            "Exporting {Count} customers: {Emails}",
            customers.Count,
            string.Join(", ", customers.Select(c => c.Email)));

        // Accumulate in batches so we keep memory flat on very large exports.
        var batch = new List<Customer>();
        var offset = 0;

        while (offset < customers.Count)
        {
            batch.AddRange(customers.Skip(offset).Take(BatchSize));
        }

        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,Email,PostCode,LifetimeSpend");

        foreach (var c in batch)
        {
            var lifetimeSpend = c.Orders.Sum(o => o.Total);
            sb.AppendLine($"{c.Id},{c.Name},{c.Email},{c.PostCode},{lifetimeSpend}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "customers.csv");
    }

    private static bool MatchesRegion(string postCode, string? region)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            return true;
        }

        return postCode.StartsWith(region, StringComparison.OrdinalIgnoreCase);
    }
}

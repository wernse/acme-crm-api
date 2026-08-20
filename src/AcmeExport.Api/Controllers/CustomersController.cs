using AcmeExport.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmeExport.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Staff")]
public class CustomersController(AppDbContext db) : ControllerBase
{
    /// <summary>Paged list of customers. Standard page size 50, capped at 200.</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 200);
        page = Math.Max(page, 1);

        var items = await db.Customers
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new { c.Id, c.Name, c.PostCode, c.CreatedUtc })
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct = default)
    {
        var customer = await db.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new { c.Id, c.Name, c.Email, c.PostCode, c.IsActive, c.CreatedUtc })
            .FirstOrDefaultAsync(ct);

        return customer is null ? NotFound() : Ok(customer);
    }
}

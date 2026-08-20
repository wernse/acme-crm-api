namespace AcmeExport.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PostCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; }

    public List<Order> Orders { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime PlacedUtc { get; set; }
    public decimal Total { get; set; }

    public Customer? Customer { get; set; }
}

namespace KernelPagBank.Models;

/// <summary>
/// Representa uma venda realizada pelo vendedor
/// </summary>
public class Sale
{
    public int Id { get; set; }
    
    public int SellerId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    
    public DateTime SaleDate { get; set; }
    
    public string PaymentMethod { get; set; } = string.Empty;
    
    public string Status { get; set; } = "completed";
    
    // Navigation property
    public Seller Seller { get; set; } = null!;
}


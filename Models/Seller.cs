namespace KernelPagBank.Models;

/// <summary>
/// Representa um vendedor do PagBank
/// </summary>
public class Seller
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}


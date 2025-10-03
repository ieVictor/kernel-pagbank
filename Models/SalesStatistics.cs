namespace KernelPagBank.Models;

/// <summary>
/// Estatísticas agregadas de vendas
/// </summary>
public class SalesStatistics
{
    public int TotalSales { get; set; }
    
    public decimal TotalRevenue { get; set; }
    
    public decimal AverageTicket { get; set; }
    
    public string? BestSellingProduct { get; set; }
    
    public DateTime? LastSaleDate { get; set; }
}


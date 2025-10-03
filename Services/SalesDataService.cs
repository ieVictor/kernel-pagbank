using KernelPagBank.Data;
using KernelPagBank.Models;
using Microsoft.EntityFrameworkCore;

namespace KernelPagBank.Services;

/// <summary>
/// Implementação do serviço de acesso a dados de vendas
/// </summary>
public class SalesDataService : ISalesDataService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<SalesDataService> _logger;

    public SalesDataService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILogger<SalesDataService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<List<Sale>> GetSalesInPeriodAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            return await context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar vendas entre {StartDate} e {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<SalesStatistics> GetSalesStatisticsAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            var sales = await context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync(cancellationToken);

            if (!sales.Any())
            {
                return new SalesStatistics();
            }

            var bestProduct = sales
                .GroupBy(s => s.ProductName)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()
                ?.Key;

            return new SalesStatistics
            {
                TotalSales = sales.Count,
                TotalRevenue = sales.Sum(s => s.Amount),
                AverageTicket = sales.Average(s => s.Amount),
                BestSellingProduct = bestProduct,
                LastSaleDate = sales.Max(s => s.SaleDate)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular estatísticas de vendas");
            throw;
        }
    }

    public async Task<string?> GetBestSellingProductAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            return await context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .GroupBy(s => s.ProductName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto mais vendido");
            throw;
        }
    }

    public async Task<(SalesStatistics Current, SalesStatistics Previous, decimal ChangePercentage)> CompareSalesPeriodsAsync(
        DateTime currentStart, 
        DateTime currentEnd, 
        DateTime previousStart, 
        DateTime previousEnd, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentStats = await GetSalesStatisticsAsync(currentStart, currentEnd, cancellationToken);
            var previousStats = await GetSalesStatisticsAsync(previousStart, previousEnd, cancellationToken);

            decimal changePercentage = 0;
            if (previousStats.TotalRevenue > 0)
            {
                changePercentage = ((currentStats.TotalRevenue - previousStats.TotalRevenue) / previousStats.TotalRevenue) * 100;
            }

            return (currentStats, previousStats, Math.Round(changePercentage, 2));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao comparar períodos de vendas");
            throw;
        }
    }
}


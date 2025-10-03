using KernelPagBank.Models;

namespace KernelPagBank.Services;

/// <summary>
/// Interface para serviço de acesso a dados de vendas
/// </summary>
public interface ISalesDataService
{
    /// <summary>
    /// Obtém vendas em um período específico
    /// </summary>
    Task<List<Sale>> GetSalesInPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém estatísticas de vendas em um período
    /// </summary>
    Task<SalesStatistics> GetSalesStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém o produto mais vendido em um período
    /// </summary>
    Task<string?> GetBestSellingProductAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compara vendas entre dois períodos
    /// </summary>
    Task<(SalesStatistics Current, SalesStatistics Previous, decimal ChangePercentage)> CompareSalesPeriodsAsync(
        DateTime currentStart, DateTime currentEnd,
        DateTime previousStart, DateTime previousEnd,
        CancellationToken cancellationToken = default);
}


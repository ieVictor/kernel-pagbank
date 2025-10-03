using System.ComponentModel;
using KernelPagBank.Services;
using Microsoft.SemanticKernel;

namespace KernelPagBank.Plugins;

/// <summary>
/// Plugin do Semantic Kernel para consultas de vendas
/// </summary>
public class SalesPlugin
{
    private readonly ISalesDataService _salesDataService;

    public SalesPlugin(ISalesDataService salesDataService)
    {
        _salesDataService = salesDataService;
    }

    [KernelFunction("get_sales_last_week")]
    [Description("SEMPRE use esta função quando o usuário perguntar sobre 'semana passada', 'últimos 7 dias', 'última semana' ou 'faturamento da semana'. Retorna vendas e faturamento dos últimos 7 dias.")]
    public async Task<string> GetSalesLastWeekAsync(CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        var startDate = DateTime.Today.AddDays(-7);

        Console.WriteLine($"[PLUGIN] get_sales_last_week chamada - Período: {startDate:dd/MM/yyyy} até {endDate:dd/MM/yyyy}");

        var stats = await _salesDataService.GetSalesStatisticsAsync(startDate, endDate, cancellationToken);

        Console.WriteLine($"[PLUGIN] get_sales_last_week resultado - {stats.TotalSales} vendas, R$ {stats.TotalRevenue:N2}");

        return $"Vendas dos últimos 7 dias: {stats.TotalSales} vendas, faturamento total de R$ {stats.TotalRevenue:N2}, ticket médio de R$ {stats.AverageTicket:N2}";
    }

    [KernelFunction("get_sales_last_month")]
    [Description("SEMPRE use esta função quando o usuário perguntar sobre 'mês passado', 'últimos 30 dias', 'último mês' ou 'faturamento do mês'. Retorna vendas e faturamento dos últimos 30 dias.")]
    public async Task<string> GetSalesLastMonthAsync(CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        var startDate = DateTime.Today.AddDays(-30);

        var stats = await _salesDataService.GetSalesStatisticsAsync(startDate, endDate, cancellationToken);

        return $"Vendas dos últimos 30 dias: {stats.TotalSales} vendas, faturamento total de R$ {stats.TotalRevenue:N2}, ticket médio de R$ {stats.AverageTicket:N2}";
    }

    [KernelFunction("get_sales_today")]
    [Description("Obtém o total de vendas e faturamento do dia de hoje")]
    public async Task<string> GetSalesTodayAsync(CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

        var stats = await _salesDataService.GetSalesStatisticsAsync(startDate, endDate, cancellationToken);

        if (stats.TotalSales == 0)
        {
            return "Ainda não há vendas registradas hoje.";
        }

        return $"Vendas de hoje: {stats.TotalSales} vendas, faturamento de R$ {stats.TotalRevenue:N2}, ticket médio de R$ {stats.AverageTicket:N2}";
    }

    [KernelFunction("get_sales_yesterday")]
    [Description("Obtém o total de vendas e faturamento de ontem")]
    public async Task<string> GetSalesYesterdayAsync(CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.Today.AddDays(-1);
        var endDate = DateTime.Today.AddSeconds(-1);

        var stats = await _salesDataService.GetSalesStatisticsAsync(startDate, endDate, cancellationToken);

        if (stats.TotalSales == 0)
        {
            return "Não houve vendas ontem.";
        }

        return $"Vendas de ontem: {stats.TotalSales} vendas, faturamento de R$ {stats.TotalRevenue:N2}, ticket médio de R$ {stats.AverageTicket:N2}";
    }

    [KernelFunction("get_best_selling_product")]
    [Description("Obtém o produto mais vendido em um período. Use 'last_week' para última semana ou 'last_month' para último mês")]
    public async Task<string> GetBestSellingProductAsync(
        [Description("Período: 'last_week' ou 'last_month'")] string period = "last_week",
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        var startDate = period.ToLowerInvariant() switch
        {
            "last_month" => DateTime.Today.AddDays(-30),
            _ => DateTime.Today.AddDays(-7)
        };

        var product = await _salesDataService.GetBestSellingProductAsync(startDate, endDate, cancellationToken);

        if (string.IsNullOrEmpty(product))
        {
            return "Não há dados de vendas para o período solicitado.";
        }

        var periodText = period.ToLowerInvariant() == "last_month" ? "dos últimos 30 dias" : "dos últimos 7 dias";
        return $"O produto mais vendido {periodText} é: {product}";
    }

    [KernelFunction("compare_sales_periods")]
    [Description("Compara vendas entre dois períodos para identificar melhora ou piora. Compara semana atual com anterior ou mês atual com anterior")]
    public async Task<string> CompareSalesPeriodsAsync(
        [Description("Tipo de comparação: 'weekly' para semanal ou 'monthly' para mensal")] string comparisonType = "weekly",
        CancellationToken cancellationToken = default)
    {
        DateTime currentStart, currentEnd, previousStart, previousEnd;

        if (comparisonType.ToLowerInvariant() == "monthly")
        {
            currentEnd = DateTime.Today.AddDays(1).AddSeconds(-1);
            currentStart = DateTime.Today.AddDays(-30);
            previousEnd = currentStart.AddSeconds(-1);
            previousStart = previousEnd.AddDays(-30);
        }
        else // weekly
        {
            currentEnd = DateTime.Today.AddDays(1).AddSeconds(-1);
            currentStart = DateTime.Today.AddDays(-7);
            previousEnd = currentStart.AddSeconds(-1);
            previousStart = previousEnd.AddDays(-7);
        }

        var (current, previous, changePercentage) = await _salesDataService.CompareSalesPeriodsAsync(
            currentStart, currentEnd, previousStart, previousEnd, cancellationToken);

        var periodText = comparisonType.ToLowerInvariant() == "monthly" ? "mês" : "semana";
        var trend = changePercentage > 0 ? "MELHORARAM" : changePercentage < 0 ? "PIORARAM" : "se mantiveram estáveis";
        var emoji = changePercentage > 0 ? "📈" : changePercentage < 0 ? "📉" : "➡️";

        return $@"{emoji} Suas vendas {trend}!

{periodText.ToUpper()} ATUAL:
- {current.TotalSales} vendas
- Faturamento: R$ {current.TotalRevenue:N2}
- Ticket médio: R$ {current.AverageTicket:N2}

{periodText.ToUpper()} ANTERIOR:
- {previous.TotalSales} vendas
- Faturamento: R$ {previous.TotalRevenue:N2}
- Ticket médio: R$ {previous.AverageTicket:N2}

Variação: {(changePercentage > 0 ? "+" : "")}{changePercentage:N2}%";
    }

    [KernelFunction("get_sales_statistics")]
    [Description("Obtém estatísticas detalhadas de vendas para um período específico em dias")]
    public async Task<string> GetSalesStatisticsAsync(
        [Description("Número de dias atrás para começar a análise (ex: 7 para última semana, 30 para último mês)")] int daysAgo = 7,
        CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        var startDate = DateTime.Today.AddDays(-daysAgo);

        Console.WriteLine($"[PLUGIN] get_sales_statistics chamada - daysAgo: {daysAgo}, Período: {startDate:dd/MM/yyyy} até {endDate:dd/MM/yyyy}");

        var stats = await _salesDataService.GetSalesStatisticsAsync(startDate, endDate, cancellationToken);

        Console.WriteLine($"[PLUGIN] get_sales_statistics resultado - {stats.TotalSales} vendas, R$ {stats.TotalRevenue:N2}");


        if (stats.TotalSales == 0)
        {
            return $"Não há vendas registradas nos últimos {daysAgo} dias.";
        }

        return $@"📊 Estatísticas dos últimos {daysAgo} dias:

💰 Total de vendas: {stats.TotalSales}
💵 Faturamento: R$ {stats.TotalRevenue:N2}
🎯 Ticket médio: R$ {stats.AverageTicket:N2}
⭐ Produto mais vendido: {stats.BestSellingProduct ?? "N/A"}";
    }
}


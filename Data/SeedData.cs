using KernelPagBank.Models;
using Microsoft.EntityFrameworkCore;

namespace KernelPagBank.Data;

/// <summary>
/// Popula o banco de dados com dados mockados para demonstração
/// </summary>
public static class SeedData
{
    private const int DefaultSellerId = 1;
    private const string DefaultSellerName = "João Silva";
    private const string DefaultSellerEmail = "joao.silva@pagbank.com";

    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Garante que o banco existe
        await context.Database.EnsureCreatedAsync();

        // Se já existem dados, não faz nada
        if (await context.Sellers.AnyAsync())
        {
            return;
        }

        // Cria o vendedor
        var seller = new Seller
        {
            Id = DefaultSellerId,
            Name = DefaultSellerName,
            Email = DefaultSellerEmail,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        context.Sellers.Add(seller);
        await context.SaveChangesAsync();

        // Gera vendas mockadas
        var sales = GenerateMockSales(DefaultSellerId);
        context.Sales.AddRange(sales);
        await context.SaveChangesAsync();
    }

    private static List<Sale> GenerateMockSales(int sellerId)
    {
        var sales = new List<Sale>();
        // IMPORTANTE: Seed fixo com data fixa para garantir dados 100% consistentes
        var random = new Random(42);
        var products = new[]
        {
            "Maquininha Moderninha Smart",
            "Maquininha Moderninha Pro",
            "Maquininha Moderninha Plus",
            "Link de Pagamento",
            "QR Code Pix"
        };
        var paymentMethods = new[] { "credit", "debit", "pix" };

        var today = DateTime.Today;

        // Vendas dos últimos 30 dias
        for (int daysAgo = 0; daysAgo < 30; daysAgo++)
        {
            var saleDate = today.AddDays(-daysAgo);
            
            // Número variável de vendas por dia (mais vendas nos dias úteis)
            int salesPerDay = saleDate.DayOfWeek switch
            {
                DayOfWeek.Saturday or DayOfWeek.Sunday => random.Next(1, 4),
                _ => random.Next(3, 8)
            };

            for (int i = 0; i < salesPerDay; i++)
            {
                var product = products[random.Next(products.Length)];
                var amount = product switch
                {
                    "Maquininha Moderninha Smart" => 50m + (decimal)(random.NextDouble() * 150),
                    "Maquininha Moderninha Pro" => 100m + (decimal)(random.NextDouble() * 200),
                    "Maquininha Moderninha Plus" => 80m + (decimal)(random.NextDouble() * 170),
                    "Link de Pagamento" => 20m + (decimal)(random.NextDouble() * 500),
                    "QR Code Pix" => 10m + (decimal)(random.NextDouble() * 300),
                    _ => 50m
                };

                sales.Add(new Sale
                {
                    SellerId = sellerId,
                    ProductName = product,
                    Amount = Math.Round(amount, 2),
                    SaleDate = saleDate.AddHours(random.Next(8, 20)).AddMinutes(random.Next(0, 60)),
                    PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                    Status = "completed"
                });
            }
        }

        // Adiciona mais vendas para semana passada (para comparação)
        var lastWeekStart = today.AddDays(-14);
        var lastWeekEnd = today.AddDays(-8);
        
        for (var date = lastWeekStart; date <= lastWeekEnd; date = date.AddDays(1))
        {
            int salesPerDay = date.DayOfWeek switch
            {
                DayOfWeek.Saturday or DayOfWeek.Sunday => random.Next(2, 5),
                _ => random.Next(5, 10)
            };

            for (int i = 0; i < salesPerDay; i++)
            {
                var product = products[random.Next(products.Length)];
                var amount = random.Next(50, 500);

                sales.Add(new Sale
                {
                    SellerId = sellerId,
                    ProductName = product,
                    Amount = amount,
                    SaleDate = date.AddHours(random.Next(8, 20)),
                    PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                    Status = "completed"
                });
            }
        }

        return sales;
    }
}


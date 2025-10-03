using KernelPagBank.Components;
using KernelPagBank.Data;
using KernelPagBank.Plugins;
using KernelPagBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Blazor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// Entity Framework Core com SQLite
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
    
    // Apenas em desenvolvimento
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Semantic Kernel
builder.Services.AddScoped<Kernel>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var configuration = builder.Configuration;
    
    // Obtém API key do Gemini
    var apiKey = configuration["Gemini:ApiKey"];
    
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException(
            "API Key do Gemini não configurada. " +
            "Crie o arquivo 'appsettings.Development.json' com a chave 'Gemini:ApiKey'. " +
            "Veja o arquivo 'appsettings.Development.json.example' para referência.");
    }

    // Cria o kernel
    var kernelBuilder = Kernel.CreateBuilder();
    
    // Configura o Google Gemini
    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        modelId: configuration["SemanticKernel:ModelId"] ?? "gemini-1.5-flash",
        apiKey: apiKey);

    // Adiciona plugins
    var salesDataService = sp.GetRequiredService<ISalesDataService>();
    kernelBuilder.Plugins.AddFromObject(new SalesPlugin(salesDataService), "SalesPlugin");

    // Configura logging
    kernelBuilder.Services.AddSingleton(loggerFactory);

    return kernelBuilder.Build();
});

// Application Services
builder.Services.AddScoped<ISalesDataService, SalesDataService>();
builder.Services.AddScoped<IChatService, ChatService>();

// HttpClient (para possíveis chamadas externas)
builder.Services.AddHttpClient();

var app = builder.Build();

// Inicializa o banco de dados com dados mockados
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using var context = await contextFactory.CreateDbContextAsync();
    await SeedData.InitializeAsync(context);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


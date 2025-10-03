using KernelPagBank.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Runtime.CompilerServices;

namespace KernelPagBank.Services;

/// <summary>
/// Implementação do serviço de chat usando Semantic Kernel
/// </summary>
public class ChatService : IChatService
{
    private readonly Kernel _kernel;
    private readonly ILogger<ChatService> _logger;
    private readonly ChatHistory _chatHistory;
    private readonly List<ChatMessage> _displayHistory;

    private const string SystemPrompt = @"Você é um assistente virtual do PagBank, especializado em ajudar vendedores a entenderem suas vendas.

Você  a funções que permitem consultar:
- Vendas em períodos específicos
- Estatísticas de vendas (total, faturamento, ticket médio)
- Produto mais vendido
- Comparações entre períodos

Responda sempre em português brasileiro de forma clara, objetiva e amigável.
Use emojis quando apropriado (📊, 💰, 📈, 📉, 🎯).
Quando mostrar valores monetários, use o formato R$ X.XXX,XX.
Seja proativo em sugerir análises relevantes.

Ao responder sobre períodos:
- ""semana passada"" = últimos 7 dias
- ""mês passado"" = últimos 30 dias
- ""hoje"" = dia atual
- ""ontem"" = dia anterior

Sempre que possível, forneça contexto e insights sobre os dados.";

    public ChatService(Kernel kernel, ILogger<ChatService> logger)
    {
        _kernel = kernel;
        _logger = logger;
        _chatHistory = new ChatHistory(SystemPrompt);
        _displayHistory = new List<ChatMessage>();
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(
        string message, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > 500)
        {
            yield return "❌ Por favor, envie uma mensagem válida (até 500 caracteres).";
            yield break;
        }

        // Adiciona mensagem do usuário ao histórico
        _chatHistory.AddUserMessage(message);
        _displayHistory.Add(new ChatMessage
        {
            Role = "user",
            Content = message,
            Timestamp = DateTime.UtcNow
        });

        // Variável para acumular a resposta
        var fullResponse = string.Empty;
        var hasError = false;
        
        // Stream com tratamento de erro separado
        await foreach (var chunk in StreamResponseWithErrorHandlingAsync(cancellationToken))
        {
            if (chunk.StartsWith("ERROR:"))
            {
                hasError = true;
                // Remove o prefixo "ERROR:"
                var errorMessage = chunk.Substring(6);
                
                _displayHistory.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = errorMessage,
                    Timestamp = DateTime.UtcNow,
                    IsError = true
                });
                
                yield return errorMessage;
            }
            else if (!string.IsNullOrEmpty(chunk))
            {
                fullResponse += chunk;
                yield return chunk;
            }
        }

        // Adiciona resposta completa ao histórico (apenas se não houve erro)
        if (!hasError && !string.IsNullOrEmpty(fullResponse))
        {
            _chatHistory.AddAssistantMessage(fullResponse);
            _displayHistory.Add(new ChatMessage
            {
                Role = "assistant",
                Content = fullResponse,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    private async IAsyncEnumerable<string> StreamResponseWithErrorHandlingAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Wrapper para capturar erros e transformá-los em mensagens
        IAsyncEnumerable<StreamingChatMessageContent>? streamingContent = null;
        
        var initError = InitializeStreamAsync();
        if (!string.IsNullOrEmpty(initError))
        {
            yield return initError;
            yield break;
        }

        // Stream da resposta
        if (streamingContent != null)
        {
            await foreach (var contentUpdate in streamingContent)
            {
                if (!string.IsNullOrEmpty(contentUpdate.Content))
                {
                    yield return contentUpdate.Content;
                }
            }
        }

        string? InitializeStreamAsync()
        {
            try
            {
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Configuração para habilitar invocação automática de funções/plugins
                var executionSettings = new GeminiPromptExecutionSettings
                {
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
                };

                streamingContent = chatCompletionService.GetStreamingChatMessageContentsAsync(
                    _chatHistory,
                    executionSettings: executionSettings,
                    kernel: _kernel,
                    cancellationToken: cancellationToken);

                return null; // Sem erro
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar serviço de chat");
                return "ERROR:❌ Erro ao inicializar o serviço de IA.";
            }
        }
    }

    public List<ChatMessage> GetChatHistory()
    {
        return _displayHistory.ToList();
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage(SystemPrompt);
        _displayHistory.Clear();
        _logger.LogInformation("Histórico de chat limpo");
    }
}


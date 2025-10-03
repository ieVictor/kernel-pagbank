using KernelPagBank.Models;

namespace KernelPagBank.Services;

/// <summary>
/// Interface para serviço de chat com IA
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Envia uma mensagem e obtém resposta da IA com streaming
    /// </summary>
    IAsyncEnumerable<string> SendMessageStreamAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém o histórico de mensagens da sessão atual
    /// </summary>
    List<ChatMessage> GetChatHistory();

    /// <summary>
    /// Limpa o histórico de chat
    /// </summary>
    void ClearHistory();
}


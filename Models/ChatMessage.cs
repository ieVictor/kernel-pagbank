namespace KernelPagBank.Models;

/// <summary>
/// Representa uma mensagem no chat entre usuário e assistente
/// </summary>
public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public bool IsError { get; set; }
}


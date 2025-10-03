using System.Text.RegularExpressions;

namespace KernelPagBank.Services;

/// <summary>
/// Implementação do serviço de segurança contra prompt injection
/// </summary>
public class PromptSecurityService : IPromptSecurityService
{
    private readonly ILogger<PromptSecurityService> _logger;

    // Padrões que indicam tentativa de prompt injection
    private static readonly string[] InjectionPatterns = new[]
    {
        // Tentativas de mudar o papel do assistente
        @"(?i)(você é|você agora é|seja|atue como|comporte-se como|finja ser|você deve ser|assuma o papel)",
        @"(?i)(you are|you are now|act as|behave as|pretend to be|you must be|assume the role)",
        
        // Tentativas de ignorar instruções anteriores
        @"(?i)(ignore|esqueça|desconsidere).{0,30}(instruções|instrução|prompt|sistema|regras|anterior)",
        @"(?i)(ignore|forget|disregard).{0,30}(instructions|instruction|prompt|system|rules|previous)",
        
        // Tentativas de revelar o prompt do sistema
        @"(?i)(mostre|exiba|revele|qual é|me diga).{0,30}(prompt|instruções do sistema|system prompt)",
        @"(?i)(show|display|reveal|what is|tell me).{0,30}(prompt|system instructions|system prompt)",
        
        // Tentativas de executar comandos
        @"(?i)(execute|rode|run|eval|system|shell|bash|cmd|powershell)",
        
        // Tentativas de roleplay não relacionado
        @"(?i)(chef|cozinheiro|médico|advogado|professor|engenheiro|cientista|psicólogo|terapeuta)",
        @"(?i)(doctor|lawyer|teacher|engineer|scientist|psychologist|therapist|cook)",
        
        // Tentativas de bypass
        @"(?i)(mas primeiro|but first|antes disso|before that|no entanto|however|na verdade|actually)",
        @"\[SYSTEM\]|\[INST\]|\[/INST\]|<\|system\|>|<\|user\|>|<\|assistant\|>",
    };

    // Palavras-chave relacionadas ao domínio de vendas
    private static readonly string[] SalesKeywords = new[]
    {
        "venda", "vendas", "vendi", "faturamento", "receita", "produto", "produtos",
        "semana", "mês", "dia", "hoje", "ontem", "período", "estatística", "estatísticas",
        "ticket médio", "total", "comparar", "comparação", "melhor", "pior",
        "quanto", "quantas", "valor", "valores", "dinheiro", "real", "reais",
        "cliente", "clientes", "pagamento", "pix", "crédito", "débito",
        "maquininha", "moderninha", "link de pagamento", "qr code",
        "sales", "revenue", "product", "week", "month", "day", "statistics"
    };

    public PromptSecurityService(ILogger<PromptSecurityService> logger)
    {
        _logger = logger;
    }

    public bool IsPromptSafe(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        // Verifica cada padrão de injection
        foreach (var pattern in InjectionPatterns)
        {
            try
            {
                if (Regex.IsMatch(message, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
                {
                    _logger.LogWarning(
                        "Tentativa de prompt injection detectada. Padrão: {Pattern}, Mensagem: {Message}",
                        pattern.Substring(0, Math.Min(50, pattern.Length)),
                        message.Substring(0, Math.Min(100, message.Length)));
                    return false;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                _logger.LogWarning("Timeout ao validar padrão de regex para: {Message}", message);
                return false;
            }
        }

        return true;
    }

    public bool IsRelevantToDomain(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        var messageLower = message.ToLowerInvariant();

        // Verifica se contém pelo menos uma palavra-chave de vendas
        var hasRelevantKeyword = SalesKeywords.Any(keyword => 
            messageLower.Contains(keyword.ToLowerInvariant()));

        // Aceita perguntas genéricas curtas (saudações, etc)
        var isShortGenericQuestion = message.Length < 50 && 
            (messageLower.Contains("olá") || 
             messageLower.Contains("oi") || 
             messageLower.Contains("ajuda") ||
             messageLower.Contains("help") ||
             messageLower.Contains("o que") ||
             messageLower.Contains("what") ||
             messageLower.Contains("como") ||
             messageLower.Contains("how"));

        if (!hasRelevantKeyword && !isShortGenericQuestion)
        {
            _logger.LogInformation(
                "Mensagem fora do domínio de vendas detectada: {Message}",
                message.Substring(0, Math.Min(100, message.Length)));
            return false;
        }

        return true;
    }
}


namespace KernelPagBank.Services;

/// <summary>
/// Interface para serviço de segurança de prompts
/// </summary>
public interface IPromptSecurityService
{
    /// <summary>
    /// Valida se a mensagem do usuário contém tentativas de prompt injection
    /// </summary>
    /// <param name="message">Mensagem do usuário</param>
    /// <returns>True se a mensagem é segura, False se contém tentativa de injection</returns>
    bool IsPromptSafe(string message);

    /// <summary>
    /// Verifica se a mensagem está relacionada ao domínio de vendas
    /// </summary>
    /// <param name="message">Mensagem do usuário</param>
    /// <returns>True se está relacionada a vendas, False caso contrário</returns>
    bool IsRelevantToDomain(string message);
}


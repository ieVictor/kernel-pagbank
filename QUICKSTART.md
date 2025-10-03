# 🚀 Guia Rápido de Configuração

## Passos para rodar o projeto em 3 minutos

### 1️⃣ Obtenha sua API Key do Gemini

1. Acesse: https://aistudio.google.com/app/apikey
2. Faça login com sua conta Google
3. Clique em "Create API Key"
4. Copie a chave gerada

### 2️⃣ Configure o projeto

Crie o arquivo `appsettings.Development.json` na raiz do projeto:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning",
            "Microsoft.SemanticKernel": "Information"
        }
    },
    "Gemini": {
        "ApiKey": "COLE_SUA_API_KEY_AQUI"
    }
}
```

### 3️⃣ Execute

```bash
dotnet restore
dotnet run
```

Pronto! Acesse: `https://localhost:5001`

## 🎯 Teste o assistente

Perguntas sugeridas para testar:

1. "Quantas vendas fiz na semana passada?"
2. "Minhas vendas melhoraram ou pioraram?"
3. "Qual meu produto mais vendido?"
4. "Quanto faturei no mês?"
5. "Compare minhas vendas de hoje com ontem"

## ⚠️ Problemas Comuns

### Erro: "API Key do Gemini não configurada"

-   Certifique-se de criar o arquivo `appsettings.Development.json`
-   Verifique se a chave está correta

### Erro ao conectar com a API

-   Verifique sua conexão com a internet
-   Confirme se a API Key está válida
-   Verifique se não excedeu o limite de requisições gratuitas

### Banco de dados não inicializa

-   Delete a pasta `Data/` e execute novamente
-   O banco será recriado automaticamente com dados mockados

## 📊 Sobre os Dados

O projeto cria automaticamente um banco SQLite com:

-   1 vendedor fictício (João Silva)
-   ~150 vendas dos últimos 30 dias
-   5 produtos diferentes
-   Valores e datas realistas

## 🎨 Apresentação

Para demonstrar o projeto em 3 minutos:

1. **[30s]** Mostre a interface do chat
2. **[90s]** Faça 3-4 perguntas ao assistente
3. **[60s]** Explique a arquitetura (Blazor + Semantic Kernel + Gemini)

Foque na **facilidade de uso** e na **inteligência das respostas**!

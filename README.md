# 🤖 PagBank - Assistente de Vendas com Semantic Kernel

Aplicação Blazor que utiliza **Microsoft Semantic Kernel** e **Google Gemini** para criar um assistente inteligente que ajuda vendedores do PagBank a analisarem suas vendas.

## 🎯 Funcionalidades

O assistente pode responder perguntas como:

- ✅ "Quantas vendas fiz na semana passada?"
- ✅ "Minhas vendas melhoraram ou pioraram?"
- ✅ "Qual meu produto mais vendido?"
- ✅ "Quanto faturei no mês?"
- ✅ "Compare minhas vendas de hoje com ontem"

## 🏗️ Arquitetura

- **Frontend:** Blazor Server com MudBlazor
- **Backend:** ASP.NET Core 8.0
- **IA:** Microsoft Semantic Kernel + Google Gemini
- **Banco de Dados:** SQLite (dados mockados)

## 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- API Key do Google Gemini ([obtenha aqui](https://aistudio.google.com/app/apikey))

## 🚀 Como Executar

### 1. Clone o repositório ou navegue até a pasta do projeto

```bash
cd kernel-pagbank
```

### 2. Configure a API Key do Gemini

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
    "ApiKey": "SUA_API_KEY_AQUI"
  }
}
```

> ⚠️ **IMPORTANTE:** Nunca commite este arquivo! Ele já está no `.gitignore`.

### 3. Restaure as dependências

```bash
dotnet restore
```

### 4. Execute a aplicação

```bash
dotnet run
```

A aplicação estará disponível em:

- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## 📁 Estrutura do Projeto

```
kernel-pagbank/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor          # Layout principal
│   └── Pages/
│       └── Home.razor                # Página do chat
│
├── Services/
│   ├── ChatService.cs                # Orquestração do Semantic Kernel
│   └── SalesDataService.cs           # Acesso a dados de vendas
│
├── Plugins/
│   └── SalesPlugin.cs                # Plugin SK com funções de vendas
│
├── Models/
│   ├── Sale.cs                       # Modelo de venda
│   ├── Seller.cs                     # Modelo de vendedor
│   ├── ChatMessage.cs                # Mensagem do chat
│   └── SalesStatistics.cs            # Estatísticas agregadas
│
├── Data/
│   ├── ApplicationDbContext.cs       # Contexto EF Core
│   └── SeedData.cs                   # Dados mockados
│
└── Program.cs                        # Configuração da aplicação
```

## 🔐 Segurança

- ✅ API Keys armazenadas em `appsettings.Development.json` (gitignored)
- ✅ Input validation em todos os pontos de entrada
- ✅ Parameterized queries (EF Core)
- ✅ Error handling sem exposição de detalhes internos
- ✅ Rate limiting configurável

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core 8.0** - Framework web
- **Blazor Server** - UI interativa com C#
- **Microsoft Semantic Kernel** - Orquestração de IA
- **Google Gemini** - Modelo de linguagem
- **MudBlazor** - Biblioteca de componentes UI
- **Entity Framework Core** - ORM
- **SQLite** - Banco de dados

## 📊 Dados Mockados

O projeto inclui dados de vendas mockados para demonstração:

- 1 vendedor (João Silva)
- ~150 vendas dos últimos 30 dias
- 5 produtos diferentes (Maquininhas Moderninha)
- Distribuição realista por dia da semana

## 🎨 Interface

- Design moderno com MudBlazor
- Cores e identidade visual do PagBank
- Chat responsivo com streaming de respostas
- Sugestões de perguntas rápidas
- Animações suaves

## 🤝 Boas Práticas Implementadas

- ✅ **Clean Architecture** - Separação de responsabilidades
- ✅ **Dependency Injection** - Inversão de controle
- ✅ **Interface-based programming** - Testabilidade
- ✅ **Async/await** - Programação assíncrona correta
- ✅ **Error handling** - Tratamento robusto de erros
- ✅ **Logging estruturado** - Rastreabilidade
- ✅ **Nullable reference types** - Segurança de tipos
- ✅ **Repository pattern** - Acesso a dados abstraído

## 📝 Próximos Passos (Fora do Escopo)

- [ ] Autenticação e autorização
- [ ] Múltiplos vendedores
- [ ] Histórico de conversas persistido
- [ ] Exportação de relatórios
- [ ] Gráficos e dashboards
- [ ] Integração com API real do PagBank
- [ ] Testes unitários e de integração

## 📄 Licença

Este é um projeto de demonstração para fins educacionais.

## 👨‍💻 Autor

Projeto desenvolvido para demonstrar a integração entre **Blazor**, **Semantic Kernel** e **Google Gemini**.

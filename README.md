# MinhaApiCQRS 🚀

API desenvolvida em .NET 10 para gerenciamento de funcionários, utilizando **Clean Architecture** e o padrão **CQRS**.

## 🛠️ Tecnologias e Padrões

- **ASP.NET Core 10**
- **Entity Framework Core** (PostgreSQL)
- **CQRS** (Command Query Responsibility Segregation)
- **Mediator Pattern** (MediatR)
- **FluentValidation** (Validação de entrada)
- **Global Exception Handling** (Middleware personalizado)
- **Repository & Unit of Work Patterns**

## 🏗️ Estrutura do Projeto

- `Domain`: Entidades, Exceções de negócio e Interfaces de Repositório.
- `Application`: Casos de uso (Handlers), Commands, Queries e Validadores.
- `Infrastructure`: Contexto do banco de dados, Migrations e Implementação dos Repositórios.
- `API`: Controllers, Middlewares e Configurações de Injeção de Dependência.
- `Photo`: Projeto especializado no processamento de imagens e armazenamento das fotos dos funcionários.

## 🚀 Como Rodar

1. Configure a Connection String no `appsettings.json` (use o `appsettings.Example.json` como base).
2. Execute as migrations para criar o banco:
   ```bash
   dotnet ef database update -s API -p Infrastructure
   Inicie a aplicação:
   ```

Bash
dotnet run --project API
🛡️ Exception Handling
A API utiliza um Middleware global que captura exceções de domínio e retorna códigos HTTP semânticos (400 para erros de validação, 404 para itens não encontrados), mantendo o Swagger limpo e padronizado.

---

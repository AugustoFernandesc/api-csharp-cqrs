# Minha API CQRS - Gestão de Funcionários & Automação

Este projeto é uma API desenvolvida em **.NET 10**, seguindo o padrão arquitetural **CQRS (Command Query Responsibility Segregation)**. O sistema gerencia funcionários e possui um fluxo de automação completo para geração e envio de relatórios via e-mail.

## 🚀 Funcionalidades Principais

- **CRUD de Funcionários:** Gestão completa (Criar, Ler, Atualizar e Deletar).
- **Upload de Fotos:** Serviço integrado para armazenamento e recuperação de imagens.
- **Automação de Relatórios (Scheduler):** \* Um **Background Service** monitora o banco de dados.
  - Filtra funcionários com foto que ainda não receberam o relatório.
  - Gera PDFs customizados dinamicamente.
  - Dispara e-mails com o documento em anexo.
- **Tratamento Global de Exceções:** Middleware customizado para respostas de erro padronizadas.

## 🏗️ Arquitetura e Tecnologias

A solução foi dividida em múltiplos projetos para garantir o desacoplamento e a testabilidade (Clean Architecture):

- **API:** Ponto de entrada, Controllers e Middlewares.
- **Application:** Lógica de negócio, Interfaces e Handlers (MediatR).
- **Domain:** Entidades, Exceções de domínio e regras de negócio.
- **Infrastructure:** Persistência de dados (Entity Framework Core) e Repositórios.
- **PDF:** Geração de documentos utilizando a biblioteca **QuestPDF**.
- **Email:** Serviço de disparo de e-mails utilizando **MailKit**.
- **Scheduler:** Robô de processamento em background (**BackgroundService**).

### Stack Técnica

- **Linguagem:** C# (.NET 10)
- **Banco de Dados:** PostgreSQL
- **Padrões:** CQRS, Unit of Work, Repository Pattern, Dependency Injection.
- **Bibliotecas:** MediatR, QuestPDF, MailKit, EF Core.

## 🤖 Como funciona o Scheduler?

O Scheduler opera como um serviço hospedeiro (_Hosted Service_) que roda em segundo plano. Ele utiliza um `IServiceScopeFactory` para acessar os serviços de banco de dados de forma segura, garantindo que:

1.  O processo seja **Idempotente**: Cada funcionário recebe o relatório apenas uma vez, controlado pela flag `IsEmailSent`.
2.  O processo seja **Resiliente**: Falhas no envio de um e-mail não interrompem o processamento dos demais.

## 🛠️ Como executar

1.  Configure a string de conexão do PostgreSQL no `appsettings.json`.
2.  Configure suas credenciais de SMTP (ex: Mailtrap ou Gmail App Password) no serviço de Email.
3.  Execute as migrações do banco de dados:
    ```bash
    dotnet ef database update --project Infrastructure --startup-project API
    ```
4.  Rode a aplicação:
    ```bash
    dotnet run --project API
    ```

---

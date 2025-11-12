# Shora Work Manager

## Descrição do Projeto

**Shora Work Manager** é uma aplicação web desenvolvida para o gerenciamento de obras e materiais, utilizando tecnologias como **ASP.NET MVC**, **SQL Server**, **Entity Framework Core (EF Core)** e o padrão **MediatR**. O sistema foi projetado para agilizar a gestão de clientes, obras, materiais, movimentação de estoque, mão de obra e pagamentos. A solução implementa uma arquitetura em camadas com base na **Clean Architecture**, influenciada por **Domain-Driven Design (DDD)**, permitindo a separação clara de responsabilidades.

## Funcionalidades

### Funcionais:
- CRUD para **Clientes**, **Materiais**, **Obras**.
- Gestão de dados gerais de cada obra, incluindo:
  - **Movimentos de Material** (entrada e saída de estoque).
  - **Registo de Mão de Obra**.
  - **Registo de Pagamentos**.
- Controle de **movimentos de estoque** por obra.
  
### Não Funcionais:
- Implementado com **ASP.NET MVC**, **SQL Server** e **Entity Framework Core**.
- Validações e registros básicos de logs.
- Interface de usuário com navegação através de **tabs**.

## Tecnologias Usadas

- **SQL Server**: Banco de dados relacional com suporte a T-SQL e ACID.
- **ASP.NET MVC**: Framework para aplicações web, com suporte a REST APIs e autenticação.
- **MediatR**: Implementação do padrão **Mediator** para desacoplamento entre controladores e a lógica de negócios, garantindo maior testabilidade e separação de responsabilidades.

## Arquitetura

O projeto segue uma arquitetura **Clean Architecture** com as seguintes camadas:

1. **Domain**: Contém as regras de negócio e validações.
2. **Application**: Gerencia os casos de uso e a orquestração dos serviços, utilizando **DTOs** e **Handlers** (via **MediatR**).
3. **Persistence**: Lida com a persistência de dados, utilizando **Entity Framework Core** para mapeamento de objetos para o banco de dados.
4. **ShoraWorkManager**: Camada de apresentação (ASP.NET MVC), que interage com a **Application Layer** e disponibiliza os dados ao frontend.


### Requisitos
- **SQL Server** instalado e configurado.

### Passos:
1. Clone o repositório.
2. Abra o projeto no **Visual Studio**.
3. Execute o comando para aplicar as migrações no banco de dados:

    ```bash
    update-database
    ```

4. Se necessário, configure a string de conexão no arquivo **appsettings.json** para apontar para o seu servidor de banco de dados.
5. Adicione a conta de **Admin** padrão nos **Secrets** do projeto:

    No seu arquivo `appsettings.json` ou no **Secrets** do .NET, adicione as seguintes chaves:

    ```json
    {
      "Account:AdminPassword": "password", 
      "Account:AdminEmail": "email"
    }
    ```

6. Execute a aplicação e acesse-a via navegador.

## Estrutura do Banco de Dados

O sistema utiliza um banco de dados relacional com as seguintes entidades:

- **Client**: Armazena informações sobre os clientes.
- **ConstructionSite**: Contém dados sobre as obras.
- **Material**: Detalhes sobre materiais utilizados nas obras.
- **MaterialMovement**: Movimentações de material por obra.
- **Worker**: Informações sobre trabalhadores.
- **ConstructionSiteWorkedHoursWorker**: Registra horas trabalhadas por trabalhador em cada obra.
- **Payment**: Detalhes dos pagamentos feitos.
- **User**: Usuários do sistema (baseado no **IdentityUser**).
- **AuthorizationToken**: Tokens de autorização utilizados no sistem para registo de novos utilizadores

## Considerações de Segurança

A aplicação implementa boas práticas de segurança, como:
- **Validações** de entrada de dados com **DataAnnotations**.
- **Anti-forgery tokens** para prevenir ataques CSRF.
- **Autenticação e Autorização** com base em **ASP.NET Identity** e **JWT tokens**.
- **Sanitização** de entradas para evitar vulnerabilidades de injeção de código.

## Conclusão

O **Shora Work Manager** oferece uma solução eficaz para gerenciar projetos de obras, com um foco em eficiência na gestão de recursos e segurança dos dados. A aplicação está estruturada para ser escalável e fácil de manter, com uma arquitetura clara e modular que permite a adição de novas funcionalidades de forma simples.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

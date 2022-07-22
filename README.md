# Desafio Fsharp

## Tecnologias Utilizadas
Foi desenvolvido utilizando F# com o framework *Giraffe* por ser mais idiomático para o estilo funcional da linguagem.
O banco de dados escolhido foi o PostgreSQL utilizando a biblioteca *NpgSQL* para conexão e execução de queries no banco.
Na camada de http foi utilizado o Thoth.json para validar jsons de entidades, retornando erro de validação caso o payload esteja em um formato incorreto.

## Estrutura
Modules/[Module]/
    -> Module
        -> ModuleHandlers.fs => realiza a comunicação HTTP
        -> ModuleService.fs => realiza a comunicação com o banco de dados
        -> ModuleValidator.fs => valida os payloads no formato de json vindo dos handlers
        -> ModuleRoutes.fs => registra as rotas do módulo
App.fs -> Registra todas as rotas da aplicação
Program.fs -> Configuração do giraffe
.devcontainer -> configurações do ambiente dockerizado

## Documentação
A documentação de cada endpoint está na collection do postman no path */docs/Desafio.postman_collection.json*, com todos os endpoints e payloads necessários para testar a API.

## Como rodar o projeto

Necessário ter o vscode com a extensão Remote containers para aproveitar o ambiente dockerizado com o devcontainer que já está configurado no projeto com o postgres e dotnet, sem precisar de mais dependências locais.

basta executar Ctrl + Shift + P -> Open folder in container, ao abrir o vscode em container executar o arquivo postgres.tables.sql para criar as tabelas utilizadas pelo sistema.
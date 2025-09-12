🏍️ Sistema de Aluguel de Motos e Entregas
📌 Descrição

Este projeto é um sistema para gerenciamento de motos disponíveis para aluguel e entregadores que realizam entregas. Permite o cadastro de motos, a locação por entregadores e o gerenciamento das entregas realizadas.

⚙️ Tecnologias Utilizadas

Backend: .NET 8.0

Banco de Dados: PostgreSQL (via Docker)

Armazenamento de Arquivos: AWS S3

Autenticação: Keycloak

🚀 Como Rodar o Projeto Localmente

Clonar o repositório:

git clone https://github.com/ferriello11/motorcycle-system.git
cd motorcycle-system

Executar Docker para o banco de dados:

docker-compose up -d

Rodar o projeto:

dotnet run

⚠️ O appsettings.json já contém todas as configurações necessárias, incluindo conexão com o banco, AWS S3 e Keycloak. (DEIXEI APENAS PARA O TESTE POIS SERA NECESSARIO TER ACESSOS A FILA E S3)

ℹ️ Estamos utilizando db.Database.EnsureCreated(), então as tabelas serão criadas automaticamente ao iniciar o projeto. Para alterações futuras nas entidades

📡 Endpoints Principais da API

API disponível em https://localhost:5001/api:

POST /users/register – Cadastrar novo usuário (entregador e admin)

POST /motorcycles – Cadastrar nova moto

POST /rentals – Criar nova locação

DELETE /motorcycles/{plate} – Deletar moto por placa

POST /deliveries – Criar nova entrega

🐳 Docker

Arquivo docker-compose.yml 

docker-compose up --build

http://localhost:5000/swagger/index.html




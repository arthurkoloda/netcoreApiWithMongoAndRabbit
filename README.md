# netcoreApiWithMongoAndRabbit
.Net Core 8 API com armazenamento de dados no MongoDB e gerenciamento de filas por RabbitMQ

Primeiramente, verificar se estão instalados os 3 pré-requisitos abaixo:

Visual Studio 2022 - IDE para desenvolver e depurar o código
.Net 8 SDK - Biblioteca mais recente do .Net Core
Docker - Utilizado para conteinerização dos recursos, como o MongoDB e o RabbitMQ

Configurando os contâiners do MongoDB e do RabbitMQ:

O Docker deve estar sendo executado na máquina
Abrir 2 instâncias do Prompt de Comando (cmd), e digitar os dois comandos abaixo, um em cada instância:

docker run --rm  -it -p 15672:15672 -p 5672:5672 rabbitmq:3-management
docker run -d -p 27017:27017 --name mongodb mongo


Executando os projetos via Visual Studio:
Ao abrir a solução com o Visual Studio 2022, temos 2 aplicativos executáveis:
WebAPI - API onde serão importados os arquivos de Excel e também listados
Consumer - Aplicativo de console responsável por processar a fila de documentos do RabbitMQ
Os dois aplicativos não precisam necessariamente estar sendo executados em conjunto, mas é possível abrí-los em duas instâncias e deixar a fila prosseguindo enquanto faz a importação de arquivos, neste meio tempo pode-se observar também o método GET que retorna os documentos e seus status, para acompanhar quais estão sendo processados


Envio de e-mail com relatório de documentos processados:
O código para enviar o e-mail está escrito, porém para utilizá-lo é necessário informar dados válidos para o SMTP, Username e Password e descomentar o trecho de código da linha 124 do RelatorioService.cs

Divisão do projeto:
1 - WEBApi - API do projeto, ela está utilizando autenticação Bearer para login, que pode ser obtida pelo método /Seguranca/Login. O Swagger foi implementado para facilitar os testes
2 - Domain - Neste projeto estão as regras de negócio e as filas do RabbitMQ
3 - Infrastructure - Neste projeto está configurada a base de dados, bem como os repositórios e entidades
4 - Test - Testes unitários da aplicação
5 - Consumer - Processos executados de tempos em tempos para leitura das filas do RabbitMQ e também envio do relatório diário

Credenciais para login da API: 
Login: arthur
Senha: testeApi
# Execução do sistema
A maneira mais fácil de executar o sistema localmente é utilizando a ferramenta Docker. É disponibilizado um arquivo pré-configurado
com as imagens dependentes que o sistema precisa juntamente com uma imagem pronta para execução do sistema através do comando:

**docker-compose -f ymls\docker-compose.yml up**

Para executar o sistema no Kubernetes é disponibilizado os seguintes arquivos de configuração:

**kubectl apply -f yamls\kubernetes-api.yaml --record** 

**kubectl apply -f yamls\kubernetes-jobs.yaml --record** 

*Obs: verificar a execução dos serviços dependentes e os enderenços de configuração nas variaveis de ambiente.*

*docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' ravendb elasticsearch apm-server*

# Dependências
Para o funcionamento do sistema é necessário que os seguintes serviços estejam em execução:

1. RavenDb
2. Elastic Search
3. Kibana
4. APM Server

É disponibilizado um arquivo pré-configurado para execução de cada dependencia utilizando a ferramente Docker através dos comandos:

**docker-compose -f ymls\dc-raven.yml up -d**

**docker-compose -f ymls\dc-elastic.yml up -d**

**docker-compose -f ymls\dc-api.yml up -d**

**docker-compose -f ymls\dc-jobs.yml up -d**

# Acessos

Api: http://localhost:5000
RavenDb: http://localhost:8080
Kibana: http://localhost:5601

## Arquitetura Proposta:
Plataforma:

Foi escolhido a stack .Net Core como base de desenvolvido pelas seguintes razões:

1. Domínio e conhecimento da plataforma e linguagens de programação suportadas como C# e F#.
2. Linguagem de programação orientada a objetos ou funcional fortemente tipada.
3. Flexibilidade de distribuição com possibilidade de execução em ambientes windows ou linux.

Distribuição:

1. Possibilidade de distribuir o sistema utilizando containers com Docker.
2. Possibilidade de distribuir o sistema utilizando orquestradores com Kubernetes.
3. Possibilidade de distribuir o sistema de maneira tradicional com IIS ou Windows Service.
4. Possibilidade de execução do ambiente local utilizando Docker-Compose.

Design:

O design proposto prioriza a  escalabilidade, disponibilidade, performance e tolerancia a falhas tendo como base o modelo de arquitetura em 12 fatores.

	- Configuração: Configuração realizada através de variaveis de ambiente.
	- Logs: Logs realizados de maneira distribuida utilizando a stack APM Server + Elastic Search + Kibana.
	- Monitoramento: Monitoramento realizado de maneira distribuida utilizando a stack APM Server + Elastic Search + Kibana.
	- Resiliência: Recuperação a falhas gerenciada por configuração através de orquestradores como Kubernetes.
	- Versionamento: Apis utilizando controle de versão com possibilidade de multiplas versões executando em paralelo.
	- Sem Estado: Sistema não armazena nenhum estado possibilitando o escalamento horizontal.
	- Reativo: A aquitetura do sistema é orientada a eventos possibilitando a execução e integração entre outros sistemas.
	- Indepêndecia de Execução: O sistema foi projetado de forma que cada processo possa ser distribuido de forma separada e concorrente.
	
Teorema de CAP (AP): 

	- O Sistema prioriza a disponibilidade e resiliência a falhas de partições sob a consistência imediata.
	- Consistencia entre entidades é realizada de maneira eventual assíncrona.
	
Concorrencia:

	- Concorrencia otimista por entidade sem uso de locks para diminuir a contenção.
	- Em cada transação é modificada somente uma entidade para possibilitar o processamento paralelo e dimuição de conflitos de acesso.
	
Persistência:	

	- Banco de dados NoSql orientado a documentos.
	- Capacidade de distribuição horizontal sem pontos unicos de falha
	- Flexibilidade na definição e manutenção dos esquemas
	- Utilizado o padrão CQRS e Event Sourcing para otimizar a leitura e gravação do sistema de forma indepentende.
	- Estado é armazenado em forma de eventos 'append only' garantindo a performance na escrita e auditoria de operações sem descarte de dados.
	- Consultas são derivativas dos eventos com capacidade de criação dinamica e auto organização.
	- Consultas são indepentendes e realizadas utilizando Map Reduce garantindo a performance na leitura.
	
Integração:

	- Sistema pode ser integrado de maneira sincrona utilizando protocolos HTTP através de chamadas a API REST.
	- Sistema pode ser integrado de maneira assíncrona utilizando notificação de eventos através de  mensageria (Kafka, RabbitMQ, etc).
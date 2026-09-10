# 🌿KaáDebug APP Mobile

O alicativo móvel **KaáDebug**, foi desenvolvido para auxiliar no monitoramento e cuidado de plantas por meio da integração entre dispositivos IoT, sensores, serviços de backend e recursos de inteligência artificial.

O aplicativo foi desenvolvido utilizando **.NET MAUI**, permitindo a execução da aplicação em dispositivos móveis Android e mantendo uma estrutura preparada para outros ambientes suportados pelo framework.

---

## 📱 Visão Geral

O aplicativo atua como a interface de interação entre o usuário e o sistema de monitoramento de plantas. Por meio dele, o usuário pode:

- Realizar cadastro e autenticação;
- Gerenciar seu perfil;
- Cadastrar e acompanhar plantas;
- Associar dispositivos ESP32 às plantas;
- Visualizar dados de sensores;
- Acompanhar o estado de saúde das plantas;
- Receber notificações e alertas;
- Consultar informações históricas;
- Receber recomendações relacionadas aos cuidados da planta;
- Enviar imagens para análise por inteligência artificial.

A comunicação com o backend é realizada por meio de uma API no modelo **Backend For Frontend (BFF)**.

---

## 🏗️ Arquitetura e Padrões

O aplicativo utiliza uma arquitetura baseada na separação entre:

```text
┌──────────────────────────────┐      ┌──────────────────────────────┐  
│          Aplicativo          │      │            BFF API           │
│          .NET MAUI           │      ├──────────────────────────────┤
├──────────────────────────────┤      │ Usuários                     │
│ Views                        │----> │ Plantas                      │ 
├──────────────────────────────┤      │ Dispositivos\Sensores        │
│ Services                     │      │ Notificações                 │
├──────────────────────────────┤      │ Diagnósticos\IA              │
│ ApiClient / DTOs             │      │                              │
└──────────────────────────────┘      └──────────────────────────────┘
```
 
 🔙 **BFF - Backend For Frontend**

 O aplicativo utiliza uma API BFF como intermediária entre o cliente móvel e os serviços de backend. O objetivo é reduzir a quantidade de requisições necessárias para montar uma tela e adaptar os dados às necessidades específicas do aplicativo.

Por exemplo, para a tela principal que busca ser um resumo geral, trazendo infromações de plantas do usuários, notificações, ultimos status, em vez de realizar diversas requisições independentes para obter informações, com BFF é possível consolidar essas informações em uma única resposta:


| Sem BFF             | Com BFF       |
| --------------------|:-------------:|
| GET /user           | GET /dashboard|
| GET /plants         |
| GET /notifications  |
| GET /sensor-readings|
  
💡 Isso reduz o número de operações de rede e consequentemente, o consumo de recursos do dispositivo.

--- 
🔄 **Camada Anticorrupção**
 
Para isolar o aplicativo de mudanças repentinas na API e garantir a segurança do código, utilizamos uma **Camada Anticorrupção (ACL)** por meio de serviços, que permitem isolamento, se a api mudar isso não quebrar a exibição das telas que por sua vez não precisam conhecer as regras de negócio, esse processo funciona da seguinte forma: 

- **Recebimento (DTO):** Os dados brutos chegam da API mapeados em DTOs (*Data Transfer Objects*).
- **Conversão (Mappers):** Transformamos esses dados em modelos internos com tipos seguros (ex: *Enums*).
- **Exibição (View):** As telas recebem apenas os modelos limpos, sem contato direto com o contrato da API.

**Exemplo prático:**

API (`"CRITICAL"`) ➔ DTO ➔ Conversão ➔ Modelo (`PlantHealthStatus.Critical`) ➔ Tela

---
🧩 **Injeção de Dependências**

A aplicação utiliza o mecanismo nativo de Injeção de Dependências disponibilizado pelo .NET para promover o desacoplamento entre as classes e facilitar a manutenção do código. Toda a configuração dos serviços está centralizada no arquivo:MauiProgram.cs. Ele atua como o Composition Root da aplicação. Ele é o único ponto responsável por registrar os serviços, páginas (Views), além de gerenciar o ciclo de vida de cada componente.

No ciclo de vida, o contêiner do .NET gerencia como as instâncias são criadas e compartilhadas. Abaixo estão as dependências registradas e seus respectivos escopos:

| Componentes e Objetivos | Ciclo de vida |
| ------------- |:-------------:|
| **ApiClient** <br> Reutilização da infraestrutura HTTP | Singleton |
| **IPlantsCatalogService** <br> Compartilhamento do catálogo | Singleton |
| **IProfileService** <br> Gerenciamento do estado relacionado ao perfil | Singleton |
| **ILoginService** <br> Instâncias independentes para cada fluxo | Transient |
| **IDashboardService** <br> Estado específico da operação | Transient |
| **Views** <br> Instâncias independentes das telas | Transient |
| **ViewModels** <br> Estado isolado de cada tela | Transient |

--- 

**Padrões de Projeto (Design Patterns)**

- Dependency Injection (DI): Utilização do contêiner nativo de injeção de dependências do .NET para desacoplamento entre componentes.
- Service Pattern: Centralização de operações de negócio e comunicação externa em Services específicos.
- DTO Pattern: Utilização de contratos específicos para comunicação com a BFF API.
- Result Pattern: Encapsulamento dos resultados das operações HTTP, permitindo tratar erros de rede e respostas inválidas sem interromper o fluxo da aplicação.
- Repository/Abstraction de comunicação: Acesso HTTP centralizado através do ApiClient, evitando que as Views realizem diretamente chamadas à API.
- Value Converter: Conversão de informações de domínio para propriedades utilizadas na apresentação da interface.

---
### 💻 Tecnologias Utilizadas
- Linguagem: C#
- Framework: .NET 9 / .NET MAUI
- Interface: XAML / MAUI
- Arquitetura de comunicação: REST / HTTP
- Backend: KaáDebug BFF API
- Autenticação: JWT (JSON Web Tokens)
- Armazenamento seguro: SecureStorage
- IoT: ESP32
- Inteligência Artificial: Serviço de diagnóstico por imagem
- Gerenciamento de dependências: Microsoft.Extensions.DependencyInjection
- Processamento de dados: LINQ

---
### 📂 Estrutura do Projeto
A solução está organizada em camadas, buscando separar as responsabilidades de apresentação, regras da aplicação, contratos e infraestrutura de comunicação.

```text
KaaDebug/
│
├── Core/
│   ├── Interfaces/
│   │   └── Contratos e intermediação entre telas e serviços
│   │
│   └── Models/
│       └── Modelos de domínio utilizados pela aplicação
│
├── Converters/
│   └── Conversores valores em modelos utilizados pela interface
│
├── Services/
│   └── Implementa os serviços e realiza a comunicação com a API 
│
├── Views/
│   └── Interfaces e páginas do aplicativo
│
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs
```
---
### 📡 Comunicação com a API
Toda comunicação externa do aplicativo é centralizada através da classe:
```
ApiClient
```
O ApiClient funciona como um wrapper sobre o HttpClient, concentrando as operações de comunicação HTTP. Entre suas responsabilidades estão:
- Execução de requisições HTTP;
- Serialização e desserialização de objetos;
- Tratamento de códigos HTTP;
- Controle de timeout;
- Tratamento de erros de rede;
- Extração de mensagens retornadas pela API;
- Padronização dos resultados das requisições.
# Design Document: Event Calendar

## Overview

O Event Calendar é um aplicativo desktop em C# que permite aos usuários gerenciar eventos com notificações visuais automáticas. O sistema utiliza Windows Forms para a interface gráfica e um serviço de background para monitoramento contínuo de eventos.

A arquitetura segue o padrão Model-View-Controller (MVC) adaptado para Windows Forms, com separação clara entre lógica de negócio, persistência de dados e interface do usuário.

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ MainForm     │  │ AddEventForm │  │ NotificationModal│
│  │ (Calendar UI)│  │              │  │ (com botão OK)│  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    Business Logic Layer                  │
│  ┌──────────────────────────────────────────────────┐   │
│  │           EventManager                           │   │
│  │  - AddEvent()                                    │   │
│  │  - RemoveEvent()                                 │   │
│  │  - GetEvents()                                   │   │
│  │  - GetEventsByDate()                             │   │
│  └──────────────────────────────────────────────────┘   │
│                          │                               │
│  ┌──────────────────────────────────────────────────┐   │
│  │      EventNotificationService                    │   │
│  │  - CheckForDueEvents()                           │   │
│  │  - TriggerNotification()                         │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    Data Layer                            │
│  ┌──────────────────────────────────────────────────┐   │
│  │           EventRepository                        │   │
│  │  - Save()                                        │   │
│  │  - Load()                                        │   │
│  │  - Add()                                         │   │
│  │  - Remove()                                      │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### Event Model

```csharp
public class Event
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime EventDateTime { get; set; }
    public bool IsNotified { get; set; }
}
```

### EventManager

Gerencia todas as operações relacionadas a eventos.

```csharp
public interface IEventManager
{
    void AddEvent(string description, DateTime eventDateTime);
    void RemoveEvent(Guid eventId);
    List<Event> GetAllEvents();
    List<Event> GetEventsByDate(DateTime date);
    Event GetEventById(Guid eventId);
}
```

### EventRepository

Responsável pela persistência de dados usando JSON.

```csharp
public interface IEventRepository
{
    void Save(List<Event> events);
    List<Event> Load();
    void Add(Event eventItem);
    void Remove(Guid eventId);
}
```

### EventNotificationService

Serviço de background que monitora eventos e dispara notificações.

```csharp
public interface IEventNotificationService
{
    void Start();
    void Stop();
    event EventHandler<Event> EventDue;
}
```

### NotificationModal

Modal de notificação que exige confirmação do usuário.

```csharp
public class NotificationModal : Form
{
    // Exibe a descrição do evento
    // Contém um botão "OK" ou "Confirmar"
    // Só fecha quando o usuário clica no botão
    // Exibido no centro da tela
}
```

## Data Models

### Event

- **Id**: Identificador único do evento (Guid)
- **Description**: Descrição textual do evento (string, não vazio)
- **EventDateTime**: Data e hora do evento (DateTime, deve ser futuro ao criar)
- **IsNotified**: Flag indicando se o evento já foi notificado (bool)

### Storage Format

Os eventos são armazenados em formato JSON no arquivo `events.json` no diretório local do aplicativo:

```json
[
  {
    "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "Description": "Reunião com cliente",
    "EventDateTime": "2026-01-21T14:30:00",
    "IsNotified": false
  }
]
```

## Correctness Properties

Uma propriedade é uma característica ou comportamento que deve ser verdadeiro em todas as execuções válidas de um sistema - essencialmente, uma declaração formal sobre o que o sistema deve fazer. As propriedades servem como ponte entre especificações legíveis por humanos e garantias de correção verificáveis por máquina.

### Property 1: Adicionar evento válido aumenta a lista

*Para qualquer* descrição não vazia e data/hora futura, adicionar um evento deve resultar no evento aparecendo na lista de eventos do sistema.

**Validates: Requirements 1.1, 1.5**

### Property 2: Descrições inválidas são rejeitadas

*Para qualquer* string composta apenas de espaços em branco ou vazia, tentar adicionar um evento com essa descrição deve ser rejeitado e a lista de eventos deve permanecer inalterada.

**Validates: Requirements 1.2**

### Property 3: Datas no passado são rejeitadas

*Para qualquer* data/hora no passado, tentar adicionar um evento com essa data deve ser rejeitado e a lista de eventos deve permanecer inalterada.

**Validates: Requirements 1.3**

### Property 4: Persistência round-trip

*Para qualquer* evento válido, adicionar o evento, salvar no armazenamento, e recarregar deve resultar em um evento equivalente sendo recuperado.

**Validates: Requirements 1.4, 5.1, 5.2**

### Property 5: Eventos são ordenados por data

*Para qualquer* conjunto de eventos com datas futuras diferentes, GetAllEvents deve retornar todos os eventos ordenados cronologicamente por EventDateTime.

**Validates: Requirements 2.1**

### Property 6: Filtro por data retorna apenas eventos daquela data

*Para qualquer* data específica e conjunto de eventos em múltiplas datas, GetEventsByDate deve retornar apenas os eventos cuja data corresponde exatamente à data especificada.

**Validates: Requirements 2.2**

### Property 7: Remover evento o exclui da lista

*Para qualquer* evento no calendário, após removê-lo, o evento não deve mais aparecer em GetAllEvents nem em GetEventsByDate.

**Validates: Requirements 3.1, 3.3**

### Property 8: Remoção persiste no armazenamento

*Para qualquer* evento, se adicionarmos, salvarmos, removermos, salvarmos novamente, e recarregarmos, o evento não deve estar presente.

**Validates: Requirements 3.2**

### Property 9: Notificação dispara para eventos devidos

*Para qualquer* evento cuja EventDateTime corresponde à hora atual do sistema, o EventNotificationService deve disparar o evento EventDue com os dados corretos do evento.

**Validates: Requirements 4.1, 4.2**

### Property 10: Múltiplos eventos simultâneos disparam múltiplas notificações

*Para qualquer* conjunto de eventos com a mesma EventDateTime, quando essa data/hora é atingida, o serviço deve disparar notificações para todos os eventos sequencialmente.

**Validates: Requirements 4.5**

## Error Handling

### Validation Errors

- **Descrição vazia ou whitespace**: Lançar `ArgumentException` com mensagem clara
- **Data no passado**: Lançar `ArgumentException` indicando que eventos devem ser futuros
- **Evento não encontrado**: Lançar `InvalidOperationException` ao tentar remover evento inexistente

### Storage Errors

- **Arquivo corrompido**: Capturar exceção de deserialização, registrar erro, e iniciar com lista vazia
- **Permissões de arquivo**: Capturar `UnauthorizedAccessException`, notificar usuário, e operar em modo somente memória
- **Disco cheio**: Capturar `IOException`, notificar usuário, mas manter eventos em memória

### Notification Service Errors

- **Timer falha**: Registrar erro e tentar reiniciar o serviço
- **Exceção ao disparar notificação**: Capturar, registrar, e continuar monitorando outros eventos

## Testing Strategy

### Dual Testing Approach

O projeto utilizará tanto testes unitários quanto testes baseados em propriedades para garantir correção abrangente:

- **Testes Unitários**: Verificam exemplos específicos, casos extremos e condições de erro
- **Testes de Propriedade**: Verificam propriedades universais através de múltiplas entradas geradas

### Property-Based Testing

Utilizaremos a biblioteca **FsCheck** para C#, que é a implementação .NET do QuickCheck.

**Configuração**:
- Mínimo de 100 iterações por teste de propriedade
- Cada teste deve referenciar a propriedade do documento de design
- Formato da tag: `// Feature: event-calendar, Property N: [texto da propriedade]`

**Estratégia de Geração**:
- **Descrições válidas**: Strings não vazias com 1-200 caracteres
- **Datas futuras**: DateTime entre agora e 5 anos no futuro
- **Datas passadas**: DateTime entre 5 anos atrás e agora
- **Strings inválidas**: Strings vazias, apenas espaços, apenas tabs

### Unit Testing

**Foco dos testes unitários**:
- Exemplos específicos de uso comum (adicionar evento para amanhã, remover evento específico)
- Casos extremos (lista vazia, múltiplos eventos na mesma data)
- Integração entre componentes (EventManager + EventRepository)
- Tratamento de erros (arquivo corrompido, permissões negadas)
- Comportamento do modal de notificação (exibição e confirmação)

**Framework**: NUnit ou xUnit para testes unitários

### Test Organization

```
EventCalendar.Tests/
├── Unit/
│   ├── EventManagerTests.cs
│   ├── EventRepositoryTests.cs
│   └── EventNotificationServiceTests.cs
└── Properties/
    ├── EventManagerPropertyTests.cs
    ├── EventRepositoryPropertyTests.cs
    └── EventNotificationServicePropertyTests.cs
```

### Coverage Goals

- Todas as 10 propriedades de correção devem ter testes de propriedade correspondentes
- Casos extremos e condições de erro devem ter testes unitários específicos
- Integração entre camadas deve ser testada com testes unitários

# Implementation Plan: Event Calendar

## Overview

Este plano de implementação detalha as etapas para construir o aplicativo Event Calendar em C#. A implementação seguirá uma abordagem incremental, começando pela camada de dados, depois lógica de negócio, e finalmente a interface do usuário.

## Tasks

- [ ] 1. Configurar estrutura do projeto
  - Criar solução C# com projeto Windows Forms
  - Adicionar projeto de testes (xUnit ou NUnit)
  - Instalar pacote FsCheck para property-based testing
  - Instalar Newtonsoft.Json para serialização
  - _Requirements: 5.4_

- [ ] 2. Implementar modelo de dados Event
  - [-] 2.1 Criar classe Event com propriedades (Id, Description, EventDateTime, IsNotified)
    - Implementar validação básica no construtor
    - _Requirements: 1.1, 1.2, 1.3_
  
  - [ ]* 2.2 Escrever testes de propriedade para validação de Event
    - **Property 2: Descrições inválidas são rejeitadas**
    - **Property 3: Datas no passado são rejeitadas**
    - **Validates: Requirements 1.2, 1.3**

- [ ] 3. Implementar EventRepository (camada de persistência)
  - [ ] 3.1 Criar interface IEventRepository
    - Definir métodos Save, Load, Add, Remove
    - _Requirements: 5.1, 5.2_
  
  - [ ] 3.2 Implementar EventRepository com serialização JSON
    - Implementar Save() para salvar lista de eventos em arquivo JSON
    - Implementar Load() para carregar eventos do arquivo JSON
    - Implementar Add() e Remove() com persistência imediata
    - Tratar erros de arquivo corrompido ou inacessível
    - _Requirements: 1.4, 3.2, 5.1, 5.2, 5.3, 5.4_
  
  - [ ]* 3.3 Escrever teste de propriedade para persistência round-trip
    - **Property 4: Persistência round-trip**
    - **Validates: Requirements 1.4, 5.1, 5.2**
  
  - [ ]* 3.4 Escrever testes unitários para tratamento de erros
    - Testar arquivo corrompido
    - Testar permissões negadas
    - _Requirements: 5.3_

- [ ] 4. Checkpoint - Verificar persistência
  - Garantir que todos os testes passam, perguntar ao usuário se há dúvidas.

- [ ] 5. Implementar EventManager (lógica de negócio)
  - [ ] 5.1 Criar interface IEventManager
    - Definir métodos AddEvent, RemoveEvent, GetAllEvents, GetEventsByDate, GetEventById
    - _Requirements: 1.1, 2.1, 2.2, 3.1_
  
  - [ ] 5.2 Implementar EventManager
    - Implementar AddEvent com validação (descrição não vazia, data futura)
    - Implementar RemoveEvent
    - Implementar GetAllEvents com ordenação por data
    - Implementar GetEventsByDate com filtro
    - Implementar GetEventById
    - Integrar com EventRepository para persistência
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 2.1, 2.2, 3.1, 3.2_
  
  - [ ]* 5.3 Escrever teste de propriedade para adicionar evento válido
    - **Property 1: Adicionar evento válido aumenta a lista**
    - **Validates: Requirements 1.1, 1.5**
  
  - [ ]* 5.4 Escrever teste de propriedade para ordenação de eventos
    - **Property 5: Eventos são ordenados por data**
    - **Validates: Requirements 2.1**
  
  - [ ]* 5.5 Escrever teste de propriedade para filtro por data
    - **Property 6: Filtro por data retorna apenas eventos daquela data**
    - **Validates: Requirements 2.2**
  
  - [ ]* 5.6 Escrever teste de propriedade para remoção de evento
    - **Property 7: Remover evento o exclui da lista**
    - **Property 8: Remoção persiste no armazenamento**
    - **Validates: Requirements 3.1, 3.2, 3.3**
  
  - [ ]* 5.7 Escrever testes unitários para validação
    - Testar rejeição de descrição vazia
    - Testar rejeição de data no passado
    - Testar remoção de evento inexistente
    - _Requirements: 1.2, 1.3_

- [ ] 6. Checkpoint - Verificar lógica de negócio
  - Garantir que todos os testes passam, perguntar ao usuário se há dúvidas.

- [ ] 7. Implementar EventNotificationService
  - [ ] 7.1 Criar interface IEventNotificationService
    - Definir métodos Start, Stop
    - Definir evento EventDue
    - _Requirements: 4.1, 4.4_
  
  - [ ] 7.2 Implementar EventNotificationService
    - Usar System.Threading.Timer para verificação periódica (a cada minuto)
    - Implementar CheckForDueEvents para comparar hora atual com eventos
    - Disparar evento EventDue quando evento é devido
    - Marcar evento como IsNotified após disparar
    - Suportar múltiplos eventos simultâneos (disparar sequencialmente)
    - _Requirements: 4.1, 4.2, 4.4, 4.5_
  
  - [ ]* 7.3 Escrever teste de propriedade para notificação de eventos devidos
    - **Property 9: Notificação dispara para eventos devidos**
    - **Validates: Requirements 4.1, 4.2**
  
  - [ ]* 7.4 Escrever teste de propriedade para múltiplos eventos simultâneos
    - **Property 10: Múltiplos eventos simultâneos disparam múltiplas notificações**
    - **Validates: Requirements 4.5**
  
  - [ ]* 7.5 Escrever testes unitários para o serviço
    - Testar Start e Stop
    - Testar tratamento de erros no timer
    - _Requirements: 4.4_

- [ ] 8. Checkpoint - Verificar serviço de notificação
  - Garantir que todos os testes passam, perguntar ao usuário se há dúvidas.

- [ ] 9. Implementar interface do usuário - MainForm
  - [ ] 9.1 Criar MainForm com controles básicos
    - Adicionar MonthCalendar para visualização de calendário
    - Adicionar ListBox ou DataGridView para exibir eventos
    - Adicionar botões "Adicionar Evento" e "Remover Evento"
    - Configurar layout e posicionamento dos controles
    - _Requirements: 2.1, 2.3, 6.1_
  
  - [ ] 9.2 Implementar carregamento e exibição de eventos
    - Carregar eventos ao iniciar o formulário
    - Exibir eventos ordenados por data no controle de lista
    - Mostrar descrição, data e hora de cada evento
    - Exibir mensagem quando não há eventos
    - Implementar filtro por data ao selecionar no MonthCalendar
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 5.1_
  
  - [ ] 9.3 Implementar funcionalidade de remover evento
    - Permitir seleção de evento na lista
    - Ao clicar em "Remover Evento", confirmar e remover
    - Atualizar lista após remoção
    - _Requirements: 3.1, 3.3, 6.2_

- [ ] 10. Implementar interface do usuário - AddEventForm
  - [ ] 10.1 Criar AddEventForm (diálogo modal)
    - Adicionar TextBox para descrição
    - Adicionar DateTimePicker para data e hora
    - Adicionar botões "Salvar" e "Cancelar"
    - _Requirements: 1.1, 6.1_
  
  - [ ] 10.2 Implementar validação e salvamento
    - Validar descrição não vazia ao salvar
    - Validar data futura ao salvar
    - Exibir mensagens de erro claras
    - Chamar EventManager.AddEvent ao salvar
    - Fechar diálogo após sucesso
    - _Requirements: 1.1, 1.2, 1.3, 6.2, 6.3_
  
  - [ ] 10.3 Integrar AddEventForm com MainForm
    - Abrir AddEventForm ao clicar em "Adicionar Evento"
    - Atualizar lista de eventos no MainForm após adicionar
    - _Requirements: 1.5_

- [ ] 11. Implementar interface do usuário - NotificationModal
  - [ ] 11.1 Criar NotificationModal (diálogo modal)
    - Adicionar Label para exibir descrição do evento
    - Adicionar botão "OK" ou "Confirmar"
    - Configurar para exibir no centro da tela
    - Configurar para ser TopMost (sempre visível)
    - _Requirements: 4.1, 4.2, 4.3_
  
  - [ ] 11.2 Integrar NotificationModal com EventNotificationService
    - Inscrever no evento EventDue do serviço
    - Exibir NotificationModal quando evento é disparado
    - Fechar modal apenas quando usuário clicar no botão
    - Suportar múltiplas notificações sequencialmente
    - _Requirements: 4.1, 4.2, 4.3, 4.5_

- [ ] 12. Integração final e inicialização
  - [ ] 12.1 Configurar Program.cs
    - Instanciar EventRepository
    - Instanciar EventManager com EventRepository
    - Instanciar EventNotificationService com EventManager
    - Iniciar EventNotificationService
    - Criar e exibir MainForm
    - _Requirements: 4.4, 5.1, 6.1_
  
  - [ ] 12.2 Implementar tratamento de erros global
    - Adicionar handler para exceções não tratadas
    - Registrar erros em arquivo de log
    - Exibir mensagens amigáveis ao usuário
    - _Requirements: 6.3_
  
  - [ ] 12.3 Garantir interface responsiva
    - Verificar que operações de I/O não bloqueiam UI
    - Usar async/await se necessário
    - _Requirements: 6.4_

- [ ] 13. Checkpoint final - Testes de integração
  - [ ]* 13.1 Escrever testes de integração end-to-end
    - Testar fluxo completo: adicionar → persistir → carregar → remover
    - Testar fluxo de notificação: adicionar evento → aguardar → verificar notificação
  
  - [ ] 13.2 Garantir que todos os testes passam
    - Executar todos os testes unitários
    - Executar todos os testes de propriedade
    - Corrigir quaisquer falhas
  
  - [ ] 13.3 Verificação final com o usuário
    - Garantir que todos os testes passam, perguntar ao usuário se há dúvidas.

## Notes

- Tasks marcadas com `*` são opcionais e podem ser puladas para um MVP mais rápido
- Cada task referencia requisitos específicos para rastreabilidade
- Checkpoints garantem validação incremental
- Testes de propriedade validam propriedades universais de correção
- Testes unitários validam exemplos específicos e casos extremos

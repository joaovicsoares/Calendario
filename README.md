# 📅 Event Calendar

Um aplicativo de calendário de eventos para Windows com notificações automáticas e execução em segundo plano.

![Windows](https://img.shields.io/badge/Windows-10%2F11-blue?logo=windows)
![.NET](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green)
![Version](https://img.shields.io/badge/Version-1.0.0-orange)

## ✨ Funcionalidades

- 📅 **Calendário Visual**: Interface intuitiva com MonthCalendar para seleção de datas
- ⏰ **Notificações Automáticas**: Popups no horário exato dos eventos
- 🔔 **Execução em Segundo Plano**: Funciona minimizado na bandeja do sistema
- 🚀 **Startup Automático**: Inicia junto com o Windows (opcional)
- 💾 **Persistência de Dados**: Salva eventos em arquivo JSON local
- 🎯 **Interface Moderna**: Design limpo e responsivo
- 🛡️ **Tratamento de Erros**: Sistema robusto de logs e recuperação

## 🖼️ Screenshots

### Tela Principal
- Calendário mensal para navegação
- Lista de eventos do dia selecionado
- Botões para adicionar/remover eventos

### System Tray
- Ícone na bandeja do sistema
- Menu de contexto com opções
- Controle de startup automático

### Notificações
- Popups automáticos no horário dos eventos
- Exibição sequencial para múltiplos eventos
- Sempre visível (TopMost)

## 🚀 Instalação

### Opção 1: Instalador (Recomendado) *ainda não implementado*
1. Baixe o instalador: `EventCalendar-Setup-v1.0.0.exe`
2. Execute o instalador
3. Siga as instruções na tela
4. Pronto! O app estará instalado e configurado

### Opção 2: Executável Portátil
1. Baixe a pasta `publish` completa
2. Execute `EventCalendar.exe`
3. Configure manualmente se necessário

### Opção 3: Compilar do Código
```bash
# Clonar repositório
git clone https://github.com/seuusuario/event-calendar.git
cd event-calendar

# Compilar
dotnet build EventCalendar

# Executar
dotnet run --project EventCalendar
```

## 🛠️ Desenvolvimento

### Pré-requisitos
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio 2022 ou VS Code (opcional)

### Estrutura do Projeto
```
EventCalendar/
├── Models/              # Modelos de dados
│   └── Event.cs
├── Services/            # Lógica de negócio
│   ├── EventManager.cs
│   ├── EventNotificationService.cs
│   └── StartupManager.cs
├── Repositories/        # Persistência de dados
│   ├── EventRepository.cs
│   └── IEventRepository.cs
├── Forms/              # Interface do usuário
│   ├── MainForm.cs
│   ├── AddEventForm.cs
│   └── NotificationModal.cs
└── Program.cs          # Ponto de entrada
```

### Compilar para Produção
```bash
# Build Release
dotnet publish EventCalendar -c Release -r win-x64 --self-contained

# Criar Instalador (requer Inno Setup)
build-installer.bat
```

## 📋 Como Usar

### Primeira Execução
1. Execute o aplicativo
2. A janela principal será exibida
3. Use o calendário para navegar entre datas

### Adicionar Eventos
1. Clique em "Adicionar Evento"
2. Preencha descrição e data/hora
3. Clique "Salvar"

### Configurar Notificações
1. Minimize o app (vai para a bandeja)
2. Clique direito no ícone da bandeja
3. Marque "Iniciar com Windows"
4. O app ficará sempre ativo para notificações

### Receber Notificações
- Popups aparecem automaticamente no horário dos eventos
- Funcionam mesmo com o app minimizado
- Clique "OK" para fechar a notificação

## ⚙️ Configuração

### Localização dos Dados
- **Eventos**: `%LOCALAPPDATA%\EventCalendar\events.json`
- **Logs**: `%LOCALAPPDATA%\EventCalendar\error.log`

### Startup Automático
- **Registry**: `HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- **Controle**: Menu da bandeja do sistema

### Personalização
O arquivo `events.json` pode ser editado manualmente se necessário:
```json
[
  {
    "Id": "guid-aqui",
    "Description": "Meu evento",
    "EventDateTime": "2026-01-20T15:30:00",
    "IsNotified": false
  }
]
```

## 🧪 Testes

```bash
# Executar testes unitários
dotnet test EventCalendar.Tests

# Executar com cobertura
dotnet test EventCalendar.Tests --collect:"XPlat Code Coverage"
```

## 🐛 Troubleshooting

### App não inicia
- Verifique se todos os arquivos foram copiados
- Execute como administrador
- Verifique logs em `%LOCALAPPDATA%\EventCalendar\error.log`

### Notificações não aparecem
- Verifique se o ícone está na bandeja
- Teste com evento próximo (1-2 minutos)
- Verifique configurações de notificação do Windows

### Startup não funciona
- Execute como administrador uma vez
- Verifique entrada no Registry
- Teste manualmente: `EventCalendar.exe --minimized`

## 📚 Documentação Adicional

- [📦 Guia de Deploy](DEPLOY_PRODUCAO.md)
- [🚀 Configuração de Startup](STARTUP_WINDOWS.md)
- [🛠️ Criar Instalador](CRIAR_INSTALADOR.md)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Padrões de Código
- Use async/await para operações I/O
- Implemente tratamento de erros adequado
- Adicione comentários XML para métodos públicos
- Siga as convenções de nomenclatura C#

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🏗️ Arquitetura

### Padrões Utilizados
- **Repository Pattern**: Para persistência de dados
- **Service Layer**: Para lógica de negócio
- **Event-Driven**: Para notificações
- **Dependency Injection**: Para desacoplamento

### Tecnologias
- **.NET 8.0**: Framework principal
- **Windows Forms**: Interface gráfica
- **Newtonsoft.Json**: Serialização JSON
- **System.Threading.Timer**: Monitoramento de eventos
- **Windows Registry**: Configuração de startup

## 📊 Estatísticas

- **Linguagem**: C# 100%
- **Linhas de Código**: ~2000
- **Arquivos**: 15+
- **Tamanho**: ~80MB (self-contained)
- **Tempo de Startup**: <2 segundos

## 🎯 Roadmap

### Versão 1.1
- [ ] Suporte a eventos recorrentes
- [ ] Categorias de eventos com cores
- [ ] Exportação para iCal
- [ ] Temas escuro/claro

### Versão 1.2
- [ ] Sincronização com Google Calendar
- [ ] Lembretes múltiplos por evento
- [ ] Interface web opcional
- [ ] Suporte a anexos

## 👨‍💻 Autor

**Seu Nome**
- GitHub: [@seuusuario](https://github.com/seuusuario)
- Email: seu.email@exemplo.com

## 🙏 Agradecimentos

- Microsoft pela plataforma .NET
- Comunidade open source
- Usuários que testaram e deram feedback

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!** ⭐
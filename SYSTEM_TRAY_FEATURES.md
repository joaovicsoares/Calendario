# Funcionalidades do System Tray - Event Calendar

## O que foi implementado:

### 1. Minimizar para System Tray
- **Fechar (X)**: Em vez de fechar o aplicativo, ele é minimizado para a bandeja do sistema
- **Minimizar**: Quando você minimiza a janela, ela vai direto para a bandeja
- **Ícone na bandeja**: Aparece um ícone do aplicativo na área de notificação

### 2. Menu de Contexto
Clique com o botão direito no ícone da bandeja para ver:
- **Mostrar**: Restaura a janela principal
- **Sair**: Fecha o aplicativo completamente

### 3. Duplo Clique
- Duplo clique no ícone da bandeja restaura a janela principal

### 4. Notificações em Segundo Plano
- **Popups continuam funcionando**: Mesmo com o app minimizado, os popups de eventos aparecem no centro da tela
- **TopMost**: Os popups aparecem sempre por cima de outras janelas
- **Sem interferência**: O app continua monitorando eventos mesmo minimizado

### 5. Feedback Visual
- Quando minimiza, mostra uma notificação "balloon tip" informando que o app está na bandeja

## Como usar:

1. **Para manter o app rodando**: Clique no X ou minimize - o app vai para a bandeja
2. **Para voltar ao app**: Duplo clique no ícone da bandeja ou clique direito → Mostrar
3. **Para fechar completamente**: Clique direito no ícone da bandeja → Sair

## Benefícios:

✅ App continua rodando em segundo plano
✅ Notificações funcionam mesmo com app "fechado"
✅ Não ocupa espaço na barra de tarefas quando minimizado
✅ Fácil acesso através da bandeja do sistema
✅ Popups aparecem no centro da tela independente do estado do app
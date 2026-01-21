using System;
using System.Windows.Forms;
using EventCalendar.Services;

namespace EventCalendar
{
    /// <summary>
    /// Main form for the Event Calendar application.
    /// Provides UI for viewing, adding, and removing events.
    /// Requirements: 2.1, 2.3, 6.1
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly IEventManager _eventManager;
        private readonly IEventNotificationService _notificationService;
        
        // UI Controls
        private MonthCalendar monthCalendar = null!;
        private ListBox eventsListBox = null!;
        private Button addEventButton = null!;
        private Button removeEventButton = null!;
        private Label titleLabel = null!;
        private Label eventsLabel = null!;
        private NotifyIcon notifyIcon = null!;

        /// <summary>
        /// Creates a new MainForm with the specified event manager and notification service.
        /// </summary>
        /// <param name="eventManager">Event manager for business logic</param>
        /// <param name="notificationService">Notification service for event alerts</param>
        public MainForm(IEventManager eventManager, IEventNotificationService notificationService)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            
            InitializeComponent();
            
            // Initialize system tray icon
            InitializeSystemTray();
            
            // Wire up event handlers
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;
            monthCalendar.DateChanged += MonthCalendar_DateChanged;
            addEventButton.Click += AddEventButton_Click;
            removeEventButton.Click += RemoveEventButton_Click;
            
            // Subscribe to notification service events (Requirements 4.1, 4.2)
            _notificationService.EventDue += NotificationService_EventDue;
        }

        /// <summary>
        /// Handles form load event - loads and displays all events asynchronously.
        /// Requirements: 2.1, 5.1, 6.4
        /// </summary>
        private async void MainForm_Load(object? sender, EventArgs e)
        {
            // If starting minimized, hide immediately
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
            
            await LoadAndDisplayAllEventsAsync();
        }

        /// <summary>
        /// Handles form closing event - minimizes to tray instead of closing.
        /// </summary>
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // If user clicked X button, minimize to tray instead of closing
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.ShowBalloonTip(2000, "Event Calendar", "Aplicativo minimizado para a bandeja do sistema", ToolTipIcon.Info);
                return;
            }
            
            // Only actually close when shutting down the application
            // Unsubscribe from notification service
            _notificationService.EventDue -= NotificationService_EventDue;
            
            // Clean up system tray icon
            notifyIcon?.Dispose();
        }

        /// <summary>
        /// Handles form resize event - minimizes to tray when minimized.
        /// </summary>
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.ShowBalloonTip(2000, "Event Calendar", "Aplicativo minimizado para a bandeja do sistema", ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// Initializes the system tray icon and context menu.
        /// </summary>
        private void InitializeSystemTray()
        {
            // Create context menu for system tray
            var contextMenu = new ContextMenuStrip();
            
            var showMenuItem = new ToolStripMenuItem("Mostrar");
            showMenuItem.Click += (s, e) => ShowMainForm();
            
            var startupMenuItem = new ToolStripMenuItem("Iniciar com Windows");
            startupMenuItem.Checked = Services.StartupManager.IsStartupEnabled();
            startupMenuItem.Click += (s, e) => ToggleStartup(startupMenuItem);
            
            var exitMenuItem = new ToolStripMenuItem("Sair");
            exitMenuItem.Click += (s, e) => ExitApplication();
            
            contextMenu.Items.Add(showMenuItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(startupMenuItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitMenuItem);
            
            // Create notify icon
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // You can replace with a custom icon
                Text = "Event Calendar - Calendário de Eventos",
                Visible = true,
                ContextMenuStrip = contextMenu
            };
            
            // Double-click to show main form
            notifyIcon.DoubleClick += (s, e) => ShowMainForm();
        }

        /// <summary>
        /// Toggles the Windows startup setting.
        /// </summary>
        private void ToggleStartup(ToolStripMenuItem menuItem)
        {
            try
            {
                bool currentState = Services.StartupManager.IsStartupEnabled();
                bool success;
                
                if (currentState)
                {
                    success = Services.StartupManager.DisableStartup();
                    if (success)
                    {
                        menuItem.Checked = false;
                        notifyIcon.ShowBalloonTip(2000, "Event Calendar", "Startup automático desabilitado", ToolTipIcon.Info);
                    }
                }
                else
                {
                    success = Services.StartupManager.EnableStartup();
                    if (success)
                    {
                        menuItem.Checked = true;
                        notifyIcon.ShowBalloonTip(2000, "Event Calendar", "Startup automático habilitado", ToolTipIcon.Info);
                    }
                }
                
                if (!success)
                {
                    MessageBox.Show(
                        "Erro ao alterar configuração de startup. Verifique as permissões.",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao alterar startup: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows the main form and brings it to front.
        /// </summary>
        private void ShowMainForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.BringToFront();
            this.Activate();
        }

        /// <summary>
        /// Exits the application completely.
        /// </summary>
        private void ExitApplication()
        {
            // Unsubscribe from notification service
            _notificationService.EventDue -= NotificationService_EventDue;
            
            // Clean up system tray icon
            notifyIcon?.Dispose();
            
            // Exit application
            Application.Exit();
        }
        /// <summary>
        /// Handles event due notification - displays NotificationModal.
        /// Requirements: 4.1, 4.2, 4.3, 4.5
        /// </summary>
        private void NotificationService_EventDue(object? sender, Models.Event e)
        {
            // Ensure UI updates happen on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowNotificationModal(e)));
            }
            else
            {
                ShowNotificationModal(e);
            }
        }

        /// <summary>
        /// Shows the notification modal for a due event.
        /// Supports multiple notifications sequentially.
        /// Shows popup even when app is minimized to tray.
        /// Requirements: 4.1, 4.2, 4.3, 4.5
        /// </summary>
        private async void ShowNotificationModal(Models.Event dueEvent)
        {
            try
            {
                // Create and show notification modal (Requirements 4.1, 4.2, 4.3)
                using (var notificationModal = new NotificationModal(dueEvent))
                {
                    // Show notification even if main form is hidden
                    // This ensures popups appear even when app is in system tray
                    notificationModal.TopMost = true;
                    notificationModal.StartPosition = FormStartPosition.CenterScreen;
                    
                    // ShowDialog blocks until user clicks OK (Requirement 4.3)
                    // This ensures sequential display for multiple events (Requirement 4.5)
                    notificationModal.ShowDialog();
                }
                
                // Refresh the event list to reflect any changes (only if main form is visible)
                if (this.Visible)
                {
                    await LoadAndDisplayAllEventsAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                MessageBox.Show(
                    $"Erro ao exibir notificação: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles date selection change in MonthCalendar - filters events by selected date asynchronously.
        /// Requirements: 2.2, 6.4
        /// </summary>
        private async void MonthCalendar_DateChanged(object? sender, DateRangeEventArgs e)
        {
            await LoadAndDisplayEventsByDateAsync(monthCalendar.SelectionStart);
        }

        /// <summary>
        /// Handles add event button click - opens AddEventForm dialog asynchronously.
        /// Requirements: 1.5, 6.4
        /// </summary>
        private async void AddEventButton_Click(object? sender, EventArgs e)
        {
            // Open AddEventForm as modal dialog (Requirement 1.5)
            using (var addEventForm = new AddEventForm(_eventManager))
            {
                var result = addEventForm.ShowDialog(this);
                
                // Update list of events after adding (Requirement 1.5)
                if (result == DialogResult.OK)
                {
                    await LoadAndDisplayAllEventsAsync();
                    
                    // Provide feedback
                    MessageBox.Show(
                        "Evento adicionado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Handles remove event button click - removes selected event with confirmation asynchronously.
        /// Requirements: 3.1, 3.3, 6.2, 6.4
        /// </summary>
        private async void RemoveEventButton_Click(object? sender, EventArgs e)
        {
            // Check if an event is selected (Requirement 3.1)
            if (eventsListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Por favor, selecione um evento para remover.",
                    "Nenhum evento selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Check if the selected item is an actual event (not a message)
            if (!(eventsListBox.SelectedItem is EventListItem eventItem))
            {
                MessageBox.Show(
                    "Por favor, selecione um evento válido para remover.",
                    "Seleção inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Confirm removal (Requirement 3.1)
            var result = MessageBox.Show(
                $"Tem certeza que deseja remover o evento:\n\n{eventItem.Event.Description}\n{eventItem.Event.EventDateTime:dd/MM/yyyy HH:mm}?",
                "Confirmar remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Disable button during removal to prevent double-click (Requirement 6.4)
                    removeEventButton.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;

                    // Remove event (Requirements 3.1, 3.3, 6.4)
                    await _eventManager.RemoveEventAsync(eventItem.Event.Id);
                    
                    // Update list after removal (Requirement 3.3)
                    await LoadAndDisplayAllEventsAsync();
                    
                    // Provide feedback (Requirement 6.2)
                    MessageBox.Show(
                        "Evento removido com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Display error message (Requirement 6.3)
                    MessageBox.Show(
                        $"Erro ao remover evento: {ex.Message}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    // Re-enable button and restore cursor (Requirement 6.4)
                    removeEventButton.Enabled = true;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Loads and displays all events ordered by date asynchronously.
        /// Requirements: 2.1, 2.3, 2.4, 6.4
        /// </summary>
        private async Task LoadAndDisplayAllEventsAsync()
        {
            eventsListBox.Items.Clear();
            
            var events = await _eventManager.GetAllEventsAsync();
            
            // Display message when no events exist (Requirement 2.3)
            if (events.Count == 0)
            {
                eventsListBox.Items.Add("Nenhum evento cadastrado.");
                return;
            }
            
            // Display events with description, date and time (Requirement 2.4)
            foreach (var evt in events)
            {
                string eventDisplay = $"{evt.EventDateTime:dd/MM/yyyy HH:mm} - {evt.Description}";
                eventsListBox.Items.Add(new EventListItem(evt, eventDisplay));
            }
        }

        /// <summary>
        /// Loads and displays events for a specific date asynchronously.
        /// Requirements: 2.2, 2.3, 2.4, 6.4
        /// </summary>
        private async Task LoadAndDisplayEventsByDateAsync(DateTime date)
        {
            eventsListBox.Items.Clear();
            
            var events = await _eventManager.GetEventsByDateAsync(date);
            
            // Display message when no events exist for the selected date (Requirement 2.3)
            if (events.Count == 0)
            {
                eventsListBox.Items.Add($"Nenhum evento para {date:dd/MM/yyyy}.");
                return;
            }
            
            // Display events with description, date and time (Requirement 2.4)
            foreach (var evt in events)
            {
                string eventDisplay = $"{evt.EventDateTime:dd/MM/yyyy HH:mm} - {evt.Description}";
                eventsListBox.Items.Add(new EventListItem(evt, eventDisplay));
            }
        }

        /// <summary>
        /// Helper class to store event data in ListBox items.
        /// </summary>
        private class EventListItem
        {
            public Models.Event Event { get; }
            private string DisplayText { get; }

            public EventListItem(Models.Event evt, string displayText)
            {
                Event = evt;
                DisplayText = displayText;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        /// <summary>
        /// Initializes the form components and layout.
        /// Requirements: 2.1, 2.3, 6.1
        /// </summary>
        private void InitializeComponent()
        {
            // Form configuration
            this.Text = "Calendário de Eventos";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(800, 600);

            // Title Label
            titleLabel = new Label
            {
                Text = "Calendário de Eventos",
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(300, 35),
                AutoSize = true
            };

            // MonthCalendar - for date selection and visualization
            monthCalendar = new MonthCalendar
            {
                Location = new System.Drawing.Point(20, 70),
                MaxSelectionCount = 1
            };

            // Events Label
            eventsLabel = new Label
            {
                Text = "Eventos:",
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(300, 70),
                Size = new System.Drawing.Size(100, 25),
                AutoSize = true
            };

            // ListBox - for displaying events
            eventsListBox = new ListBox
            {
                Location = new System.Drawing.Point(300, 100),
                Size = new System.Drawing.Size(460, 350),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                SelectionMode = SelectionMode.One
            };

            // Add Event Button
            addEventButton = new Button
            {
                Text = "Adicionar Evento",
                Location = new System.Drawing.Point(300, 470),
                Size = new System.Drawing.Size(220, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addEventButton.FlatAppearance.BorderSize = 0;

            // Remove Event Button
            removeEventButton = new Button
            {
                Text = "Remover Evento",
                Location = new System.Drawing.Point(540, 470),
                Size = new System.Drawing.Size(220, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                BackColor = System.Drawing.Color.FromArgb(232, 17, 35),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            removeEventButton.FlatAppearance.BorderSize = 0;

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(monthCalendar);
            this.Controls.Add(eventsLabel);
            this.Controls.Add(eventsListBox);
            this.Controls.Add(addEventButton);
            this.Controls.Add(removeEventButton);
        }
    }
}

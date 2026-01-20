using System;
using System.Windows.Forms;
using EventCalendar.Services;

namespace EventCalendar
{
    /// <summary>
    /// Modal dialog for adding new events to the calendar.
    /// Requirements: 1.1, 6.1
    /// </summary>
    public partial class AddEventForm : Form
    {
        private readonly IEventManager _eventManager;
        
        // UI Controls
        private Label titleLabel = null!;
        private Label descriptionLabel = null!;
        private TextBox descriptionTextBox = null!;
        private Label dateTimeLabel = null!;
        private DateTimePicker dateTimePicker = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;

        /// <summary>
        /// Creates a new AddEventForm with the specified event manager.
        /// </summary>
        /// <param name="eventManager">Event manager for adding events</param>
        public AddEventForm(IEventManager eventManager)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            InitializeComponent();
            
            // Wire up event handlers
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
        }

        /// <summary>
        /// Handles save button click - validates and saves the event asynchronously.
        /// Requirements: 1.1, 1.2, 1.3, 6.2, 6.3, 6.4
        /// </summary>
        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            // Validate description is not empty (Requirement 1.2)
            string description = descriptionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show(
                    "Por favor, insira uma descrição para o evento.",
                    "Descrição inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                descriptionTextBox.Focus();
                return;
            }

            // Validate date is in the future (Requirement 1.3)
            DateTime selectedDateTime = dateTimePicker.Value;
            if (selectedDateTime <= DateTime.Now)
            {
                MessageBox.Show(
                    "A data e hora do evento devem ser no futuro.",
                    "Data inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                dateTimePicker.Focus();
                return;
            }

            try
            {
                // Disable buttons during save to prevent double-click (Requirement 6.4)
                saveButton.Enabled = false;
                cancelButton.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                // Call EventManager.AddEventAsync (Requirements 1.1, 6.2, 6.4)
                await _eventManager.AddEventAsync(description, selectedDateTime);
                
                // Close dialog after success (Requirement 1.1)
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                // Display clear error message (Requirement 6.3)
                MessageBox.Show(
                    $"Erro ao adicionar evento: {ex.Message}",
                    "Erro de validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Display clear error message for unexpected errors (Requirement 6.3)
                MessageBox.Show(
                    $"Erro inesperado ao adicionar evento: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable buttons and restore cursor (Requirement 6.4)
                saveButton.Enabled = true;
                cancelButton.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles cancel button click - closes the dialog without saving.
        /// </summary>
        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Initializes the form components and layout.
        /// Requirements: 1.1, 6.1
        /// </summary>
        private void InitializeComponent()
        {
            // Form configuration
            this.Text = "Adicionar Evento";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title Label
            titleLabel = new Label
            {
                Text = "Novo Evento",
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(200, 30),
                AutoSize = true
            };

            // Description Label
            descriptionLabel = new Label
            {
                Text = "Descrição:",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(100, 25),
                AutoSize = true
            };

            // Description TextBox
            descriptionTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(440, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                MaxLength = 200
            };

            // DateTime Label
            dateTimeLabel = new Label
            {
                Text = "Data e Hora:",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new System.Drawing.Point(20, 145),
                Size = new System.Drawing.Size(100, 25),
                AutoSize = true
            };

            // DateTimePicker
            dateTimePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(20, 175),
                Size = new System.Drawing.Size(440, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm",
                ShowUpDown = false,
                MinDate = DateTime.Now,
                Value = DateTime.Now.AddHours(1) // Default to 1 hour from now
            };

            // Save Button
            saveButton = new Button
            {
                Text = "Salvar",
                Location = new System.Drawing.Point(260, 220),
                Size = new System.Drawing.Size(100, 35),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;

            // Cancel Button
            cancelButton = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(370, 220),
                Size = new System.Drawing.Size(100, 35),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                BackColor = System.Drawing.Color.Gray,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(descriptionTextBox);
            this.Controls.Add(dateTimeLabel);
            this.Controls.Add(dateTimePicker);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
        }
    }
}

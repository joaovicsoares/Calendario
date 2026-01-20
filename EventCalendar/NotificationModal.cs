using System;
using System.Drawing;
using System.Windows.Forms;
using EventCalendar.Models;

namespace EventCalendar
{
    /// <summary>
    /// Modal dialog that displays event notifications to the user.
    /// Requires user confirmation before closing.
    /// </summary>
    public class NotificationModal : Form
    {
        private Label lblEventDescription = null!;
        private Button btnConfirm = null!;

        /// <summary>
        /// Creates a new notification modal for the specified event.
        /// </summary>
        /// <param name="eventItem">The event to display in the notification</param>
        public NotificationModal(Event eventItem)
        {
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }

            InitializeComponents(eventItem);
            ConfigureForm();
        }

        /// <summary>
        /// Initializes the form controls.
        /// </summary>
        private void InitializeComponents(Event eventItem)
        {
            // Label for event description
            lblEventDescription = new Label
            {
                Text = $"Lembrete de Evento:\n\n{eventItem.Description}\n\n{eventItem.EventDateTime:dd/MM/yyyy HH:mm}",
                AutoSize = false,
                Size = new Size(360, 120),
                Location = new Point(20, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular)
            };

            // Confirm button
            btnConfirm = new Button
            {
                Text = "OK",
                Size = new Size(100, 35),
                Location = new Point(150, 150),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                DialogResult = DialogResult.OK
            };

            btnConfirm.Click += BtnConfirm_Click;

            // Add controls to form
            Controls.Add(lblEventDescription);
            Controls.Add(btnConfirm);
        }

        /// <summary>
        /// Configures the form properties.
        /// </summary>
        private void ConfigureForm()
        {
            // Form properties (Requirements 4.1, 4.2, 4.3)
            Text = "Notificação de Evento";
            Size = new Size(400, 230);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true; // Always visible on top
            ShowInTaskbar = true;
        }

        /// <summary>
        /// Handles the confirm button click event.
        /// </summary>
        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            // Close the modal when user confirms
            Close();
        }
    }
}

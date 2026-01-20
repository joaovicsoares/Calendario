using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventCalendar.Models;

namespace EventCalendar.Services
{
    /// <summary>
    /// Background service that monitors events and triggers notifications when they are due.
    /// Uses a timer to check for due events periodically.
    /// Uses async operations to prevent blocking (Requirement 6.4).
    /// Requirements: 4.1, 4.2, 4.4, 4.5
    /// </summary>
    public class EventNotificationService : IEventNotificationService
    {
        private readonly IEventManager _eventManager;
        private System.Threading.Timer? _timer;
        private readonly object _lock = new object();
        private bool _isRunning;

        /// <summary>
        /// Event raised when an event is due for notification.
        /// Requirement: 4.1
        /// </summary>
        public event EventHandler<Event>? EventDue;

        /// <summary>
        /// Creates a new EventNotificationService with the specified event manager.
        /// </summary>
        /// <param name="eventManager">Event manager to retrieve events from</param>
        public EventNotificationService(IEventManager eventManager)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _isRunning = false;
        }

        /// <summary>
        /// Starts the notification service to begin monitoring events.
        /// Checks for due events every minute.
        /// Requirement: 4.4
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return; // Already running
                }

                // Create timer that checks every minute (60000 milliseconds)
                _timer = new System.Threading.Timer(
                    callback: async (state) => await CheckForDueEventsAsync(),
                    state: null,
                    dueTime: TimeSpan.Zero, // Start immediately
                    period: TimeSpan.FromMinutes(1) // Check every minute
                );

                _isRunning = true;
            }
        }

        /// <summary>
        /// Stops the notification service and releases resources.
        /// Requirement: 4.4
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    return; // Already stopped
                }

                _timer?.Dispose();
                _timer = null;
                _isRunning = false;
            }
        }

        /// <summary>
        /// Checks for events that are due and triggers notifications asynchronously.
        /// Supports multiple simultaneous events by triggering them sequentially.
        /// Requirements: 4.1, 4.2, 4.5, 6.4
        /// </summary>
        private async Task CheckForDueEventsAsync()
        {
            try
            {
                // Get due events using the async method from EventManager
                var dueEvents = await _eventManager.GetDueEventsAsync();

                // Trigger notifications for all due events sequentially (Requirement 4.5)
                foreach (var dueEvent in dueEvents)
                {
                    // Mark event as notified (Requirements 4.2)
                    await _eventManager.MarkEventAsNotifiedAsync(dueEvent.Id);

                    // Trigger the notification event (Requirements 4.1, 4.2)
                    OnEventDue(dueEvent);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue monitoring
                // In a production system, this would use a proper logging framework
                Console.WriteLine($"Erro ao verificar eventos devidos: {ex.Message}");
            }
        }

        /// <summary>
        /// Raises the EventDue event.
        /// </summary>
        /// <param name="dueEvent">The event that is due</param>
        protected virtual void OnEventDue(Event dueEvent)
        {
            EventDue?.Invoke(this, dueEvent);
        }
    }
}

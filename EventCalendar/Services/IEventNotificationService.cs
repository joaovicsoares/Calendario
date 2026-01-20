using System;
using EventCalendar.Models;

namespace EventCalendar.Services
{
    /// <summary>
    /// Interface for the event notification service.
    /// Monitors events and triggers notifications when they are due.
    /// Requirements: 4.1, 4.4
    /// </summary>
    public interface IEventNotificationService
    {
        /// <summary>
        /// Starts the notification service to begin monitoring events.
        /// Requirement: 4.4
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the notification service and releases resources.
        /// Requirement: 4.4
        /// </summary>
        void Stop();

        /// <summary>
        /// Event raised when an event is due for notification.
        /// Requirement: 4.1
        /// </summary>
        event EventHandler<Event> EventDue;
    }
}

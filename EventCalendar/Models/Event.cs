using System;

namespace EventCalendar.Models
{
    /// <summary>
    /// Represents a calendar event with description, date/time, and notification status.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Unique identifier for the event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Description of the event. Cannot be null or whitespace.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time when the event occurs. Must be in the future when created.
        /// </summary>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Indicates whether the user has been notified about this event.
        /// </summary>
        public bool IsNotified { get; set; }

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public Event()
        {
            Id = Guid.NewGuid();
            IsNotified = false;
        }

        /// <summary>
        /// Creates a new event with validation.
        /// </summary>
        /// <param name="description">Event description (cannot be empty or whitespace)</param>
        /// <param name="eventDateTime">Event date and time (must be in the future)</param>
        /// <exception cref="ArgumentException">Thrown when description is invalid or date is in the past</exception>
        public Event(string description, DateTime eventDateTime)
        {
            // Requirement 1.2: Validate description is not empty or whitespace
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("A descrição do evento não pode estar vazia.", nameof(description));
            }

            // Requirement 1.3: Validate date is in the future
            if (eventDateTime <= DateTime.Now)
            {
                throw new ArgumentException("A data e hora do evento devem estar no futuro.", nameof(eventDateTime));
            }

            // Requirement 1.1: Create event with valid properties
            Id = Guid.NewGuid();
            Description = description;
            EventDateTime = eventDateTime;
            IsNotified = false;
        }
    }
}

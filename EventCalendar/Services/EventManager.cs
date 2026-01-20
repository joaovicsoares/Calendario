using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCalendar.Models;
using EventCalendar.Repositories;

namespace EventCalendar.Services
{
    /// <summary>
    /// Manages calendar events with validation and persistence.
    /// Implements business logic for event operations.
    /// Uses async operations to prevent UI blocking (Requirement 6.4).
    /// Requirements: 1.1, 1.2, 1.3, 1.4, 2.1, 2.2, 3.1, 3.2
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly IEventRepository _repository;

        /// <summary>
        /// Creates a new EventManager with the specified repository.
        /// </summary>
        /// <param name="repository">Repository for event persistence</param>
        public EventManager(IEventRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Adds a new event to the calendar with validation asynchronously.
        /// Requirements: 1.1, 1.2, 1.3, 1.4, 6.4
        /// </summary>
        /// <param name="description">Event description (cannot be empty or whitespace)</param>
        /// <param name="eventDateTime">Event date and time (must be in the future)</param>
        /// <exception cref="ArgumentException">Thrown when description is invalid or date is in the past</exception>
        public async Task AddEventAsync(string description, DateTime eventDateTime)
        {
            // Validate description (Requirement 1.2)
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("A descrição do evento não pode estar vazia.", nameof(description));
            }

            // Validate date is in the future (Requirement 1.3)
            if (eventDateTime <= DateTime.Now)
            {
                throw new ArgumentException("A data e hora do evento devem estar no futuro.", nameof(eventDateTime));
            }

            // Create and add event (Requirements 1.1, 1.4)
            var newEvent = new Event(description, eventDateTime);
            await _repository.AddAsync(newEvent);
        }

        /// <summary>
        /// Removes an event from the calendar asynchronously.
        /// Requirements: 3.1, 3.2, 6.4
        /// </summary>
        /// <param name="eventId">ID of the event to remove</param>
        /// <exception cref="InvalidOperationException">Thrown when event is not found</exception>
        public async Task RemoveEventAsync(Guid eventId)
        {
            // Check if event exists before removing
            var existingEvent = await GetEventByIdAsync(eventId);
            if (existingEvent == null)
            {
                throw new InvalidOperationException($"Evento com ID {eventId} não foi encontrado.");
            }

            // Remove event with immediate persistence (Requirements 3.1, 3.2)
            await _repository.RemoveAsync(eventId);
        }

        /// <summary>
        /// Gets all events ordered by date asynchronously.
        /// Requirements: 2.1, 6.4
        /// </summary>
        /// <returns>List of all events sorted chronologically by EventDateTime</returns>
        public async Task<List<Event>> GetAllEventsAsync()
        {
            var events = await _repository.LoadAsync();
            
            // Sort events chronologically by EventDateTime (Requirement 2.1)
            return events.OrderBy(e => e.EventDateTime).ToList();
        }

        /// <summary>
        /// Gets all events for a specific date asynchronously.
        /// Requirements: 2.2, 6.4
        /// </summary>
        /// <param name="date">Date to filter events by</param>
        /// <returns>List of events occurring on the specified date</returns>
        public async Task<List<Event>> GetEventsByDateAsync(DateTime date)
        {
            var events = await _repository.LoadAsync();
            
            // Filter events by date (Requirement 2.2)
            return events
                .Where(e => e.EventDateTime.Date == date.Date)
                .OrderBy(e => e.EventDateTime)
                .ToList();
        }

        /// <summary>
        /// Gets a specific event by its ID asynchronously.
        /// Requirement: 6.4
        /// </summary>
        /// <param name="eventId">ID of the event to retrieve</param>
        /// <returns>The event with the specified ID, or null if not found</returns>
        public async Task<Event?> GetEventByIdAsync(Guid eventId)
        {
            var events = await _repository.LoadAsync();
            return events.FirstOrDefault(e => e.Id == eventId);
        }

        /// <summary>
        /// Gets all events that are due (current time matches event time and not yet notified).
        /// Used by notification service.
        /// Requirements: 4.1, 4.2, 6.4
        /// </summary>
        /// <returns>List of events that are due for notification</returns>
        public async Task<List<Event>> GetDueEventsAsync()
        {
            var events = await _repository.LoadAsync();
            var now = DateTime.Now;
            
            // Find events where the minute matches and not yet notified
            return events
                .Where(e => !e.IsNotified && 
                           e.EventDateTime.Year == now.Year &&
                           e.EventDateTime.Month == now.Month &&
                           e.EventDateTime.Day == now.Day &&
                           e.EventDateTime.Hour == now.Hour &&
                           e.EventDateTime.Minute == now.Minute)
                .ToList();
        }

        /// <summary>
        /// Marks an event as notified asynchronously.
        /// Requirements: 4.2, 6.4
        /// </summary>
        /// <param name="eventId">ID of the event to mark as notified</param>
        public async Task MarkEventAsNotifiedAsync(Guid eventId)
        {
            var events = await _repository.LoadAsync();
            var eventToUpdate = events.FirstOrDefault(e => e.Id == eventId);
            
            if (eventToUpdate != null)
            {
                eventToUpdate.IsNotified = true;
                await _repository.SaveAsync(events);
            }
        }
    }
}

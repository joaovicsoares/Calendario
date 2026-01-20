using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventCalendar.Models;

namespace EventCalendar.Services
{
    /// <summary>
    /// Interface for managing calendar events.
    /// Provides operations for adding, removing, and querying events.
    /// Requirements: 1.1, 2.1, 2.2, 3.1
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Adds a new event to the calendar with validation asynchronously.
        /// </summary>
        /// <param name="description">Event description (cannot be empty or whitespace)</param>
        /// <param name="eventDateTime">Event date and time (must be in the future)</param>
        /// <exception cref="ArgumentException">Thrown when description is invalid or date is in the past</exception>
        Task AddEventAsync(string description, DateTime eventDateTime);

        /// <summary>
        /// Removes an event from the calendar asynchronously.
        /// </summary>
        /// <param name="eventId">ID of the event to remove</param>
        /// <exception cref="InvalidOperationException">Thrown when event is not found</exception>
        Task RemoveEventAsync(Guid eventId);

        /// <summary>
        /// Gets all events ordered by date asynchronously.
        /// </summary>
        /// <returns>List of all events sorted chronologically by EventDateTime</returns>
        Task<List<Event>> GetAllEventsAsync();

        /// <summary>
        /// Gets all events for a specific date asynchronously.
        /// </summary>
        /// <param name="date">Date to filter events by</param>
        /// <returns>List of events occurring on the specified date</returns>
        Task<List<Event>> GetEventsByDateAsync(DateTime date);

        /// <summary>
        /// Gets a specific event by its ID asynchronously.
        /// </summary>
        /// <param name="eventId">ID of the event to retrieve</param>
        /// <returns>The event with the specified ID, or null if not found</returns>
        Task<Event?> GetEventByIdAsync(Guid eventId);

        /// <summary>
        /// Gets all events that are due (current time matches event time and not yet notified).
        /// Used by notification service.
        /// </summary>
        /// <returns>List of events that are due for notification</returns>
        Task<List<Event>> GetDueEventsAsync();

        /// <summary>
        /// Marks an event as notified.
        /// </summary>
        /// <param name="eventId">ID of the event to mark as notified</param>
        Task MarkEventAsNotifiedAsync(Guid eventId);
    }
}

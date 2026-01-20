using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventCalendar.Models;

namespace EventCalendar.Repositories
{
    /// <summary>
    /// Interface for event persistence operations.
    /// Handles saving, loading, adding, and removing events from storage.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Saves the entire list of events to storage asynchronously.
        /// </summary>
        /// <param name="events">List of events to save</param>
        Task SaveAsync(List<Event> events);

        /// <summary>
        /// Loads all events from storage asynchronously.
        /// </summary>
        /// <returns>List of events, or empty list if no events exist or error occurs</returns>
        Task<List<Event>> LoadAsync();

        /// <summary>
        /// Adds a new event to storage with immediate persistence asynchronously.
        /// </summary>
        /// <param name="eventItem">Event to add</param>
        Task AddAsync(Event eventItem);

        /// <summary>
        /// Removes an event from storage with immediate persistence asynchronously.
        /// </summary>
        /// <param name="eventId">ID of the event to remove</param>
        Task RemoveAsync(Guid eventId);
    }
}

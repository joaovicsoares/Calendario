using System;
using System.Collections.Generic;
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
        /// Saves the entire list of events to storage.
        /// </summary>
        /// <param name="events">List of events to save</param>
        void Save(List<Event> events);

        /// <summary>
        /// Loads all events from storage.
        /// </summary>
        /// <returns>List of events, or empty list if no events exist or error occurs</returns>
        List<Event> Load();

        /// <summary>
        /// Adds a new event to storage with immediate persistence.
        /// </summary>
        /// <param name="eventItem">Event to add</param>
        void Add(Event eventItem);

        /// <summary>
        /// Removes an event from storage with immediate persistence.
        /// </summary>
        /// <param name="eventId">ID of the event to remove</param>
        void Remove(Guid eventId);
    }
}

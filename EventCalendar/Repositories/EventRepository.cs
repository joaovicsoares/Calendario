using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventCalendar.Models;
using Newtonsoft.Json;

namespace EventCalendar.Repositories
{
    /// <summary>
    /// Implements event persistence using JSON file storage.
    /// Handles serialization, deserialization, and error recovery.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        /// <summary>
        /// Creates a new EventRepository with the specified file path.
        /// </summary>
        /// <param name="filePath">Path to the JSON file for storing events. Defaults to "events.json"</param>
        public EventRepository(string filePath = "events.json")
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Saves the entire list of events to the JSON file.
        /// Requirements: 5.2
        /// </summary>
        /// <param name="events">List of events to save</param>
        public void Save(List<Event> events)
        {
            lock (_lock)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(events, Formatting.Indented);
                    File.WriteAllText(_filePath, json);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Log error and notify that file permissions are denied (Requirement 5.3)
                    LogError($"Acesso negado ao salvar eventos: {ex.Message}");
                    throw new InvalidOperationException("Não foi possível salvar os eventos. Verifique as permissões do arquivo.", ex);
                }
                catch (IOException ex)
                {
                    // Log error for disk full or other IO issues (Requirement 5.3)
                    LogError($"Erro de I/O ao salvar eventos: {ex.Message}");
                    throw new InvalidOperationException("Erro ao salvar os eventos no disco.", ex);
                }
            }
        }

        /// <summary>
        /// Loads all events from the JSON file.
        /// Returns empty list if file doesn't exist, is corrupted, or inaccessible.
        /// Requirements: 5.1, 5.3
        /// </summary>
        /// <returns>List of events, or empty list on error</returns>
        public List<Event> Load()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        // File doesn't exist yet, return empty list
                        return new List<Event>();
                    }

                    string json = File.ReadAllText(_filePath);
                    
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        // Empty file, return empty list
                        return new List<Event>();
                    }

                    var events = JsonConvert.DeserializeObject<List<Event>>(json);
                    return events ?? new List<Event>();
                }
                catch (JsonException ex)
                {
                    // Corrupted file - log error and return empty list (Requirement 5.3)
                    LogError($"Arquivo de eventos corrompido: {ex.Message}");
                    return new List<Event>();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Permission denied - log error and return empty list (Requirement 5.3)
                    LogError($"Acesso negado ao carregar eventos: {ex.Message}");
                    return new List<Event>();
                }
                catch (IOException ex)
                {
                    // Other IO error - log and return empty list (Requirement 5.3)
                    LogError($"Erro de I/O ao carregar eventos: {ex.Message}");
                    return new List<Event>();
                }
            }
        }

        /// <summary>
        /// Adds a new event to storage with immediate persistence.
        /// Requirements: 1.4, 5.2
        /// </summary>
        /// <param name="eventItem">Event to add</param>
        public void Add(Event eventItem)
        {
            lock (_lock)
            {
                var events = Load();
                events.Add(eventItem);
                Save(events);
            }
        }

        /// <summary>
        /// Removes an event from storage with immediate persistence.
        /// Requirements: 3.2, 5.2
        /// </summary>
        /// <param name="eventId">ID of the event to remove</param>
        public void Remove(Guid eventId)
        {
            lock (_lock)
            {
                var events = Load();
                var eventToRemove = events.FirstOrDefault(e => e.Id == eventId);
                
                if (eventToRemove != null)
                {
                    events.Remove(eventToRemove);
                    Save(events);
                }
            }
        }

        /// <summary>
        /// Logs errors to a file for debugging and monitoring.
        /// </summary>
        /// <param name="message">Error message to log</param>
        private void LogError(string message)
        {
            try
            {
                string logPath = "event_calendar_errors.log";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // If logging fails, silently continue to avoid cascading errors
            }
        }
    }
}

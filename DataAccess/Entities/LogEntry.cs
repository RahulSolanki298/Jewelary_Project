using System;

namespace DataAccess.Entities
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime LogDate { get; set; }        // Timestamp of the log
        public string LogLevel { get; set; }         // Level of the log (e.g., INFO, ERROR)
        public string LogMessage { get; set; }       // The message or details of the log entry
        public string UserName { get; set; }         // User performing the action
        public string TableName { get; set; }        // Name of the table where action was performed
        public string ActionType { get; set; }       // Action performed (INSERT, UPDATE, DELETE)

    }
}
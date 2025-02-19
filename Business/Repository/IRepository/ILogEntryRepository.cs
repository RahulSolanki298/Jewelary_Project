using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace Business.Repository.IRepository
{
    public interface ILogEntryRepository
    {
        Task<bool> DeleteLogEntry(int id);

        Task<bool> SaveLogEntry(LogEntry logEntry);

        Task<List<LogEntry>> GetLogEntries();


        Task<LogEntry> GetLogEntryById(int id);
    }
}

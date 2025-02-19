using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Repository
{
    public class LogEntryRepository : ILogEntryRepository
    {
        private readonly ApplicationDBContext _context;
        public LogEntryRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<LogEntry> GetLogEntryById(int id)
        {
            try
            {
                var response = await _context.LogEntries.FindAsync(id);

                if (response != null)
                {
                    return response;
                }

                return new LogEntry();
            }
            catch (Exception)
            {
                return new LogEntry();
            }
        }


        public async Task<List<LogEntry>> GetLogEntries()
        {
            try
            {
                var response = await _context.LogEntries.ToListAsync();

                if (response != null)
                {
                    return response;
                }

                return new List<LogEntry>();
            }
            catch (Exception)
            {
                return new List<LogEntry>();
            }
        }

        public async Task<bool> SaveLogEntry(LogEntry logEntry)
        {
            try
            {
                if (logEntry.Id > 0)
                {
                    _context.LogEntries.Update(logEntry);
                }
                else
                {
                    var log = await _context.LogEntries.AddAsync(logEntry);
                }

                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> DeleteLogEntry(int id)
        {
            try
            {
                var response = await _context.LogEntries.FindAsync(id);

                if (response != null)
                {
                    _context.LogEntries.Remove(response);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<VirtualAppointment> VirtualAppointment { get; set; }
    }
}

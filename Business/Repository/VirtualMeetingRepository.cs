using AutoMapper;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class VirtualMeetingRepository : IVirtualMeetingRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILogEntryRepository _logRepository;

        public VirtualMeetingRepository(ApplicationDBContext context, IMapper mapper, ILogEntryRepository logRepository)
        {
            _context = context;
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<VirtualAppointmentDTO> CreateVirtualAppointment(VirtualAppointmentDTO appointment)
        {
            try
            {
                VirtualAppointment virtualAppointment = _mapper.Map<VirtualAppointmentDTO, VirtualAppointment>(appointment);
                var addVA = await _context.VirtualAppointment.AddAsync(virtualAppointment);
                await _context.SaveChangesAsync();

                return _mapper.Map<VirtualAppointment, VirtualAppointmentDTO>(addVA.Entity);
            }
            catch (Exception ex)
            {
                await LogException(ex, "VirtualAppointment");
                return new VirtualAppointmentDTO();
            }
        }

        public async Task<VirtualAppointmentDTO> EditVirtualAppointment(int Id, VirtualAppointmentDTO appointment)
        {
            try
            {
                var existingAppointment = await _context.VirtualAppointment.FindAsync(Id);
                if (existingAppointment == null)
                    return null;

                _mapper.Map(appointment, existingAppointment);
                _context.VirtualAppointment.Update(existingAppointment);
                await _context.SaveChangesAsync();

                return _mapper.Map<VirtualAppointment, VirtualAppointmentDTO>(existingAppointment);
            }
            catch (Exception ex)
            {
                await LogException(ex, "VirtualAppointment");
                return null;
            }
        }

        public async Task<IEnumerable<VirtualAppointmentDTO>> GetAllVirtualAppointment()
        {
            try
            {
                var appointments = await _context.VirtualAppointment.ToListAsync();
                return _mapper.Map<IEnumerable<VirtualAppointment>, IEnumerable<VirtualAppointmentDTO>>(appointments);
            }
            catch (Exception ex)
            {
                await LogException(ex, "VirtualAppointment");
                return new List<VirtualAppointmentDTO>();
            }
        }

        public async Task<VirtualAppointmentDTO> GetVirtualAppointmentById(int id)
        {
            try
            {
                var appointment = await _context.VirtualAppointment.FindAsync(id);
                if (appointment == null)
                    return null;

                return _mapper.Map<VirtualAppointment, VirtualAppointmentDTO>(appointment);
            }
            catch (Exception ex)
            {
                await LogException(ex, "VirtualAppointment");
                return null;
            }
        }

        public async Task<bool> DeleteVirtualAppointment(int id)
        {
            try
            {
                var appointment = await _context.VirtualAppointment.FindAsync(id);
                if (appointment == null)
                    return false;

                _context.VirtualAppointment.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await LogException(ex, "VirtualAppointment");
                return false;
            }
        }

        private async Task LogException(Exception ex, string tableName)
        {
            var logEntry = new LogEntry()
            {
                LogMessage = ex.Message,
                LogDate = DateTime.Now,
                ActionType = ex.Source,
                LogLevel = ex.InnerException?.Message ?? "Error",
                TableName = tableName
            };
            await _logRepository.SaveLogEntry(logEntry);
        }
    }

}

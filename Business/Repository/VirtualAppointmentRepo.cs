﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business.Repository.IRepository;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Business.Repository
{
    public class VirtualAppointmentRepo : IVirtualAppointmentRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogEntryRepository _logEntryRepository;
        private readonly IMapper _mapper;
        public VirtualAppointmentRepo(ApplicationDBContext context,
            ILogEntryRepository logEntryRepository, IMapper mapper)
        {
            _context = context;
            _logEntryRepository = logEntryRepository;
            _mapper = mapper;
        }

        #region Virtual Appointment
        /// <summary>
        /// Appointment Operations
        /// </summary>
        /// <param name="changeStatus"></param>
        /// <returns></returns>
        /// 
        public async Task<bool> ChangeStatusVirtualAppointment(ChangeStatusDTO changeStatus)
        {
            try
            {
                if (changeStatus.meetingId <= 0 || changeStatus.Status == null)
                {
                    return false;
                }

                var existingAppointment = await _context.VirtualAppointment
                       .FirstOrDefaultAsync(x => x.Id == changeStatus.meetingId);
                existingAppointment.Status = changeStatus.Status;
                existingAppointment.Message = changeStatus.Message;
                var responseDT = _context.VirtualAppointment.Update(existingAppointment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                var logDT = new LogEntry
                {
                    LogMessage = ex.Message,
                    LogDate = DateTime.Now,
                    LogLevel = LogLevel.Error.ToString(),
                };

                await _logEntryRepository.SaveLogEntry(logDT);

                return false;
            }
        }

        public async Task<VirtualAppointmentDTO> CreateVirtualAppointment(VirtualAppointmentDTO virtualAppointment)
        {
            try
            {
                var vpRequest = _mapper.Map<VirtualAppointmentDTO, VirtualAppointment>(virtualAppointment);

                var result = await _context.VirtualAppointment.AddAsync(vpRequest);
                await _context.SaveChangesAsync();

                return _mapper.Map<VirtualAppointment, VirtualAppointmentDTO>(result.Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while creating virtual appointment.", ex);
            }
        }

        public async Task<int> DeleteVirtualAppointmentById(int virtualId)
        {
            var vaRequest = await _context.VirtualAppointment.FindAsync(virtualId);
            if (vaRequest != null)
            {
                _context.VirtualAppointment.Remove(vaRequest);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<VirtualAppointmentDTO> GetVirtualAppointmentById(int virtualId)
        {
            if (virtualId <= 0)
            {
                // Return null immediately if the virtualId is invalid
                return null;
            }

            try
            {

                var virtualAppointment = await (from va in _context.VirtualAppointment
                                                join cat in _context.Category on va.CategoryId equals cat.Id
                                                select new VirtualAppointmentDTO
                                                {
                                                    Id = va.Id,
                                                    CategoryId = va.CategoryId,
                                                    CategoryName = cat.Name,
                                                    CompanyName = va.CompanyName,
                                                    EmailId = va.EmailId,
                                                    FirstName = va.FirstName,
                                                    LastName = va.LastName,
                                                    Message = va.Message,
                                                    RegisterDate = va.RegisterDate,
                                                    RegisterTime = va.RegisterTime,
                                                    Status = va.Status
                                                }).FirstOrDefaultAsync(x => x.Id == virtualId);

                // If no data found, return null
                if (virtualAppointment == null)
                {
                    return null;
                }

                // Return the mapped DTO
                return virtualAppointment;
            }
            catch (Exception ex)
            {
                // Log the exception properly (including a log level)
                var logDT = new LogEntry
                {
                    LogMessage = ex.Message,
                    LogDate = DateTime.Now,
                    LogLevel = LogLevel.Error.ToString(), // Set a log level (Error)
                };

                // Save log entry asynchronously
                await _logEntryRepository.SaveLogEntry(logDT);

                // Instead of returning an empty DTO, you may return null to indicate failure
                return null;
            }
        }

        public async Task<IEnumerable<VirtualAppointmentDTO>> GetVirtualAppointmentList()
        {
            try
            {
                var virtualAppointments = await (from va in _context.VirtualAppointment
                                                 join cat in _context.Category on va.CategoryId equals cat.Id
                                                 select new VirtualAppointmentDTO
                                                 {
                                                     Id = va.Id,
                                                     CategoryId = va.CategoryId,
                                                     CategoryName = cat.Name,
                                                     CompanyName = va.CompanyName,
                                                     EmailId = va.EmailId,
                                                     FirstName = va.FirstName,
                                                     LastName = va.LastName,
                                                     Message = va.Message,
                                                     RegisterDate = va.RegisterDate,
                                                     RegisterTime = va.RegisterTime,
                                                     Status = va.Status
                                                 }).ToListAsync();


                return virtualAppointments;
            }
            catch (Exception ex)
            {
                // Log the exception
                var logDT = new LogEntry
                {
                    LogMessage = ex.Message,
                    LogDate = DateTime.Now,
                    LogLevel = LogLevel.Error.ToString(),
                };

                await _logEntryRepository.SaveLogEntry(logDT);

                // Return an empty list or handle as needed
                return new List<VirtualAppointmentDTO>();
            }
        }

        public async Task<VirtualAppointmentDTO> UpdateVirtualAppointmentById(int virtualId, VirtualAppointmentDTO virtualAppointmentDTO)
        {
            if (virtualId <= 0 || virtualAppointmentDTO == null)
            {
                // Handle invalid input early
                return null;  // You could return a different response indicating invalid input
            }

            try
            {
                // Retrieve the existing appointment by id
                var existingAppointment = await _context.VirtualAppointment
                    .FirstOrDefaultAsync(x => x.Id == virtualId);

                // If the appointment doesn't exist, return null or an appropriate response
                if (existingAppointment == null)
                {
                    return null; // Appointment not found
                }

                // Map the updated data from DTO to the existing entity (you might want to exclude fields that shouldn't be updated)
                _mapper.Map(virtualAppointmentDTO, existingAppointment);

                // Optionally, you could validate the appointment data here (e.g., check for required fields)

                // Save the changes to the database
                _context.VirtualAppointment.Update(existingAppointment);
                await _context.SaveChangesAsync();

                // Return the updated VirtualAppointmentDTO
                return _mapper.Map<VirtualAppointment, VirtualAppointmentDTO>(existingAppointment);
            }
            catch (Exception ex)
            {
                // Log the exception (same as previously explained)
                var logDT = new LogEntry
                {
                    LogMessage = ex.Message,
                    LogDate = DateTime.Now,
                    LogLevel = LogLevel.Error.ToString(),
                };

                await _logEntryRepository.SaveLogEntry(logDT);

                // Handle the exception (you might return a null DTO or an error DTO)
                return null; // Return null to indicate failure
            }
        }

        #endregion

        #region Virtual Appointment Meeting
        ///Meeting Schedual

        public async Task<AcceptedVirtualAppointmentData> GetVirtualAppointmentDataById(int id)
        {
            try
            {
                var data = await _context.AcceptedVirtualAppointmentData.FirstOrDefaultAsync(x => x.Id == id);

                return data;
            }
            catch (Exception ex)
            {

                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.Now,
                    ActionType = "Post",
                    TableName = "VirtualAppointmentData",
                    LogMessage = ex.InnerException.Message
                });

                return new AcceptedVirtualAppointmentData();
            }
        }

        public async Task<List<AcceptedVirtualAppointmentData>> GetVirtualAppointmentDataList()
        {
            var result = await _context.AcceptedVirtualAppointmentData.ToListAsync();
            return result;
        }

        public async Task<bool> CreateVirtualAppointmentData(AcceptedVirtualAppointmentData appointmentData)
        {
            try
            {
                var result = await _context.AcceptedVirtualAppointmentData.AddAsync(appointmentData);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.Now,
                    ActionType = "Post",
                    TableName = "VirtualAppointmentData",
                    LogMessage = ex.InnerException.Message,
                });
                return false;
            }
        }

        public async Task<bool> UpdateVirtualAppointmentData(int id, AcceptedVirtualAppointmentData appointmentData)
        {
            try
            {
                var result = await _context.AcceptedVirtualAppointmentData.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    result.FirstName = appointmentData.FirstName;
                    result.LastName = appointmentData.LastName;
                    result.CompanyName = appointmentData.CompanyName;
                    result.Designation = appointmentData.Designation;
                    result.MeetingDate = appointmentData.MeetingDate;
                    result.MeetingTime = appointmentData.MeetingTime;
                    result.MeetingWith = appointmentData.MeetingWith;
                    result.MeetingDescription = appointmentData.MeetingDescription;
                    result.EmployeeId = appointmentData.EmployeeId;
                    result.MeetingUrl = appointmentData.MeetingUrl;
                    result.IsActived = appointmentData.IsActived;
                    _context.AcceptedVirtualAppointmentData.Update(result);
                    await _context.SaveChangesAsync();

                    return true;

                }

                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteVirtualAppointmentDataById(int id)
        {
            try
            {
                var data = await _context.AcceptedVirtualAppointmentData.FirstOrDefaultAsync(x => x.Id == id);

                if (data != null)
                {
                    _context.AcceptedVirtualAppointmentData.Remove(data);
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.Now,
                    ActionType = "Post",
                    TableName = "VirtualAppointmentData",
                    LogMessage = ex.InnerException.Message,
                });

                return false;
            }
        }

        /// 

        public async Task<bool> AddReviewsByMeeting(int meetingId,int star,string comment)
        {
            try
            {
                if (meetingId > 0)
                {
                    var meetingDT = await _context.AcceptedVirtualAppointmentData.FirstOrDefaultAsync(x=>x.Id==meetingId);
                    meetingDT.NoOfStar = star;
                    meetingDT.Comment = comment;
                    _context.AcceptedVirtualAppointmentData.Update(meetingDT);
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.Now,
                    ActionType = "Post",
                    TableName = "AcceptedVirtualAppointmentData",
                    LogMessage = ex.InnerException.Message,
                });

                return false;
            }
        }


        #endregion
    }
}

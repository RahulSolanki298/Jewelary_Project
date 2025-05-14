using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace B2C_ECommerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VirtualAppointmentController : ControllerBase
    {
        private readonly IVirtualAppointmentRepo _virtualAppointmentRepo;
        private readonly ILogEntryRepository _logEntryRepository;
        public VirtualAppointmentController(IVirtualAppointmentRepo virtualAppointment,
            ILogEntryRepository logEntryRepository)
        {
            _virtualAppointmentRepo = virtualAppointment;
            _logEntryRepository = logEntryRepository;
        }

        #region Virtual Appointment
        [HttpGet("appointment-list")]
        public async Task<ActionResult> GetVirtualAppointments()
        {
            try
            {
                var appointmentList = await _virtualAppointmentRepo.GetVirtualAppointmentList();

                if (appointmentList.Count() == 0)
                {
                    return NotFound("Virtual Appointment List doesn't found.");
                }

                return Ok(appointmentList);
            }
            catch (Exception ex)
            {

                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }

        [HttpGet("appointment-by-id/{id}")]
        public async Task<ActionResult> GetVirtualAppointmentById(int id)
        {
            try
            {
                var appointmentList = await _virtualAppointmentRepo.GetVirtualAppointmentById(id);

                if (appointmentList == null)
                {
                    return NotFound("Virtual Appointment List doesn't found.");
                }

                return Ok(appointmentList);
            }
            catch (Exception ex)
            {

                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }

        [HttpPost("add-appointment")]
        public async Task<ActionResult> AddVirtualAppointment([FromBody] VirtualAppointmentDTO virtualAppointment)
        {
            if (virtualAppointment == null)
            {
                return BadRequest("Invalid virtual appointment data.");
            }

            try
            {
                var vaResponse = new VirtualAppointmentDTO
                {
                    CategoryId = virtualAppointment.CategoryId,
                    CompanyName = virtualAppointment.CompanyName,
                    EmailId = virtualAppointment.EmailId,
                    LastName = virtualAppointment.LastName,
                    FirstName = virtualAppointment.FirstName,
                    Message = virtualAppointment.Message,
                    RegisterDate = virtualAppointment.RegisterDate.HasValue
                                    ? Convert.ToDateTime(virtualAppointment.RegisterDate)
                                    : (DateTime?)null,
                    RegisterTime = virtualAppointment.RegisterTime,
                    Status = virtualAppointment.Status
                };

                var response = await _virtualAppointmentRepo.CreateVirtualAppointment(vaResponse).ConfigureAwait(false);

                if (response == null)
                {
                    return BadRequest("Failed to create virtual appointment.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.UtcNow,
                    ActionType = nameof(AddVirtualAppointment),
                    LogMessage = ex.Message,
                    LogLevel = "Error",
                    TableName = "VirtualAppointment",
                    //UserName = User.
                }).ConfigureAwait(false);

                return StatusCode(500, "An error occurred while creating the virtual appointment.");
            }
        }

        [HttpPost("update-appointment/{virtualId}")]
        public async Task<ActionResult> UpdateVirtualAppointment(int virtualId, VirtualAppointmentDTO virtualAppointment)
        {
            try
            {
                if (virtualId == 0)
                {
                    return BadRequest("Virtual appointment doesn't found. please check your appointment.");
                }

                if (virtualAppointment == null)
                {
                    return BadRequest("Invalid virtual appointment data.");
                }

                var response = await _virtualAppointmentRepo.UpdateVirtualAppointmentById(virtualId, virtualAppointment);

                if (response == null)
                {
                    return BadRequest("Failed to create virtual appointment.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.UtcNow,
                    ActionType = nameof(AddVirtualAppointment),
                    LogMessage = ex.Message,
                    LogLevel = "Error",
                    TableName = "VirtualAppointment",
                    //UserName = User.
                }).ConfigureAwait(false);

                return StatusCode(500, "An error occurred while creating the virtual appointment.");
            }
        }

        [HttpPost("change-appointment-status")]
        public async Task<ActionResult> ChangeStatusForAppointment(ChangeStatusDTO changeStatus)
        {
            try
            {
                if (changeStatus.meetingId == 0)
                {
                    return BadRequest("Virtual appointment doesn't found. please check your appointment.");
                }

                var response = await _virtualAppointmentRepo.ChangeStatusVirtualAppointment(changeStatus);

                if (response == null)
                {
                    return BadRequest("Failed to create virtual appointment.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logEntryRepository.SaveLogEntry(new LogEntry
                {
                    LogDate = DateTime.UtcNow,
                    ActionType = nameof(AddVirtualAppointment),
                    LogMessage = ex.Message,
                    LogLevel = "Error",
                    TableName = "VirtualAppointment",
                    //UserName = User.
                }).ConfigureAwait(false);

                return StatusCode(500, "An error occurred while creating the virtual appointment.");
            }
        }

        [HttpDelete("delete-appointment-by-id")]
        public async Task<ActionResult> DeleteVirtualAppointmentById(int id)
        {
            try
            {
                var response = await _virtualAppointmentRepo.DeleteVirtualAppointmentById(id);

                if (response == 0)
                {
                    return BadRequest($"Appointment doesn't found.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }

        [HttpGet("appointment-status-list")]
        public ActionResult GetAppointmentStatus()
        {
            var statusList = new List<string>()
            {
                "Meeting-Request",
                "Accept-Meeting-Request",
                "Reject-Meeting-Request",
                "Hold-Meeting-Request",
                "Pending-Meeting-Request",
                "Reschedule-Meeting-Request",
                "Cancel-Meeting-Request",
                "Complate-Meeting-Request"
            };

            return Ok(statusList);
        }

        #endregion

        #region Virtual Appointment Meeting


        [HttpGet("appointment-data-list")]
        public async Task<ActionResult> GetVirtualAppointmentDataList()
        {
            try
            {
                var appointmentList = await _virtualAppointmentRepo.GetVirtualAppointmentDataList();

                if (appointmentList.Count() == 0)
                {
                    return NotFound("Virtual appointment data List doesn't found.");
                }

                return Ok(appointmentList);
            }
            catch (Exception ex)
            {

                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }

        [HttpGet("appointment-data/{id}")]
        public async Task<ActionResult> GetVirtualAppointmentData(int id)
        {
            try
            {
                var appointmentList = await _virtualAppointmentRepo.GetVirtualAppointmentDataById(id);

                if (appointmentList == null)
                {
                    return NotFound("Virtual appointment data List doesn't found.");
                }

                return Ok(appointmentList);
            }
            catch (Exception ex)
            {

                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }
        
        [HttpPost("generate-virtual-appointment-data")]
        public async Task<ActionResult> GenerateVirtualAppointmentData([FromBody] AcceptedVirtualAppointmentData virtualAppointment)
        {
            try
            {
                if (virtualAppointment == null)
                {
                    return NotFound("Virtual appointment doesn't found.");
                }

                var response = await _virtualAppointmentRepo.CreateVirtualAppointmentData(virtualAppointment);

                if (response == false)
                {
                    return BadRequest("Invalid virtual appointmetn data. please try again");
                }

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid virtual appointment data. Exception : {ex.InnerException.Message}");
            }
        }

        [HttpPost("update-virtual-appointment-data/{id}")]
        public async Task<ActionResult> UpdateVirtualAppointmentData(int id, AcceptedVirtualAppointmentData virtualAppointment)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound("Virtual appointment doesn't found.");
                }

                var response = await _virtualAppointmentRepo.CreateVirtualAppointmentData(virtualAppointment);

                if (response == false)
                {
                    return BadRequest("Invalid virtual appointmetn data. please try again");
                }

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid virtual appointment data. Exception : {ex.InnerException.Message}");
            }
        }

        [HttpPost("add-comment-virtual-appointment-data/{id}/star/{star}/comment/{comment}")]
        public async Task<ActionResult> AddCommentVirtualAppointmentData(int id, int star, string comment)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound("Virtual appointment data doesn't found.");
                }

                var response = await _virtualAppointmentRepo.AddReviewsByMeeting(id, star, comment);

                if (response == false)
                {
                    return BadRequest("Invalid virtual appointmetn data. please try again");
                }

                return Ok("Review added successfully.");

            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid virtual appointment data. Exception : {ex.InnerException.Message}");
            }
        }

        [HttpDelete("delete-appointment-data/{id}")]
        public async Task<ActionResult> DeleteVirtualAppointmentData(int id)
        {
            try
            {
                var result = await _virtualAppointmentRepo.DeleteVirtualAppointmentDataById(id);
                if (result)
                {
                    return Ok("appointment has been deleted successfully.");
                }
                return NotFound("appointment is not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Bad Request, Method returns exception : {ex.Message}");
            }
        }
        #endregion
    }
}

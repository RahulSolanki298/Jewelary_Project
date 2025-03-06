using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VirtualAppointmentController : ControllerBase
    {
        private readonly IVirtualAppointmentRepo _virtualAppointmentRepo;
        public VirtualAppointmentController(IVirtualAppointmentRepo virtualAppointment)
        {
            _virtualAppointmentRepo = virtualAppointment;
        }

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

        [HttpGet("appointment-list")]
        public async Task<ActionResult> GetVirtualAppointment()
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


    }
}

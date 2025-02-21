using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IVirtualMeetingRepository
    {
        Task<VirtualAppointmentDTO> CreateVirtualAppointment(VirtualAppointmentDTO appointment);
        Task<VirtualAppointmentDTO> EditVirtualAppointment(int Id,VirtualAppointmentDTO appointment);
        Task<IEnumerable<VirtualAppointmentDTO>> GetAllVirtualAppointment();
        Task<VirtualAppointmentDTO> GetVirtualAppointmentById(int id);
        Task<bool> DeleteVirtualAppointment(int id);

    }
}

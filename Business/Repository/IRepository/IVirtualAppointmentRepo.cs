using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Business.Repository.IRepository
{
    public interface IVirtualAppointmentRepo
    {
        public Task<VirtualAppointmentDTO> GetVirtualAppointmentById(int virtualId);
        public Task<IEnumerable<VirtualAppointmentDTO>> GetVirtualAppointmentList();
        public Task<VirtualAppointmentDTO> CreateVirtualAppointment(VirtualAppointmentDTO virtualAppointment);
        public Task<VirtualAppointmentDTO> UpdateVirtualAppointmentById(int virtualId,VirtualAppointmentDTO virtualAppointment);
        public Task<int> DeleteVirtualAppointmentById(int virtualId);

    }
}

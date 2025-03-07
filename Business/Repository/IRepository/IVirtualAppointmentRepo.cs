using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Models;

namespace Business.Repository.IRepository
{
    public interface IVirtualAppointmentRepo
    {
        public Task<VirtualAppointmentDTO> GetVirtualAppointmentById(int virtualId);
        public Task<IEnumerable<VirtualAppointmentDTO>> GetVirtualAppointmentList();
        public Task<VirtualAppointmentDTO> CreateVirtualAppointment(VirtualAppointmentDTO virtualAppointment);
        public Task<VirtualAppointmentDTO> UpdateVirtualAppointmentById(int virtualId, VirtualAppointmentDTO virtualAppointment);
        public Task<int> DeleteVirtualAppointmentById(int virtualId);
        public Task<bool> ChangeStatusVirtualAppointment(ChangeStatusDTO changeStatus);


        public Task<AcceptedVirtualAppointmentData> GetVirtualAppointmentDataById(int id);

        public Task<List<AcceptedVirtualAppointmentData>> GetVirtualAppointmentDataList();

        public Task<bool> UpdateVirtualAppointmentData(int id, AcceptedVirtualAppointmentData appointmentData);
        
        public Task<bool> CreateVirtualAppointmentData(AcceptedVirtualAppointmentData appointmentData);

        public Task<bool> DeleteVirtualAppointmentDataById(int id);

        public Task<bool> AddReviewsByMeeting(int meetingId, int star, string comment);
    }
}

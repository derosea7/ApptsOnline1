
using Appts.Models.Document;
using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public interface IAppointmentRepository
  {
    Task<GetAppointmentDetailResponse> GetAppointmentToCancel(
  GetAppointmentDetailRequest request);

    // have to return cancelled appt to remove from gcal
    Task<CancelAppointmentResponse> CancelAppointmentAsync(CancelAppointmentRequest request);
    Task<ViewAppointmentsResponse> GetViewAppointmentsAsync(
      ViewAppointmentsRequest request);

    Task<Appointment> GetAppointmentAsync(string userId, string apptId);
    //Task Reschedule(RescheduleAppointmentRequest request);
  }
}

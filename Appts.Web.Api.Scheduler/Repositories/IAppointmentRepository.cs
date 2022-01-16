using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Models.Rest;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IAppointmentRepository
  {
    Task<List<Appointment>> GetAppointmentsByProviderAsync(
      string providerUserId, DateTime start, DateTime end,
      string typeId = null, string status = null);
    //multiple providers, for clients
    Task<List<Appointment>> GetAppointmentsByProvidersAsync(
      string providerUserId, DateTime start, DateTime end,
      string typeId = null, string status = null);

    Task AddAppointmentAsync(Appointment appointment);
    Task RescheduleAppointmentAsync(RescheduleAppointmentRequest request);
    Task<Appointment> GetAppointmentAsync(string serviceProviderId, string appointmentId);
    Task<GetAppointmentDetailResponse> GetAppointmentToCancelAsync(
      GetAppointmentDetailRequest request);
    Task<CancelAppointmentResponse> CancelAppointmentAsync(CancelAppointmentRequest request);
    Task UpdateReadRecieptNoReturnAsync(Appointment appointment, UserRole userRole);
  }
}

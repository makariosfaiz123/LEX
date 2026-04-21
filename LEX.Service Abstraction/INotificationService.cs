using LEX.Domain.EnumS;
using LEX.Shared.DTOs.Notifications;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int userId, string title, string message, NotificationType type);

        Task<ServiceResponse<List<NotificationDto>>> GetMyNotificationsAsync(int userId);
    }
}

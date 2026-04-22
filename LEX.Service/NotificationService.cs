using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs.Notifications;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddNotificationAsync(int userId, string title, string message, NotificationType type)
        {
            var repo = _unitOfWork.GetRepository<Notification, int>();

            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type
            };

            await repo.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ServiceResponse<List<NotificationDto>>> GetMyNotificationsAsync(int userId)
        {
            var repo = _unitOfWork.GetRepository<Notification, int>();

            var data = await repo.GetAsync(
                n => n.UserId == userId,
                q => q.OrderByDescending(n => n.CreatedAt)
            );

            var result = data.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type.ToString(),
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();

            return ServiceResponse<List<NotificationDto>>.SuccessResult(result);
        }
    }
}

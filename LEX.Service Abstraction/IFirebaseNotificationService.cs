using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction
{
    public interface IFirebaseNotificationService
    {
        Task<bool> SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);
    }
}

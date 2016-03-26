using System;

namespace Anvil.Services
{
    public interface INotificationService
    {
        void ShowError(string message, Exception exception);
    }
}

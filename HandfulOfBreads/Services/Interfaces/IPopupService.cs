using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandfulOfBreads.Services.Interfaces
{
    public interface IPopupService
    {
        Task ShowPopupAsync(Popup popup);
        Task<T?> ShowPopupAsync<T>(Popup popup);
    }
}

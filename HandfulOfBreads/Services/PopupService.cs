using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandfulOfBreads.Services
{
    public class PopupService : IPopupService
    {
        public async Task ShowPopupAsync(Popup popup)
        {
            await Application.Current.MainPage.ShowPopupAsync(popup);
        }

        public async Task<T?> ShowPopupAsync<T>(Popup popup)
        {
            return (T?) await Application.Current.MainPage.ShowPopupAsync(popup);
        }
    }
}

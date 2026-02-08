using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace PeterTours.Utils
{
    public static class NavigationHelper
    {
        private static bool _isNavigating = false;

        public static async Task SafePushAsync(INavigation navigation, Page page)
        {
            if (_isNavigating) return; // si ya está navegando, ignorar
            _isNavigating = true;
            try
            {
                await navigation.PushAsync(page);
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}

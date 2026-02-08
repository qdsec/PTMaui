using Acr.UserDialogs;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step2ConfirmaPasajero : ContentPage
    {
        public step2ConfirmaPasajero()
        {
            InitializeComponent();
            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");
        }

        private async void btnNext_Clicked(object sender, EventArgs e)
        {

            LoadingService.Show("Cargando");
            await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());

            LoadingService.Hide();
        }

        private async void btnContinuar_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("viajeTitular","1");
            await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());
        }

        private async void btnAgregarPasajero_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("viajeTitular","0");
            await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajeroOtro());
        }
    }
}
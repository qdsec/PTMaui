using PeterTours.Utils;
using PeterTours.Views.ViajeComp;
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
    public partial class step2ConfirmarParrilla : ContentPage
    {
        public static int? necesitaParrilla = null;
        public step2ConfirmarParrilla()
        {
            InitializeComponent();
        }

        private async void rbSi_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                await DisplayAlert("¡Excelente decisión!",
                    "Realiza tu traslado con todo lo que necesites usando la parrilla por $10 y disfruta tu viaje sin preocuparte por el espacio.",
                    "Aceptar");
            }
        }
        private async void btnNext_Clicked(object sender, EventArgs e)
        {

            if (rbSi.IsChecked)
                necesitaParrilla = 1;
            else if (rbNo.IsChecked)
                necesitaParrilla = 0;

            if (necesitaParrilla == null)
            {
                await DisplayAlert("Aviso", "Por favor selecciona una opción antes de continuar.", "Aceptar");
                return;
            }

             await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajeroOtro());
        }
    }
}
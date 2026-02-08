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
    public partial class step5Cuentas : ContentPage
    {
        public step5Cuentas()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");
        }

        private void chbConfirmar_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //if (chbConfirmar.IsChecked == true)
            //{
            //    DisplayAlert("Atención","Me comprometo a realizar el viaje en la fecha y hora seleccionada", "Acepto");
            //    btnRegresar.IsEnabled = true;
            //}
            //else
            //{
            //    btnRegresar.IsEnabled = false;
            //}
            //"Gracias por usar Peter Tours, el conductor se comunicará con usted una hora antes del viaje"
        }
        protected override bool OnBackButtonPressed()
        {

            return true;
        }

        private void btnRegresar_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");


            Navigation.PushAsync(new Mapa());
            LoadingService.Hide();
        }
    }
}
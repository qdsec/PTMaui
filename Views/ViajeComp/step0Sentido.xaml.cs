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
    public partial class step0Sentido : ContentPage
    {
        public static string tipoViaje = "";
        // Flag para bloquear recursión
        private bool _isUpdatingPickers = false;

        // Listas y grupos
        List<string> listaCompleta = new List<string> {
        "Quito", "Ibarra", "Atuntaqui", "Otavalo", "Cotacachi", "Aeropuerto"
    };
        HashSet<string> grupoA = new HashSet<string> { "Ibarra", "Atuntaqui", "Otavalo", "Cotacachi" };
        HashSet<string> grupoB = new HashSet<string> { "Quito", "Aeropuerto" };
        HashSet<string> grupoC = new HashSet<string> { "Quito","Ibarra", "Atuntaqui", "Otavalo", "Cotacachi" };
        HashSet<string> grupoD = new HashSet<string> { "Aeropuerto", "Ibarra", "Atuntaqui", "Otavalo", "Cotacachi" };

        string tipoServicio = Preferences.Get("tipo","0").ToString(); 
        public step0Sentido()
        {
            InitializeComponent();
            // Listas con todos los orígenes



            piViajeIda.ItemsSource = listaCompleta;
            piViajeRegreso.ItemsSource = listaCompleta;

            //string tipoServicio = Application.Current.Properties["tipo"].ToString();
            //string tipoServicio = "3";
            if (tipoServicio == "3")
            {
               lblSentido.Text = "Seleccione el sentido la encomienda";
            }
            else
            {
                lblSentido.Text = "Seleccione el sentido de su viaje";
            }
            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");
        }
        private void PiViajeIda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingPickers || piViajeIda.SelectedIndex == -1)
                return;

            _isUpdatingPickers = true;

            // 1) Lo que el usuario acaba de elegir en Ida
            string seleccionadoIda = piViajeIda.SelectedItem.ToString();

            // 2) Guarda la selección actual de Regreso
            string regresoPrevio = piViajeRegreso.SelectedItem as string;

            // 3) Filtra el ItemsSource de Regreso
            if (seleccionadoIda== "Aeropuerto" && tipoServicio == "2")
                piViajeRegreso.ItemsSource = grupoC.ToList();
            else if (seleccionadoIda == "Quito" && tipoServicio == "2")
                piViajeRegreso.ItemsSource = grupoD.ToList();
            else if (grupoA.Contains(seleccionadoIda))
                piViajeRegreso.ItemsSource = grupoB.ToList();
            else if (grupoB.Contains(seleccionadoIda))
                piViajeRegreso.ItemsSource = grupoA.ToList();
            else
                piViajeRegreso.ItemsSource = listaCompleta;

            // 4) Si el valor guardado sigue existiendo, restaurarlo
            if (regresoPrevio != null && piViajeRegreso.ItemsSource.Cast<string>().Contains(regresoPrevio))
                piViajeRegreso.SelectedItem = regresoPrevio;
            // (sino no tocamos nada: deja la que había o queda sin seleccionar)

            _isUpdatingPickers = false;
        }

        private void PiViajeRegreso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingPickers || piViajeRegreso.SelectedIndex == -1)
                return;

            _isUpdatingPickers = true;

            // 1) Lo que el usuario acaba de elegir en Regreso
            string seleccionadoRegreso = piViajeRegreso.SelectedItem.ToString();

            // 2) Guarda la selección actual de Ida
            string idaPrevio = piViajeIda.SelectedItem as string;

            // 3) Filtra el ItemsSource de Ida
            if ((idaPrevio == "Aeropuerto"|| idaPrevio == "Quito") && tipoServicio == "2")
                piViajeIda.ItemsSource = grupoB.ToList();
            else if (grupoA.Contains(seleccionadoRegreso))
                piViajeIda.ItemsSource = grupoB.ToList();
            else if (grupoB.Contains(seleccionadoRegreso))
                piViajeIda.ItemsSource = grupoA.ToList();
            else
                piViajeIda.ItemsSource = listaCompleta;

            // 4) Si el valor guardado sigue existiendo, restaurarlo
            if (idaPrevio != null && piViajeIda.ItemsSource.Cast<string>().Contains(idaPrevio))
                piViajeIda.SelectedItem = idaPrevio;

            _isUpdatingPickers = false;
        }


        //private async void btnNext_Clicked(object sender, EventArgs e)
        //{
        //    LoadingService.Show("Cargando");

        //    if (piViajeIda.SelectedIndex != -1 && piViajeRegreso.SelectedIndex != -1)
        //    {
        //        LabelError.Text = "";

        //        // Concatenamos los valores seleccionados con un guion
        //        tipoViaje = $"{piViajeIda.SelectedItem}-{piViajeRegreso.SelectedItem}";

        //        // Navegamos a la siguiente página
        //        await NavigationHelper.SafePushAsync(Navigation, new step1Fecha());
        //    }
        //    else
        //    {
        //        LabelError.Text = "Favor seleccione el sentido";
        //    }

        //    LoadingService.Hide();
        //}
        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            if (piViajeIda.SelectedIndex == -1 || piViajeRegreso.SelectedIndex == -1)
            {
                LabelError.Text = "Favor seleccione el sentido";
                return; // Detener la ejecución si no es válido
            }

            LabelError.Text = "";
            tipoViaje = $"{piViajeIda.SelectedItem}-{piViajeRegreso.SelectedItem}";

            LoadingService.Show("Cargando");

            await Task.Delay(50);

            try
            {
                await Task.Run(async () =>
                {
                    var nextPage = new step1Fecha();

                    await Dispatcher.DispatchAsync(async () =>
                    {
                        await NavigationHelper.SafePushAsync(Navigation, nextPage);
                    });
                });
            }
            finally
            {
                LoadingService.Hide();
            }
        }
    }
}
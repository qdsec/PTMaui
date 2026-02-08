using Acr.UserDialogs;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
//using Java.Lang;
using Newtonsoft.Json;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static PeterTours.libreriaDatos;

namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class viajeSeleccionado : ContentPage 
    {
        //public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public ICommand VerDetalleCommand { get; private set; }

        public ICommand TapCommand { get; set; }
        public string DynamicUrl { get; set; }
        public static int id_cupon = 0;
        public static string tipo = "0";
        public ObservableCollection<string> Meses { get; set; } = new();
        public ObservableCollection<string> Anios { get; set; } = new();

        private int mesSeleccionado;
        private int anioSeleccionado;


        public ObservableCollection<CitasOutputXCedula> Viajes { get; set; } = new();

        public viajeSeleccionado()
        {
            InitializeComponent();
            BindingContext = this;
            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");
            VerDetalleCommand = new Command<CitasOutputXCedula>(async (viaje) =>
            {
                if (viaje == null) return;
                await NavigationHelper.SafePushAsync(
                    Navigation,
                    new ViajeDetallePage(viaje)
                );
            });

            CargarMeses();
            CargarAnios();

            mesSeleccionado = DateTime.Now.Month;
            anioSeleccionado = DateTime.Now.Year;
        }

        private void CargarMeses()
        {
            Meses.Clear();

            var cultura = new CultureInfo("es-EC");
            for (int i = 1; i <= 12; i++)
                Meses.Add(cultura.DateTimeFormat.GetMonthName(i).ToUpper());
        }

        private void CargarAnios()
        {
            Anios.Clear();

            int anioActual = DateTime.Now.Year;
            for (int i = anioActual; i >= anioActual - 5; i--)
                Anios.Add(i.ToString());
        }
        private async void OnFiltroChanged(object sender, EventArgs e)
        {
            if (pkMes.SelectedIndex == -1 || pkAnio.SelectedIndex == -1)
                return;

            mesSeleccionado = pkMes.SelectedIndex + 1;
            anioSeleccionado = int.Parse(Anios[pkAnio.SelectedIndex]);

            LoadingService.Show("Cargando viajes...");
            await CargarViajes(mesSeleccionado, anioSeleccionado);
            LoadingService.Hide();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            pkMes.SelectedIndex = mesSeleccionado - 1;
            pkAnio.SelectedIndex = Anios.IndexOf(anioSeleccionado.ToString());

            LoadingService.Show("Cargando viajes...");
            await CargarViajes(mesSeleccionado, anioSeleccionado);
            LoadingService.Hide();
        }

        private async Task CargarViajes(int mes, int anio)
        {
            try
            {
                string cedula = Preferences.Get("ci", "");
                string mesStr = mes.ToString("D2");

                string url = $"http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/citas-x-cedula/{mesStr}/{anio}/{cedula}";

                using HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.StatusCode != HttpStatusCode.OK)
                    return;

                string content = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<CitasOutputXCedula>>(content);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Viajes.Clear();

                    foreach (var item in resultado)
                        Viajes.Add(item);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        private async void OnViajeSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var viaje = e.CurrentSelection[0] as CitasOutputXCedula;
            if (viaje == null)
                return;

            var collection = (CollectionView)sender;
            collection.SelectedItem = null;

            await NavigationHelper.SafePushAsync(Navigation, new ViajeDetallePage(viaje));
        }




    }
}
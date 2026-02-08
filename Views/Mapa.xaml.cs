using Acr.UserDialogs;
//using Java.Util;
using Newtonsoft.Json;
using PeterTours.Utils;
using PeterTours.Views.ViajeComp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Mapa : ContentPage
    {
        private string nombreUsuario;
        private string IsLoggedIn;
        public string mascota = "";
        public bool solicitudActiva=false;
        public List<libreriaDatos.QExpressData> resultado;
        public Mapa()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            nombreUsuario = Preferences.Get("name", string.Empty).ToString();
            IsLoggedIn = Preferences.Get("IsLoggedIn", "0").ToString();
            if (IsLoggedIn != "1")
            {
                NavigationHelper.SafePushAsync(Navigation, new LoginPage());
            }
            OnAppearing();
        }

        private async void btnViajeCom_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            Preferences.Set("tipo", "1");
            await NavigationHelper.SafePushAsync(Navigation, new ViajeComp.step0Sentido());
            LoadingService.Hide();

        }

        private async void btnViajeVIP_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            Preferences.Set("tipo", "2");
            //bool activaExpress = true;
            //var request = new HttpRequestMessage();
            //request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/QExpressOutputs");
            //request.Method = HttpMethod.Get;
            //request.Headers.Add("Accept", "application/json");
            //var client = new HttpClient();
            //HttpResponseMessage response = await client.SendAsync(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    string content = await response.Content.ReadAsStringAsync();
            //    resultado = JsonConvert.DeserializeObject<List<libreriaDatos.QExpressData>>(content);
            //    foreach (var item in resultado)
            //    {
            //        if(item.qhs_fecha.Day==DateTime.Now.Day&& item.qhs_fecha.Month == DateTime.Now.Month && item.qhs_fecha.Year == DateTime.Now.Year&&item.qhs_estado==1)
            //        {
            //            await DisplayAlert("Aviso", "Los viajes Express de hoy han terminado, favor intente nuevamente el día de mañana ", "Aceptar");
            //            activaExpress=false;
            //            break;
            //        }
            //    }
            //}
            //if ((DateTime.Now.Hour > 19 || DateTime.Now.Hour < 8)&& activaExpress==true)
            //{
            //    await DisplayAlert("Aviso", "No se encuentra disponible el Viaje Express. Horario de atención" +
            //        "de 8am a 8pm ", "Aceptar");
            //}
            //else if(activaExpress == true)
            //{
            //    await NavigationHelper.SafePushAsync(Navigation, new step0Sentido());
            //}
            await NavigationHelper.SafePushAsync(Navigation, new ViajeComp.step0Sentido());
            LoadingService.Hide();
        }
        private async void btnEncomienda_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            Preferences.Set("tipo", "3");
            await NavigationHelper.SafePushAsync(Navigation, new ViajeComp.step0Sentido());
            LoadingService.Hide();
        }
        private async void btnComWpp_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            try
            {
                var message = "Tengo una consulta acerca de los viajes";
                var phone = "+593995951038";

                // Formato URL para WhatsApp
                var url = $"https://wa.me/{phone}?text={Uri.EscapeDataString(message)}";

                await Launcher.OpenAsync(url);
                //if (await Launcher.CanOpenAsync(url))
                //{
                //    await Launcher.OpenAsync(url);
                //}
                //else
                //{
                //    await DisplayAlert("Error", "No se pudo abrir WhatsApp. Asegúrate de que esté instalado en el dispositivo.", "Cerrar");
                //}
            }
            catch (FeatureNotSupportedException ex)
            {
                await DisplayAlert("No soportado", "Tu dispositivo no soporta abrir esta app: " + ex.Message, "Cerrar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error inesperado", $"Error al intentar abrir WhatsApp: {ex.Message}", "Cerrar");
            }
            finally
            {
                LoadingService.Hide();
            }
        }


        private void btnCerrarSesion_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("name", "");
            Preferences.Set("IsLoggedIn", "0");
            Settings.LastUsedPW = string.Empty;
            Settings.LastUsedName = string.Empty;
            Settings.LastUsedTelf = string.Empty;
            Settings.LastUsedLog = "0";
            NavigationHelper.SafePushAsync(Navigation, new LoginPage());

        }
        protected override bool OnBackButtonPressed()
        {

            return true;
        }

        private void swMascota_Toggled(object sender, ToggledEventArgs e)
        {
            //if (swMascota.IsToggled==true)
            //{
            //    btnViajeCom.IsVisible = false;

            //}
            //else
            //{
            //    btnViajeCom.IsVisible = true;
            //}
        }

        private async void btnEliminarUsuario_Clicked(object sender, EventArgs e)
        {



            try
            {
                var eliminar = await DisplayAlert("Eliminar Usuario", "¿Está seguro de querer eliminar su usuario?", "Sí", "No");

                if (eliminar)
                {
                    libreriaDatos.UsuariosDataInsert datos = new libreriaDatos.UsuariosDataInsert()
                    {
                        usu_ci = Preferences.Get("ci","").ToString()

                    };

                    Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/UsuariosOutputs");

                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(datos);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(RequestUri, contentJson);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Preferences.Set("name", "");
                        Preferences.Set("IsLoggedIn", "0");
                        await NavigationHelper.SafePushAsync(Navigation, new LoginPage());
                    }
                    else
                    {
                        await DisplayAlert("Atención", "No es posible eliminar su usuario, favor intente mas tarde", "Cerrar");
                    }
                }


            }
            catch (Exception ex)
            {

            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Console.WriteLine("OnAppearing ejecutado");

            bool tieneSolicitudActiva = await cargarCitas(Preferences.Get("ci", "").ToString().ToString());

            var colorEncendido = Color.FromArgb("#FF9900");

            // Color Opacado (un gris claro, simulando inactividad, puedes ajustarlo a tu gusto)
            var colorOpacado = Color.FromArgb("#C0C0C0"); // Ejemplo: Color plata (Silver)

            // 2. Aplicar el color de fondo (BackgroundColor) al Border:
            if (tieneSolicitudActiva)
            {
                // Estado "Encendido" (color natural)
                borderVerViaje.BackgroundColor = colorEncendido;
            }
            else
            {
                // Estado "Opacado" (gris)
                borderVerViaje.BackgroundColor = colorOpacado;
            }

            // Si deseas que no sea clickeable cuando está opacado:
            borderVerViaje.InputTransparent = !tieneSolicitudActiva;
            //burbujaSolicitud.IsVisible = tieneSolicitudActiva;
        }

        private async void btnVerViaje_Clicked(object sender, EventArgs e)
        {
            bool tieneSolicitudActiva = await cargarCitas(Preferences.Get("ci", "").ToString().ToString());
            if (tieneSolicitudActiva)
            {
                await NavigationHelper.SafePushAsync(Navigation, new ViajeComp.viajeSeleccionado());
            }
            else
            {
                await DisplayAlert("Atención", "No tienes un viaje activo.", "OK");
            }
        }
        double xInicial, yInicial;
        double lastX, lastY;

        //private async void BurbujaSolicitud_Tapped(object sender, EventArgs e)
        //{
        //    //await DisplayAlert("Viaje Solicitado", "Tienes una solicitud de viaje pendiente.", "OK");

        //    await NavigationHelper.SafePushAsync(Navigation, new viajeSeleccionado());
        //}
        //private async Task<bool> VerificarSolicitudActiva()
        //{

        //    bool resultado = true; // Asegúrate de que sea true por ahora
        //    Console.WriteLine("Resultado: " + resultado);
        //    return resultado;
        //}
        private async Task<bool> cargarCitas(string ci)
        {
            //ci = "1003426713";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/ultima-cita-activa/"+ci);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.CitaOutput>>(content);

                if (resultado.Count >0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
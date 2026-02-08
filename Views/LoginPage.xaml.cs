using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using PeterTours.Utils;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Acr.UserDialogs;
using Microsoft.Maui.ApplicationModel;
//using Plugin.LatestVersion;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;



namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public static string osVersion = "";
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var existingPages = Navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                Navigation.RemovePage(page);
            }
            txtCI.Text = Settings.LastUsedCI;
            txtContrasenia.Text = Settings.LastUsedPW;

            txtCI.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            txtContrasenia.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            if (Settings.LastUsedCI != string.Empty && Settings.LastUsedPW != string.Empty)
            {
                logearUsuarioRegistrado();
            }

            // comprobarVersion();

        }

        protected override void OnAppearing()
        {
            comprobarVersion();
            base.OnAppearing();
        }

        private async void comprobarVersion()
        {
            //CrossLatestVersion.Current.CountryCode = "ec";
            //var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();
            //if (!isLatest)
            //{
            //    var update = await DisplayAlert("Nueva Versión", "Hay una nueva versión disponible, desea actualizar en este momento?", "Si","No");

            //    if (update)
            //    {
            //        await CrossLatestVersion.Current.OpenAppInStore();
            //    }
            //}
            // 
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/OsOutputs");
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.OsData>>(content);

                    foreach (var item in resultado)
                    {
                        string currentVersion = VersionTracking.CurrentVersion;

                        bool isAndroid = DeviceInfo.Platform == DevicePlatform.Android;
                        bool isiOS = DeviceInfo.Platform == DevicePlatform.iOS;

                        // Convierte las versiones a números de manera segura
                        if (double.TryParse(currentVersion, out double appVersion) &&
                            double.TryParse(item.ov_version, out double serverVersion))
                        {
                            // Si hay una nueva versión disponible según el sistema operativo
                            if (appVersion < serverVersion &&
                                ((isAndroid && item.ov_detalle == "android") ||
                                 (isiOS && item.ov_detalle == "ios")))
                            {
                                await DisplayAlert(
                                    "Nueva Versión",
                                    "Hay una nueva versión disponible. Es necesario actualizar la aplicación para continuar.",
                                    "Actualizar"
                                );

                                // Define la URL de la tienda según la plataforma
                                string storeUrl = isAndroid
                                    ? "https://play.google.com/store/apps/details?id=com.qds.petertours"
                                    : "https://apps.apple.com/app/id1619444731"; 

                                // Abre la tienda
                                await Launcher.OpenAsync(storeUrl);

                                // Cierra la app
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        
        private void logearUsuarioRegistrado()
        {
            Preferences.Set("name", Settings.LastUsedName);
            Preferences.Set("telf", Settings.LastUsedTelf);
            Preferences.Set("ci", Settings.LastUsedCI);
            Preferences.Set("IsLoggedIn", "1");
            NavigationHelper.SafePushAsync(Navigation, new Mapa());
        }
        private async void IniciarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                LoadingService.Show("Cargando");
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/UsuariosOutputs");
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.UsuariosData>>(content);
                    foreach (var item in resultado)
                    {
                        if (txtCI.Text == item.usu_ci && txtContrasenia.Text == item.usu_contrasena)
                        {
                            LabelError.Text = "";
                            string NombreUsuario = item.usu_nombre + " " + item.usu_apellido;
                            Preferences.Set("name", NombreUsuario);
                            Preferences.Set("telf", item.usu_telefono);
                            Preferences.Set("ci",txtCI.Text);
                            Preferences.Set("IsLoggedIn","1");

                            Settings.LastUsedCI = txtCI.Text;
                            Settings.LastUsedPW = txtContrasenia.Text;
                            Settings.LastUsedName = NombreUsuario;
                            Settings.LastUsedTelf = item.usu_telefono;
                            Settings.LastUsedLog = "1";

                            await NavigationHelper.SafePushAsync(Navigation, new Mapa());
                            break;
                        }
                        else
                        {
                            LabelError.Text = "Usuario o contraseña incorrectos";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN LOGIN: " + ex.Message);
                await DisplayAlert("Error", ex.Message, "Cerrar");
            }
            finally
            {
                LoadingService.Hide();
            }
        }

        private async void Registrarse_Click(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            await NavigationHelper.SafePushAsync(Navigation, new step1nombre());
            LoadingService.Hide();
        }

        private async void Recuperar_Click(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");

            try
            {
                string numero = "+593995951038";
                string mensaje = "Por favor necesito recuperar mi contraseña";

                // Codifica el mensaje para la URL
                string mensajeCodificado = Uri.EscapeDataString(mensaje);

                string url = $"https://wa.me/{numero}?text={mensajeCodificado}";

                // Abrir WhatsApp con el número y mensaje
                await Launcher.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir WhatsApp:\n{ex.Message}", "Cerrar");
            }
            finally
            {
                LoadingService.Hide();
            }
        }

        protected override  bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("Atención", "Desea cerrar la aplicación?", "Sí", "No");
                if (result)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });
            return  true;
        }
        private void PasswordIcon_Clicked(object sender, EventArgs e)
        {
            txtContrasenia.IsPassword = !txtContrasenia.IsPassword;

            if (txtContrasenia.IsPassword)
            {
                passwordEye.Source = "show_password.png";
            }
            else
            {
                passwordEye.Source = "hide_password.png";
            }
        }

    }
}
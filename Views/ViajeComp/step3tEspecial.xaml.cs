using Acr.UserDialogs;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if IOS
using UIKit;
#endif


namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step3tEspecial : ContentPage
    {
        // Variables estáticas para guardar la información
        public static string motivoViaje = "";
        public static string horaLlegada = "";
        public static string otraActividad = "";
        public static string horaEspera = "";
        public static string requerimiento = "";
        public static string resumen = "";

        public step3tEspecial()
        {
            InitializeComponent();
        }

        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LabelError.Text = "";

            try
            {
                motivoViaje = txtMotivoViaje?.Text?.Trim() ?? "";
                horaLlegada = timeLlegada?.Time.ToString(@"hh\:mm") ?? "";
                horaEspera = timeEspera?.Time.ToString(@"hh\:mm") ?? "";
                requerimiento = txtRequerimientoEspecial?.Text?.Trim() ?? "";

                resumen =
                    $"Motivo del viaje: {motivoViaje}. " +
                    $"Hora de llegada: {horaLlegada}. " +
                    (!string.IsNullOrWhiteSpace(otraActividad) ? $"Otra actividad antes del viaje: {otraActividad}. " : "") +
                    $"Hora que desea que el vehículo le espere: {horaEspera}. " +
                    (!string.IsNullOrWhiteSpace(requerimiento) ? $"Requerimiento especial: {requerimiento}. " : "");


                Preferences.Set("resumenViaje", resumen);
                string tipoTrifa = Preferences.Get("tipo", "0").ToString();
                if (tipoTrifa == "1")
                {                // 🟦 Popup simple
                    await Application.Current.MainPage.DisplayAlert(
                        "Gracias por tu respuesta.",
                        "Queremos recordarte que, al tratarse de un viaje compartido, no podemos comprometernos exactamente a los horarios de recogida y llegada que ingresaste. Sin embargo, haremos todo lo posible por organizar el viaje de acuerdo con tus preferencias y brindarte la mejor experiencia.",
                        "Aceptar"
                    );
                }


                LoadingService.Show("Cargando");
                await NavigationHelper.SafePushAsync(Navigation, new step3Facturacion());
            }
            catch (Exception ex)
            {
                LabelError.Text = "Ocurrió un error al continuar.";
                System.Diagnostics.Debug.WriteLine($"Error en step3tEspecial: {ex}");
            }
            finally
            {
                LoadingService.Hide();
            }
        }

        private async void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) &&
                e.NewTextValue.EndsWith("\n"))
            {
                txtRequerimientoEspecial.Text = e.NewTextValue.TrimEnd('\n');

                // 🔴 NO cierres aquí directamente
                await CloseKeyboardDeferred();
            }
        }
        private async Task CloseKeyboardDeferred()
        {
            // Espera a que MAUI termine de procesar el TextChanged
            await Task.Delay(80);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                // Paso 1: quitar foco
                txtRequerimientoEspecial.IsEnabled = false;
                txtRequerimientoEspecial.IsEnabled = true;

                // Paso 2: forzar resignación del foco
                txtRequerimientoEspecial.Unfocus();

#if IOS
                UIApplication.SharedApplication.SendAction(
                    new ObjCRuntime.Selector("resignFirstResponder"),
                    null,
                    null,
                    null);
#endif
            });
        }





    }

}
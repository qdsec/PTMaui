using Acr.UserDialogs;
//using Java.Lang;
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
    public partial class step2ConfirmaPasajeroOtroEnc : ContentPage
    {
        public static string nombre = "";
        public static string telefono = "";
        public static string descEncomienda = "";
        public static string receptorEncomienda = "";
        public static string celularReceptorEncomienda = "";
        public int numeroOcupantes = (string.IsNullOrEmpty(step1Fecha.ocupantes) || step1Fecha.ocupantes == "0") ? 1 : Int32.Parse(step1Fecha.ocupantes);
        public static List<PasajeroInfo> ListaPasajeros = step2ConfirmaPasajeroOtro.ListaPasajeros;

        public step2ConfirmaPasajeroOtroEnc()
        {
            InitializeComponent();

            // Configurar color de barra
            var navigationPage = Application.Current.MainPage as NavigationPage;
            if (navigationPage != null)
                navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");

            // Generar formulario de retiro/entrega
            GenerarFormularioRetiroEntrega();

            // Si hay datos guardados en Application.Current.Properties, rellenarlos
            if (ListaPasajeros == null || ListaPasajeros.Count == 0)
            {
                var lista = ListaPasajeros;
                if (lista != null && lista.Count > 0)
                {
                    var pasajero = lista[0];

                    var frame = pnlPasajeros.Children[1] as Frame;
                    var stack = frame.Content as StackLayout;

                    var retiroNombre = stack.Children[1] as Entry;
                    var retiroCelular = stack.Children[2] as Entry;
                    var gestionRetiroSwitch = (stack.Children[3] as StackLayout).Children[1] as Switch;

                    var destinoNombre = stack.Children[5] as Entry;
                    var destinoCelular = stack.Children[6] as Entry;
                    var gestionEntregaSwitch = (stack.Children[7] as StackLayout).Children[1] as Switch;

                    // Rellenar datos existentes
                    retiroNombre.Text = pasajero.ContactoRetiroNombre;
                    retiroCelular.Text = pasajero.ContactoRetiroCelular;
                    gestionRetiroSwitch.IsToggled = pasajero.GestionRetiro;

                    destinoNombre.Text = pasajero.ContactoDestinoNombre;
                    destinoCelular.Text = pasajero.ContactoDestinoCelular;
                    gestionEntregaSwitch.IsToggled = pasajero.GestionEntrega;

                    // Eventos de alerta
                    gestionRetiroSwitch.Toggled += async (s, e) =>
                    {
                        if (e.Value)
                        {
                            await DisplayAlert(
                                "Atención",
                                "Esto genera costo extra. Nos comunicaremos con el cliente para informarle sobre los valores adicionales de la reserva. Este cargo es adicional al valor final",
                                "OK"
                            );
                        }
                    };

                    gestionEntregaSwitch.Toggled += async (s, e) =>
                    {
                        if (e.Value)
                        {
                            await DisplayAlert(
                                "Atención",
                                "Esto genera costo extra. Nos comunicaremos con el cliente para informarle sobre los valores adicionales de la reserva. Este cargo es adicional al valor final",
                                "OK"
                            );
                        }
                    };
                }
            }
        }


        private void GenerarFormularioRetiroEntrega()
        {
            pnlPasajeros.Children.Clear();

            // --- CABECERA ---
            pnlPasajeros.Children.Add(new Label
            {
                Text = "Ingrese los datos de retiro y entrega",
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.Black,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            });

            var frame = new Frame
            {
                BorderColor = Colors.LightGray,
                CornerRadius = 10,
                Padding = 10,
                Content = new StackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Label { Text = "Contacto Retiro", FontAttributes = FontAttributes.Bold },
                        new Entry { Placeholder = "Nombre y Apellido" },
                        new Entry { Placeholder = "Número de Celular", Keyboard = Keyboard.Numeric, MaxLength=10 },

                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                new Label{ Text = "¿Necesita gestión para el retiro?", VerticalOptions = LayoutOptions.Center },
                                new Switch()
                            }
                        },

                        new Label { Text = "Contacto Destino", FontAttributes = FontAttributes.Bold },
                        new Entry { Placeholder = "Nombre y Apellido" },
                        new Entry { Placeholder = "Número de Celular", Keyboard = Keyboard.Numeric, MaxLength=10 },

                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                new Label{ Text = "¿Necesita gestión para la entrega?", VerticalOptions = LayoutOptions.Center },
                                new Switch()
                            }
                        }
                    }
                }
            };

            pnlPasajeros.Children.Add(frame);
        }

        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            await System.Threading.Tasks.Task.Delay(100);
            LabelError.Text = "";

            // Recuperamos la lista guardada en la pantalla anterior
            var listaTemp = (ListaPasajeros != null && ListaPasajeros.Count > 0)
                ? ListaPasajeros
                : new List<PasajeroInfo>();

            bool valido = true;

            if (listaTemp != null && listaTemp.Count > 0)
            {
                var pasajero = listaTemp[0]; // Encomienda siempre es 1 elemento

                var frame = pnlPasajeros.Children[1] as Frame;
                var stack = frame.Content as StackLayout;

                var retiroNombre = stack.Children[1] as Entry;
                var retiroCelular = stack.Children[2] as Entry;
                var gestionRetiroSwitch = (stack.Children[3] as StackLayout).Children[1] as Switch;

                var destinoNombre = stack.Children[5] as Entry;
                var destinoCelular = stack.Children[6] as Entry;
                var gestionEntregaSwitch = (stack.Children[7] as StackLayout).Children[1] as Switch;

                // Validación
                if (string.IsNullOrWhiteSpace(retiroNombre.Text) ||
                    string.IsNullOrWhiteSpace(retiroCelular.Text) ||
                    retiroCelular.Text.Length < 10 ||
                    string.IsNullOrWhiteSpace(destinoNombre.Text) ||
                    string.IsNullOrWhiteSpace(destinoCelular.Text) ||
                    destinoCelular.Text.Length < 10)
                {
                    valido = false;
                }
                else
                {

                    pasajero.Nombre = retiroNombre.Text;
                    pasajero.Celular = retiroCelular.Text;
                    pasajero.ContactoRetiroNombre = retiroNombre.Text;
                    pasajero.ContactoRetiroCelular = retiroCelular.Text;
                    pasajero.GestionRetiro = gestionRetiroSwitch.IsToggled;

                    pasajero.ContactoDestinoNombre = destinoNombre.Text;
                    pasajero.ContactoDestinoCelular = destinoCelular.Text;
                    pasajero.GestionEntrega = gestionEntregaSwitch.IsToggled;
                }
            }
            else
            {
                valido = false;
            }

            if (valido)
            {


                step2ConfirmaPasajeroOtro.ListaPasajeros= listaTemp;

                await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());
            }
            else
            {
                LabelError.Text = "Favor ingrese todos los datos correctamente";
            }

            LoadingService.Hide();
        }
    }
    
}
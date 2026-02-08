using Acr.UserDialogs;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step2ConfirmaPasajeroOtro : ContentPage
    {
        public static string nombre = "";
        public static string telefono = "";
        public static string descEncomienda = "";
        public static string receptorEncomienda = "";
        public static string celularReceptorEncomienda = "";
        public int numeroOcupantes = (string.IsNullOrEmpty(step1Fecha.ocupantes) || step1Fecha.ocupantes == "0") ? 1 : Int32.Parse(step1Fecha.ocupantes);
        public static List<PasajeroInfo> ListaPasajeros = new List<PasajeroInfo>();

        public step2ConfirmaPasajeroOtro()
        {
            InitializeComponent();

            // Configurar color de barra
            var navigationPage = Application.Current.MainPage as NavigationPage;
            if (navigationPage != null)
                navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");

            // Generar formularios dinámicos según tipo de servicio
            GenerarFormulariosPasajeros(numeroOcupantes); // numeroOcupantes: cantidad de pasajeros o 1 para encomienda

            // Obtener tipo de servicio
            var tipoTrifa = Preferences.Get("tipo","0").ToString();

            if (pnlPasajeros.Children.Count > 0)
            {
                // El primer frame siempre está en índice 1 (0 = label de cabecera)
                var primerFrame = pnlPasajeros.Children[1] as Frame;
                if (primerFrame != null)
                {
                    var stack = primerFrame.Content as StackLayout;

                    if (tipoTrifa == "3") // ENCOMIENDA
                    {
                        var cedulaEntry = stack.Children[1] as Entry;
                        var descEditor = stack.Children[3] as Editor;
                        var valorEntry = stack.Children[5] as Entry;
                        var fotoStack = stack.Children[6] as StackLayout; 
                        var imgEncomienda = fotoStack?.Children[1] as Image;
                        string rutaFoto = (imgEncomienda?.Source as FileImageSource)?.File;

                        //var retiroNombre = stack.Children[7] as Entry;
                        //var retiroCelular = stack.Children[8] as Entry;

                        //var destinoNombre = stack.Children[11] as Entry;
                        //var destinoCelular = stack.Children[12] as Entry;


                        //var gestionRetiroSwitch = (stack.Children[9] as StackLayout).Children[1] as Switch;
                        //var gestionEntregaSwitch = (stack.Children[13] as StackLayout).Children[1] as Switch;

                        // Evento para gestión de retiro
                        //gestionRetiroSwitch.Toggled += async (s, e) =>
                        //{
                        //    if (e.Value) // Si se activa
                        //    {
                        //        await Application.Current.MainPage.DisplayAlert(
                        //            "Atención",
                        //            "Esto genera costo extra. Nos comunicaremos con el cliente para informarle sobre los valores adicionales de la reserva. Este cargo es adicional al valor final",
                        //            "OK"
                        //        );
                        //    }
                        //};

                        //// Evento para gestión de entrega
                        //gestionEntregaSwitch.Toggled += async (s, e) =>
                        //{
                        //    if (e.Value) // Si se activa
                        //    {
                        //        await Application.Current.MainPage.DisplayAlert(
                        //            "Atención",
                        //            "Esto genera costo extra. Nos comunicaremos con el cliente para informarle sobre los valores adicionales de la reserva. Este cargo es adicional al valor final",
                        //            "OK"
                        //        );
                        //    }
                        //};


                        // Rellenar datos si existen en Application.Current.Properties
                        if (Preferences.ContainsKey("cedula"))
                        {
                            cedulaEntry.Text = Preferences.Get("cedula", string.Empty);
                        }

                        //if (Application.Current.Properties.ContainsKey("descEnvio"))
                        //    descEditor.Text = Application.Current.Properties["descEnvio"].ToString();

                        //if (Application.Current.Properties.ContainsKey("valorEnvio"))
                        //    valorEntry.Text = Application.Current.Properties["valorEnvio"].ToString();


                        //if (Application.Current.Properties.ContainsKey("retiroNombre"))
                        //    retiroNombre.Text = Application.Current.Properties["retiroNombre"].ToString();

                        //if (Application.Current.Properties.ContainsKey("retiroCelular"))
                        //    retiroCelular.Text = Application.Current.Properties["retiroCelular"].ToString();

                        //if (Application.Current.Properties.ContainsKey("destinoNombre"))
                        //    destinoNombre.Text = Application.Current.Properties["destinoNombre"].ToString();

                        //if (Application.Current.Properties.ContainsKey("destinoCelular"))
                        //    destinoCelular.Text = Application.Current.Properties["destinoCelular"].ToString();
                    }
                    else // PASAJERO NORMAL
                    {

                        var nombreEntry = stack.Children[1] as Entry;
                        var empresaEntry = stack.Children[2] as Entry;
                        var celularStack = stack.Children[3] as StackLayout;
                        var celularEntry = celularStack.Children[1] as Entry;

                        if (Preferences.ContainsKey("name"))
                            nombreEntry.Text = Preferences.Get("name", string.Empty);

                        if (Preferences.ContainsKey("telf"))
                            celularEntry.Text = Preferences.Get("telf", string.Empty);

                        if (Preferences.ContainsKey("empresa"))
                            empresaEntry.Text = Preferences.Get("empresa", string.Empty);

                    }
                }
            }
        }


        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando...");
            await Task.Delay(100);

            LabelError.Text = "";
            ListaPasajeros.Clear();
            bool valido = true;

            string tipoServicio = Preferences.Get("tipo", "0").ToString();

            if (tipoServicio == "3") // --- ENCOMIENDA ---
            {
                // frame de encomienda (0 = label, 1 = frame)
                var frame = pnlPasajeros.Children[1] as Frame;
                var stack = frame.Content as StackLayout;
                var cedulaEntry = stack.Children[1] as Entry;
                var descEditor = stack.Children[3] as Editor;
                var valorEntry = stack.Children[5] as Entry;

                // Foto
                var fotoStack = stack.Children[6] as StackLayout;
                var imgEncomienda = fotoStack?.Children[1] as Image;
                string rutaFotoLocal = (imgEncomienda?.Source as FileImageSource)?.File;

                // --- VALIDACIÓN ---
                if (string.IsNullOrWhiteSpace(cedulaEntry.Text) ||
                    string.IsNullOrWhiteSpace(descEditor.Text) ||
                    string.IsNullOrWhiteSpace(valorEntry.Text) ||
                    string.IsNullOrEmpty(rutaFotoLocal))
                {
                    valido = false;
                    LabelError.Text = "Ingrese todos los datos correctamente y cargue la foto de la encomienda.";
                }
                else
                {
                    string urlAPI = "http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/subir-foto-encomienda";
                    string urlFotoServidor = "";

                    try
                    {
                        using (var httpClient = new HttpClient())
                        using (var contenido = new MultipartFormDataContent())
                        {
                            using (var fileStream = File.OpenRead(rutaFotoLocal))
                            {
                                var streamContent = new StreamContent(fileStream);
                                streamContent.Headers.ContentType =
                                    new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                                contenido.Add(streamContent, "archivo", Path.GetFileName(rutaFotoLocal));

                                var respuesta = await httpClient.PostAsync(urlAPI, contenido);
                                var json = await respuesta.Content.ReadAsStringAsync();

                                if (respuesta.IsSuccessStatusCode)
                                {
                                    var resultado = Newtonsoft.Json.Linq.JObject.Parse(json);
                                    urlFotoServidor = resultado["url"]?.ToString();
                                }
                                else
                                {
                                    valido = false;
                                    LabelError.Text = "Error al subir la foto, favor seleccione una desde su galería: " + json;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        valido = false;
                        LabelError.Text = "Error al subir la foto, favor seleccione una desde su galería: " + ex.Message;
                    }

                    if (valido)
                    {
                        ListaPasajeros.Add(new PasajeroInfo
                        {
                            CedulaResponsable = cedulaEntry.Text,
                            DescripcionEnvio = descEditor.Text,
                            ValorEnvio = decimal.TryParse(valorEntry.Text, out var val) ? val : 0,
                            RutaFoto = urlFotoServidor 
                        });
                    }
                }
            }
            else // --- PASAJEROS ---
            {
                for (int i = 1; i < pnlPasajeros.Children.Count; i++)
                {
                    var frame = pnlPasajeros.Children[i] as Frame;
                    if (frame != null)
                    {
                        var stack = frame.Content as StackLayout;
                        var nombreEntry = stack.Children[1] as Entry;
                        var empresaEntry = stack.Children[2] as Entry;

                        var celularStack = stack.Children[3] as StackLayout;
                        var celularEntry = celularStack.Children[1] as Entry;

                        if (string.IsNullOrWhiteSpace(nombreEntry.Text) ||
                            string.IsNullOrWhiteSpace(celularEntry.Text) ||
                            celularEntry.Text.Length < 10)
                        {
                            valido = false;
                            LabelError.Text = "Ingrese los nombres y teléfono de contacto.";
                            break;
                        }

                        ListaPasajeros.Add(new PasajeroInfo
                        {
                            Nombre = string.IsNullOrWhiteSpace(empresaEntry.Text)
                                ? nombreEntry.Text
                                : nombreEntry.Text + " - " + empresaEntry.Text,
                            Celular = celularEntry.Text,
                            Empresa = empresaEntry.Text,
                            CedulaResponsable= Preferences.Get("ci", "0").ToString()
                        });
                    }
                }
            }

            // --- NAVEGACIÓN ---
            if (valido)
            {
                //Application.Current.Properties["Pasajeros"] = ListaPasajeros;

                if (tipoServicio == "3")
                    await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajeroOtroEnc());
                else
                    await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());
            }

            LoadingService.Hide();
        }



        private void GenerarFormulariosPasajeros(int numPasajeros)
        {
            pnlPasajeros.Children.Clear();
            string tipoTrifa = Preferences.Get("tipo", "0").ToString();

            if (tipoTrifa == "3") // Encomienda
            {
                // --- CABECERA ---
                pnlPasajeros.Children.Add(new Label
                {
                    Text = "Ingrese los datos de la encomienda",
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                });
                var editorEncomienda = new Editor
                {
                    Placeholder = "Ejemplo: Funda de ropa, caja 15x15, celular.",
                    AutoSize = EditorAutoSizeOption.Disabled,
                    HeightRequest = 80,
                    BackgroundColor = Colors.White,
                    TextColor = Colors.Black
                };

                // 2. Suscribir el evento
                editorEncomienda.TextChanged += OnEditorTextChanged;
                var frame = new Frame
                {
                    BorderColor = Colors.LightGray,
                    CornerRadius = 10,
                    Padding = 10,
                    Content = new StackLayout
                    {
                        Spacing = 8,
                        Children =
                {
                    new Label { Text = "Cédula del Responsable", TextColor = Colors.Black },
                    new Entry { Placeholder = "Cédula", Keyboard = Keyboard.Numeric, MaxLength=10 },

                    new Label { Text = "Descripción de la encomienda", TextColor = Colors.Black },
                    editorEncomienda,
                    //new Editor { Placeholder = "Ejemplo: Funda de ropa, caja 15x15, celular.", AutoSize = EditorAutoSizeOption.TextChanges, HeightRequest = 80 },


                    new Label { Text = "Valor de la encomienda", TextColor = Colors.Black },
                    new Entry { Placeholder = "Ingrese el valor en dólares", Keyboard = Keyboard.Numeric,MaxLength=6 },

                    // --- SECCIÓN FOTO DE LA ENCOMIENDA ---
                    CrearSeccionFotoEncomienda()
                }
                    }
                };

                pnlPasajeros.Children.Add(frame);

            }
            else // Viaje / Pasajeros
            {
                // --- CABECERA PASAJEROS ---
                pnlPasajeros.Children.Add(new Label
                {
                    Text = "Ingrese los datos de quienes viajan",
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Colors.Black,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                });

                for (int i = 1; i <= numPasajeros; i++)
                {
                    var frame = new Frame
                    {
                        BorderColor = Colors.LightGray,
                        CornerRadius = 10,
                        Padding = 10,
                        Content = new StackLayout
                        {
                            Spacing = 8,
                            Children =
                    {
                        new Label { Text = $"Pasajero {i}", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                        new Entry { Placeholder = "Nombres y Apellidos", MaxLength = 25 },
                        new Entry { Placeholder = "Empresa o Institución", MaxLength = 25 },
                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Children = {
                                new Image { Source = "iconocel.png", WidthRequest = 40, HeightRequest = 40 },
                                new Entry { Placeholder = "Número de Celular", Keyboard = Keyboard.Numeric, MaxLength = 10 }
                            }
                        }
                    }
                        }
                    };

                    pnlPasajeros.Children.Add(frame);
                }
            }
        }
        private async void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            // Usamos 'as Editor' para que funcione con cualquier editor dinámico
            var editor = sender as Editor;

            if (editor != null && !string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.EndsWith("\n"))
            {
                editor.Text = e.NewTextValue.TrimEnd('\n');

                // Llamamos al cierre del teclado pasando el editor actual
                await CloseKeyboardDeferred(editor);
            }
        }

        private async Task CloseKeyboardDeferred(Editor editor)
        {
            await Task.Delay(80);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                editor.IsEnabled = false;
                editor.IsEnabled = true;
                editor.Unfocus();

#if IOS
        UIKit.UIApplication.SharedApplication.SendAction(
            new ObjCRuntime.Selector("resignFirstResponder"),
            null,
            null,
            null);
#endif
            });
        }
        private StackLayout CrearSeccionFotoEncomienda()
        {
            var imgEncomienda = new Image
            {
                Source = "cargar_foto.png",
                WidthRequest = 150,
                HeightRequest = 150,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromRgb(245, 245, 245)
            };

            string rutaFotoLocal = null; // ← aquí guardaremos la ruta persistente

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                try
                {
                    PermissionStatus photoStatus;
                    PermissionStatus cameraStatus;

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        photoStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
                        cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

                        if (photoStatus != PermissionStatus.Granted)
                            photoStatus = await Permissions.RequestAsync<Permissions.Photos>();

                        if (cameraStatus != PermissionStatus.Granted)
                            cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    }
                    else // iOS
                    {
                        photoStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
                        cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

                        if (photoStatus != PermissionStatus.Granted)
                            photoStatus = await Permissions.RequestAsync<Permissions.Photos>();

                        if (cameraStatus != PermissionStatus.Granted)
                            cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    }

                    if (photoStatus != PermissionStatus.Granted || cameraStatus != PermissionStatus.Granted)
                    {
                        bool abrirConfig = await Application.Current.MainPage.DisplayAlert(
                            "Permiso Denegado",
                            "No se puede acceder a la cámara o galería. ¿Desea abrir la configuración para activarlo?",
                            "Sí",
                            "No"
                        );

                        if (abrirConfig)
                        {
                            if (Device.RuntimePlatform == Device.iOS)
                                AppInfo.ShowSettingsUI();
                        }
                        return;
                    }

                    //string accion = await Application.Current.MainPage.DisplayActionSheet(
                    //    "Selecciona acción",
                    //    "Cancelar",
                    //    null,
                    //    "Seleccionar de la galería",
                    //    "Tomar foto"
                    //);

                    FileResult foto = null;

                    //if (accion == "Seleccionar de la galería")
                    //{
                    //    foto = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    //    {
                    //        Title = "Selecciona una foto de la encomienda"
                    //    });
                    //}
                    //else if (accion == "Tomar foto")
                    //{
                    //    foto = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                    //    {
                    //        Title = "Toma una foto de la encomienda"
                    //    });
                    //}
                    foto = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Selecciona una foto de la encomienda"
                    });
                    if (foto != null)
                    {
                        string nombreArchivo = Path.GetFileName(foto.FullPath);
                        string nuevaRuta = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);

                        using (var streamOrigen = await foto.OpenReadAsync())
                        using (var streamDestino = File.Create(nuevaRuta))
                        {
                            await streamOrigen.CopyToAsync(streamDestino);
                        }

                        rutaFotoLocal = nuevaRuta;

                        // Mostrar la imagen
                        imgEncomienda.Source = ImageSource.FromFile(rutaFotoLocal);
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "No se pudo cargar la foto: " + ex.Message,
                        "OK"
                    );
                }
            };

            imgEncomienda.GestureRecognizers.Add(tapGesture);

            return new StackLayout
            {
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Center,
                Children =
        {
            new Label
            {
                Text = "Foto de la encomienda",
                TextColor = Colors.Black,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            },
            imgEncomienda
        }
            };
        }










        //private void GenerarFormulariosPasajeros(int numPasajeros)
        //{
        //    pnlPasajeros.Children.Clear();
        //    string tipoTrifa = Application.Current.Properties["tipo"].ToString();

        //    if (tipoTrifa == "3")
        //    {
        //        // --- CABECERA ---
        //        pnlPasajeros.Children.Add(new Label
        //        {
        //            Text = "Ingrese los datos de la encomienda",
        //            HorizontalOptions = LayoutOptions.Center,
        //            TextColor = Colors.Black,
        //            FontSize = 18,
        //            FontAttributes = FontAttributes.Bold
        //        });

        //        var frame = new Frame
        //        {
        //            BorderColor = Colors.LightGray,
        //            CornerRadius = 10,
        //            Padding = 10,
        //            Content = new StackLayout
        //            {
        //                Spacing = 8,
        //                Children =
        //        {
        //            new Label { Text = "Cédula del Responsable", TextColor = Colors.Black },
        //            new Entry { Placeholder = "Cédula", Keyboard = Keyboard.Numeric, MaxLength=10 },

        //            new Label { Text = "Descripción de la encomienda", TextColor = Colors.Black },
        //            new Editor { Placeholder = "Ejemplo: Funda de ropa, caja 15x15, celular.", AutoSize = EditorAutoSizeOption.TextChanges, HeightRequest=80 },

        //            new Label { Text = "Valor de la encomienda", TextColor = Colors.Black },
        //            new Entry { Placeholder = "Ingrese el valor en dólares", Keyboard = Keyboard.Numeric },

        //            new Label { Text = "Contacto Retiro", FontAttributes=FontAttributes.Bold },
        //            new Entry { Placeholder = "Nombre y Apellido" },
        //            new Entry { Placeholder = "Número de Celular", Keyboard=Keyboard.Numeric, MaxLength=10 },

        //            new StackLayout {
        //                Orientation=StackOrientation.Horizontal,
        //                Children={
        //                    new Label{ Text="¿Necesita gestión para el retiro?", TextColor=Colors.Black, VerticalOptions=LayoutOptions.Center },
        //                    new Switch()
        //                }
        //            },

        //            new Label { Text = "Contacto Destino", FontAttributes=FontAttributes.Bold },
        //            new Entry { Placeholder = "Nombre y Apellido" },
        //            new Entry { Placeholder = "Número de Celular", Keyboard=Keyboard.Numeric, MaxLength=10 },

        //            new StackLayout {
        //                Orientation=StackOrientation.Horizontal,
        //                Children={
        //                    new Label{ Text="¿Necesita gestión para la entrega?", TextColor=Colors.Black, VerticalOptions=LayoutOptions.Center },
        //                    new Switch()
        //                }
        //            }
        //        }
        //            }
        //        };

        //        pnlPasajeros.Children.Add(frame);
        //    }
        //    else
        //    {
        //        // --- CABECERA PASAJEROS ---
        //        pnlPasajeros.Children.Add(new Label
        //        {
        //            Text = "Ingrese los datos de quienes viajan",
        //            HorizontalOptions = LayoutOptions.Center,
        //            TextColor = Colors.Black,
        //            FontSize = 18,
        //            FontAttributes = FontAttributes.Bold
        //        });

        //        // Ciclo de pasajeros
        //        for (int i = 1; i <= numPasajeros; i++)
        //        {
        //            var frame = new Frame
        //            {
        //                BorderColor = Colors.LightGray,
        //                CornerRadius = 10,
        //                Padding = 10,
        //                Content = new StackLayout
        //                {
        //                    Spacing = 8,
        //                    Children =
        //            {
        //                new Label { Text = $"Pasajero {i}", FontAttributes=FontAttributes.Bold, HorizontalOptions=LayoutOptions.Center },
        //                new Entry { Placeholder = "Nombres y Apellidos", MaxLength=25 },
        //                new Entry { Placeholder = "Empresa o Institución", MaxLength=25 },
        //                new StackLayout {
        //                    Orientation=StackOrientation.Horizontal,
        //                    HorizontalOptions=LayoutOptions.Center,
        //                    Children={
        //                        new Image { Source="iconocel.png", WidthRequest=40, HeightRequest=40 },
        //                        new Entry { Placeholder="Número de Celular", Keyboard=Keyboard.Numeric, MaxLength=10 }
        //                    }
        //                }
        //            }
        //                }
        //            };

        //            pnlPasajeros.Children.Add(frame);
        //        }
        //    }
        //}




        //private async void btnNext_Clicked(object sender, EventArgs e)
        //{
        //    //string tipoServicio = "3";
        //    LoadingService.Show("Cargando");


        //    string tipoServicio = Application.Current.Properties["tipo"].ToString();
        //    if (tipoServicio == "3")
        //    {
        //        if (!String.IsNullOrWhiteSpace(txtNombre.Text) && !String.IsNullOrWhiteSpace(txtApellido.Text) && !String.IsNullOrWhiteSpace(txtCelular.Text)
        //            && !String.IsNullOrWhiteSpace(txtReceptorNombre.Text) && !String.IsNullOrWhiteSpace(txtCelularReceptor.Text))
        //        {
        //            if (txtCelular.Text.Length > 9 && txtCelularReceptor.Text.Length > 9)
        //            {
        //                nombre = txtNombre.Text + " " + txtApellido.Text;
        //                telefono = txtCelular.Text;
        //                descEncomienda = txtDescEnco.Text;
        //                celularReceptorEncomienda = txtCelularReceptor.Text;
        //                receptorEncomienda = txtReceptorNombre.Text;
        //                LabelError.Text = "";
        //                await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());
        //            }
        //            else
        //            {
        //                LabelError.Text = "Ingrese números de celulares válidos";
        //            }
        //        }
        //        else
        //        {
        //            LabelError.Text = "Favor rellene todos los campos";
        //        }
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrWhiteSpace(txtNombre.Text) && !String.IsNullOrWhiteSpace(txtApellido.Text) && !String.IsNullOrWhiteSpace(txtCelular.Text))
        //        {
        //            if (txtCelular.Text.Length > 9)
        //            {
        //                nombre = txtNombre.Text + " " + txtApellido.Text;
        //                telefono = txtCelular.Text;
        //                descEncomienda = txtDescEnco.Text;
        //                celularReceptorEncomienda = txtCelularReceptor.Text;
        //                receptorEncomienda = txtReceptorNombre.Text;
        //                LabelError.Text = "";
        //                await NavigationHelper.SafePushAsync(Navigation, new step3Ruta());
        //            }
        //            else
        //            {
        //                LabelError.Text = "Ingrese un número de celular válido";
        //            }
        //        }
        //        else
        //        {
        //            LabelError.Text = "Favor rellene todos los campos";
        //        }
        //    }
        //    LoadingService.Hide();
        //}
    }
    public class PasajeroInfo
    {
        // Pasajeros
        public string Nombre { get; set; }
        public string Celular { get; set; }
        public string Empresa { get; set; }

        // Encomienda
        public string CedulaResponsable { get; set; }
        public string DescripcionEnvio { get; set; }
        public decimal ValorEnvio { get; set; }

        public string ContactoRetiroNombre { get; set; }
        public string ContactoRetiroCelular { get; set; }
        public bool GestionRetiro { get; set; }

        public string ContactoDestinoNombre { get; set; }
        public string ContactoDestinoCelular { get; set; }
        public bool GestionEntrega { get; set; }

        // Ruta
        public string RetiroLat { get; set; }
        public string RetiroLon { get; set; }
        public string LlegadaLat { get; set; }
        public string LlegadaLon { get; set; }
        public decimal Precio { get; set; }
        public byte[] FotoEncomienda { get; set; } 
        public string RutaFoto { get; set; } 
    }



}
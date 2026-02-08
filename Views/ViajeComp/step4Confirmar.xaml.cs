using Acr.UserDialogs;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
//using Android.Text.Method;
using Newtonsoft.Json;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step4Confirmar : ContentPage
    {
        //public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public ICommand TapCommand { get; set; }
        public string DynamicUrl { get; set; }
        public static int id_cupon = 0;


        public step4Confirmar()
        {
            InitializeComponent();

            // URL dinámica


            // Fecha y hora
            txtFecha.Text = step1Fecha.fechaLet;
            txtHora.Text = step1Fecha.hora;

            // Determinar tipo de servicio
            string tipoServicio = Preferences.Get("tipo", "0").ToString();

            // Total y precio
            decimal precioUnitario = Decimal.Parse(step3Ruta.tarifaPrecio);
            int cantidadPasajeros = Int32.Parse(step1Fecha.ocupantes.ToString());

            if (tipoServicio == "1") // viaje normal
            {
                //lblAgendar.Text = "Acepto los términos y condiciones del viaje";
                //txtAgendar.Text = "Acepto los términos y condiciones del viaje";
                //txtTiempoAproxLlegada.Text = tiempoEstimadoViaje(step3Ruta.tiempoEstimado);
                string tiempo = step3Ruta.tiempoEstimado?.Trim();
                if (string.IsNullOrEmpty(tiempo) || tiempo == "0")
                {
                    // Generar un número aleatorio entre 150 y 180 minutos
                    Random rnd = new Random();
                    int minutos = rnd.Next(150, 167);

                    int horas = minutos / 60;
                    int mins = minutos % 60;

                    tiempo = $"{horas:D2}h{mins:D2}";
                }

                txtTiempoAproxLlegada.Text = tiempoEstimadoViaje(tiempo);
                txtPrecio.Text = (precioUnitario).ToString("F2");
                txtTotal.Text = (precioUnitario).ToString("F2");
                pnlViajes.IsVisible = true;
                pnlEncomienda.IsVisible = false;
                pnlPasajeros.IsVisible = true;

            }
            else if (tipoServicio == "2") // viaje con extra
            {
                //lblAgendar.Text = "Acepto los términos y condiciones del viaje";
                //txtAgendar.Text = "Acepto los términos y condiciones del viaje";
                //txtTiempoAproxLlegada.Text = tiempoEstimadoViaje(step3Ruta.tiempoEstimado);
                string tiempo = step3Ruta.tiempoEstimado?.Trim();
                if (string.IsNullOrEmpty(tiempo) || tiempo == "0")
                {
                    // Generar un número aleatorio entre 150 y 180 minutos
                    Random rnd = new Random();
                    int minutos = rnd.Next(150, 167);

                    int horas = minutos / 60;
                    int mins = minutos % 60;

                    tiempo = $"{horas:D2}h{mins:D2}";
                }
                txtTiempoAproxLlegada.Text = tiempoEstimadoViaje(tiempo);
                txtPrecio.Text = step3Ruta.tarifaPrecio;
                decimal extra = (step2ConfirmarParrilla.necesitaParrilla == 1) ? 10 : 0;
                txtTotal.Text = (precioUnitario + extra).ToString("F2");
                pnlParrilla.IsVisible = (extra > 0);
                pnlViajes.IsVisible = true;
                pnlEncomienda.IsVisible = false;
                pnlPasajeros.IsVisible = true;
            }
            else if (tipoServicio == "3") // encomienda
            {
                txtPrecio.Text = step3Ruta.tarifaPrecio;
                txtTotal.Text = step3Ruta.tarifaPrecio;
                lblTitulo.Text = "Detalle de la encomienda";
                //lblAgendar.Text = "Acepto los términos y condiciones de la encomienda";
                //txtAgendar.Text = "Acepto los términos y condiciones de la encomienda";
                btnConfirmar.Text = "CONFIRMAR ENCOMIENDA";
                pnlEncomienda.IsVisible = true;
                pnlViajes.IsVisible = false;
                pnlPasajeros.IsVisible = false;
            }

            string horaTerminos = step1Fecha.hora;
            DynamicUrl = GetUrlForTime(horaTerminos);
            TapCommand = new Command<string>(async (url) => await Launcher.OpenAsync(url));
            BindingContext = this;

            // Ubicaciones
            txtRetiro.Text = step0Sentido.tipoViaje.Split('-')[0];
            txtLlegada.Text = step0Sentido.tipoViaje.Split('-')[1];

            // Requerimiento
            txtRequerimiento.Text = string.IsNullOrEmpty(step3tEspecial.requerimiento) ? "N/A" : "Sí";

            // Cupón
            btnCupon.IsEnabled = true;
            btnCupon.BackgroundColor = Color.FromHex("#fc940c");
            txtCupon.Text = "";
            txtCuponAplicado.IsEnabled = true;

            // Cargar pasajeros/encomiendas dinámicamente
            CargarListaPasajeros();

            cargarCorreo();
        }
        public static string tiempoEstimadoViaje(string horaEstim)
        {
            // Validar nulo, vacío o "0"
            if (string.IsNullOrWhiteSpace(horaEstim) || horaEstim.Trim() == "0")
                return "Duración de viaje no disponible";

            // Debe contener "h"
            if (!horaEstim.Contains("h"))
                return "Duración de viaje no disponible";

            // Separar horas y minutos
            string[] partes = horaEstim.Split('h');
            if (partes.Length != 2 ||
                !int.TryParse(partes[0], out int horas) ||
                !int.TryParse(partes[1], out int minutos))
                return "Duración de viaje no disponible";

            try
            {
                // Crear el TimeSpan original
                TimeSpan tiempo = new TimeSpan(horas, minutos, 0);

                // Sumar 15 minutos
                tiempo = tiempo.Add(TimeSpan.FromMinutes(15));

                // Devolver en formato XhYY (con 00 si es en punto)
                return $"{(int)tiempo.TotalHours}h{tiempo.Minutes:D2}";
            }
            catch
            {
                // Si algo falla, devolver leyenda genérica
                return "Duración de viaje no disponible";
            }
        }




        private void CargarListaPasajeros()
        {
            // Usamos la lista que puede tener 1 o varios elementos
            List<PasajeroInfo> lista = step2ConfirmaPasajeroOtro.ListaPasajeros;

            if (lista == null || lista.Count == 0) return;

            // Si es un solo pasajero, llenamos los campos individuales
            var primerPasajero = lista[0];

            txtNombresEnc.Text = primerPasajero.ContactoRetiroNombre;
            txtTelfEnc.Text = primerPasajero.ContactoRetiroCelular;
            txtTelf.Text = primerPasajero.Celular;
            txtReceptorEnc.Text = primerPasajero.ContactoDestinoNombre;
            txtTelfReceptorEnc.Text = primerPasajero.ContactoDestinoCelular;
            txtRequerimientoEnc.Text = (primerPasajero.GestionEntrega || primerPasajero.GestionRetiro) ? "Sí" : "No";
            //txtRequerimiento.Text = primerPasajero.GestionEntrega ? "Sí" : "No";

            txtRetiroEnc.Text = step0Sentido.tipoViaje.Split('-')[0];
            txtLlegadaEnc.Text = step0Sentido.tipoViaje.Split('-')[1];

            txtRetiro.Text = step0Sentido.tipoViaje.Split('-')[0];
            txtLlegada.Text = step0Sentido.tipoViaje.Split('-')[1];

            txtPasajeros.Text = step1Fecha.ocupantes.ToString();

            // Si quieres mostrar más de un pasajero, puedes usar un ListView o un StackLayout dinámico
            if (lista.Count > 0)
            {
                pnlPasajeros.Children.Clear();

                pnlPasajeros.Children.Add(new Label
                {
                    Text = "Pasajeros:",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black,
                    Margin = new Thickness(10, 0, 0, 3)
                });

                foreach (var p in lista)
                {
                    var stack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(10, 2, 10, 0),
                        Spacing = 10
                    };

                    stack.Children.Add(new Label
                    {
                        Text = $"Nombre: {p.Nombre}",
                        FontSize = 15,
                        TextColor = Colors.Black
                    });

                    stack.Children.Add(new Label
                    {
                        Text = $"Celular: {p.Celular}",
                        FontSize = 15,
                        TextColor = Colors.Black
                    });

                    pnlPasajeros.Children.Add(stack);
                }
            }


        }


        private string GetUrlForTime(string horaTerminos)
        {
            // Extraer solo la hora (los primeros 2 caracteres antes de 'h')
            int hora = int.Parse(horaTerminos.Substring(0, 2));
            string tipoServicio = Preferences.Get("tipo", "0").ToString();
            if (tipoServicio == "1")
            {
                // Determinar la URL según el rango de hora
                if (hora >= 0 && hora <= 5)
                {
                    return "https://transpeters.com/terminos-y-condiciones-noche/";
                }
                else if (hora >= 6 && hora <= 9)
                {
                    return "https://transpeters.com/terminos-y-condiciones-nueveam/";
                }
                else // El resto de las horas
                {
                    return "https://transpeters.com/terminos-y-condiciones/";
                }
            }
            else if (tipoServicio == "2")
            {
                return "https://transpeters.com/terminos-y-condiciones-express/";
            }
            else if (tipoServicio == "3") {
                return "https://transpeters.com/terminos-y-condiciones-encomiendas/";
            }
            else
            {
                return "https://transpeters.com/terminos-y-condiciones/";
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //lblAgendar.TextType = TextType.Html;
            //lblAgendar.TextType = TextType.Text;
            await cargarCorreo();
        }

        private async void btnConfirmar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var lista = step2ConfirmaPasajeroOtro.ListaPasajeros ?? new List<PasajeroInfo>();
                bool citParrilla = step2ConfirmarParrilla.necesitaParrilla.HasValue
                               && step2ConfirmarParrilla.necesitaParrilla.Value == 1;

                // Recuperar datos de facturación guardados en el paso anterior
                string facCedula = Preferences.Get("factura_id", "");
                string facNombre = Preferences.Get("factura_nombre", "");
                string facCorreo = Preferences.Get("factura_correo", "");
                string facTelefono = Preferences.Get("factura_telefono", "");
                string facDireccion = Preferences.Get("factura_direccion", "");

                var request = new CitaDinamicaRequest
                {
                    Pasajeros = lista,
                    Fecha = DateTime.ParseExact(step1Fecha.fecha.Trim(), "d/M/yyyy", CultureInfo.InvariantCulture),
                    Hora = txtHora.Text,
                    Origen = step3Ruta.ubicacionRetiro,
                    Destino = step3Ruta.ubicacionLlegada,
                    Telf = txtTelf.Text,
                    Detalles = step3tEspecial.resumen,
                    Tipo = int.Parse(Preferences.Get("tipo", "0").ToString()),
                    Precio = decimal.Parse(txtTotal.Text),
                    Parrilla = citParrilla,
                    CedulaFac = facCedula,
                    NombreRcFac = facNombre,
                    CorreoFac = facCorreo,
                    TelefonoFac = facTelefono,
                    DireccionFac = facDireccion,
                    Sentido= step1Fecha.idSentidoViaje,
                    SentidoOrigen= step0Sentido.tipoViaje.Split('-')[0],
                    SentidoDestino= step0Sentido.tipoViaje.Split('-')[1]
                };
                txtRetiro.Text = step0Sentido.tipoViaje.Split('-')[0];
                txtLlegada.Text = step0Sentido.tipoViaje.Split('-')[1];
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Se recomienda usar un timeout para evitar que la app se quede colgada
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var response = await client.PostAsync("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/insert-cita-dinamica-v3", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await envioCorreo(request);
                        await insertarViajeConDescuento(id_cupon, Preferences.Get("ci", "0").ToString());

                        // Limpiar los datos de factura de las preferencias después del éxito (opcional)
                        // LimpiarFacturaPreferences();

                        await NavigationHelper.SafePushAsync(Navigation, new step5Cuentas());
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Error", "No se pudo registrar la cita. Servidor respondió: " + response.StatusCode, "Cerrar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo registrar la cita: " + ex.Message, "Cerrar");
            }
        }

        //private async void btnConfirmar_Clicked(object sender, EventArgs e)
        //{
        //    string fec = step1Fecha.fecha;
        //    string ci = Application.Current.Properties["ci"].ToString();
        //    string nombre = txtNombres.Text;
        //    string pasajeros = txtPasajeros.Text;
        //    string fecFormato = fec.Split('/')[2] + "-" + fec.Split('/')[1] + "-" + fec.Split('/')[0];
        //    DateTime fecha = DateTime.Parse(fecFormato);
        //    string hora = txtHora.Text;
        //    //string origen = txtRetiro.Text;
        //    //string destino = txtLlegada.Text;
        //    string requerimiento=txtRequerimiento.Text;
        //    string origen = step3Ruta.ubicacionRetiro;
        //    string destino = step3Ruta.ubicacionLlegada;
        //    string retiroLatitud = step3Ruta.retiroLat;
        //    string retiroLongitud = step3Ruta.retiroLon;
        //    string llegadaLatitud = step3Ruta.llegadaLat;
        //    string llegadaLongitud = step3Ruta.llegadaLon;
        //    string tipo = Application.Current.Properties["tipo"].ToString();
        //    decimal precio = Decimal.Parse(txtTotal.Text);
        //    string telf = txtTelf.Text;
        //    //Encomienda
        //    string receptorEncomienda = step2ConfirmaPasajeroOtro.receptorEncomienda;
        //    string descripcionEncomienda = step2ConfirmaPasajeroOtro.descEncomienda;
        //    string celularReceptorEncomienda = step2ConfirmaPasajeroOtro.celularReceptorEncomienda;
        //    //Fin Encomienda

        //    if (!String.IsNullOrWhiteSpace(ci) && !String.IsNullOrWhiteSpace(nombre) && !String.IsNullOrWhiteSpace(pasajeros) &&
        //    !String.IsNullOrWhiteSpace(hora) && !String.IsNullOrWhiteSpace(origen) && !String.IsNullOrWhiteSpace(destino) &&
        //    !String.IsNullOrWhiteSpace(retiroLatitud) && !String.IsNullOrWhiteSpace(retiroLongitud) && !String.IsNullOrWhiteSpace(llegadaLatitud) &&
        //    !String.IsNullOrWhiteSpace(llegadaLongitud) && !String.IsNullOrWhiteSpace(tipo) && !String.IsNullOrWhiteSpace(precio.ToString()))
        //    {
        //        libreriaDatos.CitasDataInsert datos = new libreriaDatos.CitasDataInsert()
        //        {
        //            cit_ci = ci,
        //            cit_nombre = nombre,
        //            cit_cantidad_pasajeros = Int32.Parse(pasajeros),
        //            cit_fecha = fecha,
        //            cit_hora = hora,
        //            cit_origen = origen,
        //            cit_destino = destino,
        //            cit_origen_lat = retiroLatitud,
        //            cit_origen_lon = retiroLongitud,
        //            cit_destino_lat = llegadaLatitud,
        //            cit_destino_lon = llegadaLongitud,
        //            cit_tipo = Int32.Parse(tipo),
        //            cit_enviado = Int32.Parse(tipo),
        //            cit_precio = precio,
        //            cit_rec_detalle = descripcionEncomienda,
        //            cit_rec_telf = celularReceptorEncomienda,
        //            cit_rec_encom = receptorEncomienda,
        //            cit_telf = telf,
        //            cit_detalles=requerimiento
        //        };
        //        Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs");

        //        var client = new HttpClient();
        //        var json = JsonConvert.SerializeObject(datos);
        //        var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = client.PostAsync(RequestUri, contentJson).Result;
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {

        //            await envioCorreo(datos);
        //            //await DisplayAlert("Viaje Confirmado", "Gracias por usar Peter Tours, el conductor se comunicará con usted una hora antes del viaje", "Cerrar");
        //            //await NavigationHelper.SafePushAsync(Navigation, new MainPage());
        //            await insertarViajeConDescuento(id_cupon, Application.Current.Properties["ci"].ToString());
        //            await NavigationHelper.SafePushAsync(Navigation, new step5Cuentas());
        //            //App.Current.Logout();
        //        }
        //        else
        //        {
        //             await DisplayAlert("Atención", "Favor intente agendar su cita mas tarde", "Cerrar");
        //        }


        //    }
        //    else
        //    {
        //         await DisplayAlert("Atención", "Favor intente agendar su cita mas tarde", "Cerrar");
        //    }

        //}

        public async Task<bool> cargarCorreo()
        {
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
                    if (Preferences.Get("ci", "0").ToString() == item.usu_ci)
                    {
                        Preferences.Set("correo", item.usu_email);
                        return true;

                    }
                }
            }
            return false;
        }
        //public async Task<bool> envioCorreo(libreriaDatos.CitasDataInsert data)
        //{
        //    try
        //    {
        //        LoadingService.Show("Cargando");

        //        libreriaDatos.CitaOutput datos = new libreriaDatos.CitaOutput()
        //        {
        //            cit_ci = data.cit_ci,
        //            cit_nombre = data.cit_nombre,
        //            cit_cantidad_pasajeros = data.cit_cantidad_pasajeros,
        //            cit_fecha = data.cit_fecha,
        //            cit_hora = data.cit_hora,
        //            cit_origen = data.cit_origen,
        //            cit_destino = data.cit_destino,
        //            cit_origen_lat = data.cit_origen_lat,
        //            cit_origen_lon = data.cit_origen_lon,
        //            cit_destino_lat = data.cit_destino_lat,
        //            cit_destino_lon = data.cit_destino_lon,
        //            cit_tipo = data.cit_tipo,
        //            cit_enviado = data.cit_enviado,
        //            cit_precio = data.cit_precio,
        //            cit_rec_detalle = data.cit_rec_detalle,
        //            cit_rec_telf = data.cit_rec_telf,
        //            cit_rec_encom = data.cit_rec_encom,
        //            cit_telf = data.cit_telf,
        //            cit_detalles = data.cit_detalles,
        //            correo = Application.Current.Properties["correo"].ToString()
        //        };

        //        Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/enviar-correo");

        //        using (var client = new HttpClient())
        //        {
        //            var json = JsonConvert.SerializeObject(datos);
        //            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

        //            var response = await client.PostAsync(requestUri, contentJson);

        //            return response.StatusCode == HttpStatusCode.OK;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Puedes loguear el error si deseas
        //        return false;
        //    }
        //    finally
        //    {
        //        LoadingService.Hide();
        //    }
        //}

        public async Task<bool> envioCorreo(CitaDinamicaRequest data)
        {
            try
            {
                LoadingService.Show("Cargando");

                // Construimos un objeto que refleje la info del correo
                var correoData = new
                {
                    Fecha = data.Fecha.ToString("yyyy-MM-dd"),
                    Hora = data.Hora,
                    Origen = data.Origen,
                    Destino = data.Destino,
                    Telf = data.Telf,
                    Detalles = data.Detalles,
                    Tipo = data.Tipo,
                    Precio = data.Precio,
                    Parrilla = data.Parrilla,
                    CorreoDestino = Preferences.Get("correo", "").ToString(),
                    Pasajeros = data.Pasajeros.Select(p => new
                    {
                        Nombre = p.Nombre,
                        Celular = p.Celular,
                        Empresa = p.Empresa,
                        CedulaResponsable = p.CedulaResponsable,
                        DescripcionEnvio = p.DescripcionEnvio,
                        ValorEnvio = p.ValorEnvio,
                        ContactoRetiroNombre = p.ContactoRetiroNombre,
                        ContactoRetiroCelular = p.ContactoRetiroCelular,
                        GestionRetiro = p.GestionRetiro,
                        ContactoDestinoNombre = p.ContactoDestinoNombre,
                        ContactoDestinoCelular = p.ContactoDestinoCelular,
                        GestionEntrega = p.GestionEntrega,
                        RetiroLat = p.RetiroLat,
                        RetiroLon = p.RetiroLon,
                        LlegadaLat = p.LlegadaLat,
                        LlegadaLon = p.LlegadaLon,
                        Precio = p.Precio,
                        RutaFoto = p.RutaFoto
                        //Parrilla = p.Parrilla
                    }).ToList()
                };

                Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/enviar-correo-nuevo");

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(correoData, Formatting.Indented);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(requestUri, contentJson);

                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                // Opcional: loguear ex.Message
                return false;
            }
            finally
            {
                LoadingService.Hide();
            }
        }


        private void chbConfirmar_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (chbConfirmar.IsChecked == true)
            {
                string tipoServicio = Preferences.Get("tipo", "0").ToString();
                btnConfirmar.BackgroundColor = Color.FromHex("#fc940c");
                string textoTerminos = "He leído los términos y condiciones y me comprometo a realizar el viaje en la fecha y hora seleccionada. En caso de no realizarlo me comprometo a cumplir la política de cancelación de viaje y pagar la penalidad del 100% del valor del viaje";
                //string tipoServicio = "3";
                if (tipoServicio == "3")
                {
                    //txtTerminos.Text = "Términos y condiciones de la encomienda";
                    DisplayAlert("Atención", "He leído y acepto los términos y condiciones de la encomienda. Me comprometo a tener la encomienda lista al menos una hora antes del horario seleccionado. En caso de no " +
                        "concretarse la encomienda, me responsabilizo por el pago que disponga Peter Tours por las gestiones realizadas.", "Acepto");
                }
                else
                {
                    DisplayAlert("Atención", textoTerminos, "Acepto");
                }

                btnConfirmar.IsEnabled = true;
            }
            else
            {
                btnConfirmar.IsEnabled = false;
                btnConfirmar.BackgroundColor = Colors.LightGray;
            }
        }

        private void btnCupon_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtCupon.Text))
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/DescuentosViajesOutputs/descuentocaduca?nombre=" + txtCupon.Text);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.DescuentosViajesOutput>>(content).FirstOrDefault();

                    if (resultado != null)
                    {
                        comprobarDescuentoAplicado(resultado.dv_id, Preferences.Get("ci", "").ToString(), resultado.dv_porcentaje);
                    }
                    else
                    {
                        DisplayAlert("Atención", "El Cupón no se encuentra disponible!", "Acepto");
                    }
                }
            }
        }

        private void comprobarDescuentoAplicado(int id, string cedulaUsuario, decimal descuento)
        {
            libreriaDatos.DescuentosViajesRealizados datosPrecios;
            datosPrecios = new libreriaDatos.DescuentosViajesRealizados()
            {
                dv_id = id,
                vcd_ci_usuario = cedulaUsuario
            };
            Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/DescuentosViajesRealizadosOutputs/descuentousuario");

            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(datosPrecios);
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(RequestUri, contentJson).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.DescuentosViajesRealizadosOutput>>(content).FirstOrDefault();

                if (resultado == null)
                {
                    decimal descuentoAplicar = (100 - descuento) / 100;
                    decimal calculoDescuento = Decimal.Parse(txtTotal.Text) * descuentoAplicar;
                    txtTotal.Text = calculoDescuento.ToString();
                    txtCuponAplicado.Text = "Descuento del " + (int)Math.Round(descuento, 0) + "%!";
                    btnCupon.IsEnabled = false;
                    txtCupon.IsEnabled = false;
                    btnCupon.BackgroundColor = Colors.LightGray;
                    id_cupon = id;
                }
                else
                {
                    DisplayAlert("Atención", "No puedes usar este cupón más veces!", "Acepto");
                }
            }
        }
        private async Task<bool> insertarViajeConDescuento(int id, string cedulaUsuario)
        {
            try
            {
                var datosPrecios = new libreriaDatos.DescuentosViajesRealizados()
                {
                    dv_id = id,
                    vcd_ci_usuario = cedulaUsuario
                };

                Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/DescuentosViajesRealizadosOutputs/insertdescuentousuario");

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(datosPrecios);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(requestUri, contentJson);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.DescuentosViajesRealizados>>(content)?.FirstOrDefault();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes loguear el error si lo necesitas
                return false;
            }
        }
        public class CitaDinamicaRequest
        {
            public List<PasajeroInfo> Pasajeros { get; set; }
            public DateTime Fecha { get; set; }
            public string Hora { get; set; }
            public string Origen { get; set; }
            public string Destino { get; set; }
            public string Telf { get; set; }
            public string Detalles { get; set; }
            public int Tipo { get; set; }
            public decimal Precio { get; set; }
            public bool Parrilla { get; set; }
            public string CedulaFac { get; set; }
            public string NombreRcFac { get; set; }
            public string CorreoFac { get; set; }
            public string TelefonoFac { get; set; }
            public string DireccionFac { get; set; }
            public int Sentido { get; set; }
            public string SentidoOrigen { get; set; }
            public string SentidoDestino { get; set; }
        }
    }
}
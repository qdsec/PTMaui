using Acr.UserDialogs;
//using Java.Net;
using Newtonsoft.Json;
using PeterTours.Utils;
//using Plugin.Geolocator;
//using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Windows.Devices.Geolocation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors; // para Location
using Microsoft.Maui.Maps; // para Map, Pin, y MapClickedEventArgs
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.ApplicationModel;

namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step3Ruta : ContentPage
    {
        private const string GOOGLE_API_KEY = "AIzaSyB2AYgv8OhthZJUZcu35B6FqfjJt16l-aU";
        //private readonly Geocoder _geocoder = new Geocoder();
        public static string ubicacionRetiro = "";
        public static string ubicacionLlegada = "";
        public static string retiroLat = "";
        public static string retiroLon = "";
        public static string llegadaLat = "";
        public static string llegadaLon = "";
        public static string tarifaPrecio = "";
        public static int contadorPines = 0;
        public static string tipoSelec = "";
        public static double latEspecial = 0;
        public static double lonEspecial = 0;
        public static bool zonasEsp = false;
        double res = 0;
        public static List<libreriaDatos.GeoCoordinateID> listaCoordenadas = new List<libreriaDatos.GeoCoordinateID>();
        public static List<libreriaDatos.CatalogoLugaresOutput> listaLugares = new List<libreriaDatos.CatalogoLugaresOutput>();
        public static List<libreriaDatos.GeoCoordinate> coordinates = new List<libreriaDatos.GeoCoordinate>();
        public int[] idLugares = new int[2];
        private int pasajeroActual = 0; // índice del pasajero que está seleccionando
        private int totalPasajeros = 1; // viene de la pantalla anterior
        private List<PasajeroInfo> pasajeros;
        List<PasajeroInfo> listaPasajeros = step2ConfirmaPasajeroOtro.ListaPasajeros;
        public int numeroOcupantes = (string.IsNullOrEmpty(step1Fecha.ocupantes) || step1Fecha.ocupantes == "0") ? 1 : Int32.Parse(step1Fecha.ocupantes);
        public bool estaActualizando = false;
        public static string tiempoEstimado;
        public List<libreriaDatos.GeoCoordinate> Vertices { get; set; }
        public step3Ruta()
        {
            try
            {
                InitializeComponent();
                lvSugerencias.IsVisible = false;

                tipoSelec = "";

                cargarPosicion();
                seleccionarUbicaciones();
                //testPuntos();
                var navigationPage = Application.Current.MainPage as NavigationPage;
                navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");
                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    pnlDireccion.IsVisible = false;
                //    espacioIos.IsVisible = true;
                //}
                //else
                //{
                //    pnlDireccion.IsVisible = true;
                //    espacioIos.IsVisible = false;
                //}
                //pnlDireccion.IsVisible = true;
                //espacioIos.IsVisible = false;


                listaCoordenadas = new List<libreriaDatos.GeoCoordinateID>();
                consultarPuntos(0);
                //consultarLugares();

                //libreriaDatos.GeoCoordinate comprueba = new libreriaDatos.GeoCoordinate(-0.17237893241397062, -78.48129301788553);

                //foreach (var item in listaCoordenadas)
                //{
                //    if (PointInPolygon(item.GeoCoordenadas, comprueba))
                //    {
                //        Console.WriteLine("La coordenada está dentro del polígono.");
                //    }
                //    else
                //    {
                //        Console.WriteLine("La coordenada está fuera del polígono.");
                //    }
                //}

                pasajeros = listaPasajeros;
                totalPasajeros = pasajeros.Count;
                pasajeroActual = 0;

                // Si es un solo pasajero, el botón siempre activo
                btnUbiDestino.IsEnabled = numeroOcupantes == 1;

                string tipoTrifa = Preferences.Get("tipo", "0").ToString();
                if (tipoTrifa == "3")
                {
                    //DisplayAlert("Atención", $"Selecciona la ruta para su encomienda", "Aceptar");

                }
                else
                {
                    DisplayAlert("Atención", $"Selecciona la ruta para {pasajeros[pasajeroActual].Nombre}", "Aceptar");
                }

            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
        private void btnConfirmarPin_Clicked(object sender, EventArgs e)
        {
            contadorPines++;
            txtDireccion.Placeholder = "Dirección de Llegada";
            mapRutas_MapClicked(null, null);
        }
        //private async Task<string?> ObtenerDireccionAsync(Microsoft.Maui.Devices.Sensors.Location location)
        //{
        //    try
        //    {
        //        var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
        //        var p = placemarks?.FirstOrDefault();

        //        if (p == null)
        //            return null;

        //        // Prioridad de campos (LATAM friendly)
        //        if (!string.IsNullOrWhiteSpace(p.Thoroughfare))
        //            return p.Thoroughfare;

        //        if (!string.IsNullOrWhiteSpace(p.SubLocality))
        //            return p.SubLocality;

        //        if (!string.IsNullOrWhiteSpace(p.Locality))
        //            return p.Locality;

        //        if (!string.IsNullOrWhiteSpace(p.AdminArea))
        //            return p.AdminArea;

        //        return null;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        private async Task<string?> ObtenerDireccionAsync(Microsoft.Maui.Devices.Sensors.Location location)
        {
            try
            {
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
                var p = placemarks?.FirstOrDefault();

                if (p == null)
                    return "APROX";

                bool EsCodigoRaro(string v) =>
                    System.Text.RegularExpressions.Regex.IsMatch(v, @"\w{4}\+\w{3}") || // Plus code
                    System.Text.RegularExpressions.Regex.IsMatch(v, @"^[NS]\d+") ||     // S11E
                    System.Text.RegularExpressions.Regex.IsMatch(v, @"^\d+°");          // coordenadas

                string Limpiar(string? v)
                {
                    if (string.IsNullOrWhiteSpace(v))
                        return string.Empty;

                    v = v.Trim();

                    // conjunciones basura
                    if (v.Length <= 2 && new[] { "y", "o", "&" }
                        .Contains(v.ToLower()))
                        return string.Empty;

                    if (EsCodigoRaro(v))
                        return string.Empty;

                    return v;
                }

                var thoroughfare = Limpiar(p.Thoroughfare);
                var subThoroughfare = Limpiar(p.SubThoroughfare);
                var subLocality = Limpiar(p.SubLocality);
                var locality = Limpiar(p.Locality);
                var admin = Limpiar(p.AdminArea);

                // 🔥 Intersección real
                if (!string.IsNullOrEmpty(thoroughfare) && !string.IsNullOrEmpty(subThoroughfare))
                    return $"{thoroughfare} y {subThoroughfare}";

                // Calle
                if (!string.IsNullOrEmpty(thoroughfare))
                    return thoroughfare;

                // Barrio / parroquia
                if (!string.IsNullOrEmpty(subLocality))
                    return subLocality;

                // Ciudad
                if (!string.IsNullOrEmpty(locality))
                    return locality;

                // Provincia
                if (!string.IsNullOrEmpty(admin))
                    return admin;

                // 🚨 Nada usable → devolver código controlado
                var codigoRaro = new[]
                {
            p.Thoroughfare,
            p.FeatureName,
            p.SubLocality
        }
                .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v) && EsCodigoRaro(v));

                return codigoRaro != null
                    ? $"APROX:{codigoRaro}"
                    : "APROX";
            }
            catch
            {
                return "APROX";
            }
        }

        private async void mapRutas_MapClicked(object sender, MapClickedEventArgs e)
        {
            try
            {
                LimpiarMensajes();

                bool esRutaQ = lblRuta.Text.StartsWith("Q");

                if (esRutaQ)
                    await ProcesarRutaQ(e);
                else
                    await ProcesarRutaNormal(e);
            }
            catch (Exception ex)
            {
                await DisplayAlert(
            "Sucedió un inconveniente",
            "Favor comunicarse con el administrador: "+ex.Message,
            "Acepto");
            }
        }
        private async Task ProcesarRutaNormal(MapClickedEventArgs e)
        {
            if (contadorPines == 0)
                await SeleccionarOrigen(e, esRutaQ: false);
            else if (contadorPines == 1)
                await PrepararSeleccionDestino();
            else if (contadorPines == 2)
                await SeleccionarDestinoNormal(e);
        }
        private async Task ProcesarRutaQ(MapClickedEventArgs e)
        {
            if (contadorPines == 0)
                await SeleccionarOrigen(e, esRutaQ: true);
            else if (contadorPines == 1)
                await PrepararSeleccionDestino();
            else if (contadorPines == 2)
                await SeleccionarDestinoQ(e);
        }
        private void LimpiarMensajes()
        {
            LabelError.Text = "";
            txtDireccion.Text = "";
        }
        private async Task<string> ObtenerDireccion(MapClickedEventArgs e)
        {
            var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
            await geocoder.GetPlacemarksAsync(e.Location);
            return await ObtenerDireccionAsync(e.Location);
        }
        private Pin CrearPin(string label, string address, MapClickedEventArgs e)
        {
            return new Pin
            {
                Type = PinType.Place,
                Label = label,
                Address = address,
                Location = new Microsoft.Maui.Devices.Sensors.Location(
                    e.Location.Latitude,
                    e.Location.Longitude)
            };
        }
        private async Task SeleccionarOrigen(MapClickedEventArgs e, bool esRutaQ)
        {
            btnConfirmarPin.IsVisible = true;
            btnUbiActual.IsVisible = true;

            if (esRutaQ)
                txtDireccion.Placeholder = "Dirección de Llegada";

            string direccion = await ObtenerDireccion(e);
            var pin = CrearPin("Origen", direccion, e);

            retiroLat = e.Location.Latitude.ToString();
            retiroLon = e.Location.Longitude.ToString();

            mapRutas.Pins.RemoveAt(0);
            mapRutas.Pins.Insert(0, pin);

            int index = esRutaQ ? 1 : 0;
            idLugares[index] = idsPuntos(
                Decimal.Parse(retiroLat),
                Decimal.Parse(retiroLon));

            ubicacionRetiro = direccion;
        }
        private async Task PrepararSeleccionDestino()
        {
            contadorPines++;
            btnConfirmarPin.IsVisible = false;
            btnUbiActual.IsVisible = true;

            PosicionCiudades(lblRuta.Text.Split('-')[1]);
            btnUbiDestino.Text = "Volver a seleccionar";

            await DisplayAlert(
                "Seleccione la ubicación de destino",
                "Desplázate sobre el mapa y seleccione pulsando una vez",
                "Acepto");
        }
        private async Task SeleccionarDestinoNormal(MapClickedEventArgs e)
        {
            contadorPines = 1;

            string direccion = await ObtenerDireccion(e);
            var pin = CrearPin("Destino", direccion, e);

            llegadaLat = e.Location.Latitude.ToString();
            llegadaLon = e.Location.Longitude.ToString();

            idLugares[1] = idsPuntos(
                Decimal.Parse(llegadaLat),
                Decimal.Parse(llegadaLon));

            double precio = Double.Parse(
                obtenerTarifa(idLugares[1], idLugares[0]).ToString());

            AplicarPrecioYDestino(pin, direccion, precio);
        }
        private async Task SeleccionarDestinoQ(MapClickedEventArgs e)
        {
            latEspecial = 0;
            lonEspecial = 0;
            contadorPines = 1;

            string direccion = await ObtenerDireccion(e);
            var pin = CrearPin("Destino", direccion, e);

            llegadaLat = e.Location.Latitude.ToString();
            llegadaLon = e.Location.Longitude.ToString();

            idLugares[0] = idsPuntos(
                Decimal.Parse(llegadaLat),
                Decimal.Parse(llegadaLon));

            double precio = Double.Parse(
                obtenerTarifa(idLugares[1], idLugares[0]).ToString());

            AplicarPrecioYDestino(pin, direccion, precio);
        }
        private async Task AplicarPrecioYDestino(Pin pin, string direccion, double precio)
        {
            lblPrecio.Text = precio.ToString();
            tarifaPrecio = precio.ToString();

            mapRutas.Pins.RemoveAt(1);
            mapRutas.Pins.Insert(1, pin);

            ubicacionLlegada = direccion;

            pasajeros[pasajeroActual].RetiroLat = retiroLat;
            pasajeros[pasajeroActual].RetiroLon = retiroLon;
            pasajeros[pasajeroActual].LlegadaLat = llegadaLat;
            pasajeros[pasajeroActual].LlegadaLon = llegadaLon;
            pasajeros[pasajeroActual].Precio = Decimal.Parse(tarifaPrecio);

            await ProcesarRutaSeleccionada(
                pasajeros[pasajeroActual],
                Double.Parse(retiroLat),
                Double.Parse(retiroLon),
                Double.Parse(llegadaLat),
                Double.Parse(llegadaLon),
                Decimal.Parse(tarifaPrecio));
        }

        //private async void mapRutas_MapClicked(object sender, MapClickedEventArgs e)
        //{
        //    try
        //    {
        //        LabelError.Text = "";
        //        txtDireccion.Text = "";


        //        if (!lblRuta.Text.StartsWith("Q"))
        //        {
        //            if (contadorPines == 0)
        //            {
        //                btnConfirmarPin.IsVisible = true;
        //                btnUbiActual.IsVisible = true;
        //                //var addresses = await _geocoder.GetAddressesForPositionAsync(e.Location);
        //                var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
        //                var placemarks = await geocoder.GetPlacemarksAsync(e.Location);

        //                var firstAddress = await ObtenerDireccionAsync(e.Location);

        //                Pin pins = new Pin()
        //                {
        //                    Type = PinType.Place,
        //                    Label = "Origen",
        //                    Address = firstAddress,
        //                    Location = new Microsoft.Maui.Devices.Sensors.Location(e.Location.Latitude, e.Location.Longitude)
        //                };
        //                retiroLat = e.Location.Latitude.ToString();
        //                retiroLon = e.Location.Longitude.ToString();

        //                mapRutas.Pins.RemoveAt(0);
        //                mapRutas.Pins.Insert(0, pins);

        //                idLugares[0] = idsPuntos(Decimal.Parse(e.Location.Latitude.ToString()), Decimal.Parse(e.Location.Longitude.ToString()));
        //                ubicacionRetiro = firstAddress;
        //            }
        //            if (contadorPines == 1)
        //            {
        //                contadorPines++;
        //                btnConfirmarPin.IsVisible = false;
        //                btnUbiActual.IsVisible = true;
        //                //mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
        //                //                                     Distance.FromMiles(1)));
        //                PosicionCiudades(lblRuta.Text.Split('-')[1]);
        //                btnUbiDestino.Text = "Volver a seleccionar";
        //                //btnUbiDestino.HeightRequest = 40;
        //                //await DisplayAlert("Atención", "Favor seleccione la ubicación de destino en el mapa", "Acepto");
        //                string tipoTrifa = Preferences.Get("tipo", "0").ToString();
        //                if (tipoTrifa == "3")//viaje compartido
        //                {
        //                    await DisplayAlert("Seleccione la ubicación de destino", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
        //                }
        //                else
        //                {
        //                    await DisplayAlert("Seleccione la ubicación de destino", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
        //                }
        //            }
        //            else if (contadorPines == 2)
        //            {
        //                LabelError.Text = "";
        //                contadorPines = 1;
        //                //var addresses = await _geocoder.GetAddressesForPositionAsync(e.Location);
        //                var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
        //                var placemarks = await geocoder.GetPlacemarksAsync(e.Location);

        //                var firstAddress = await ObtenerDireccionAsync(e.Location);

        //                Pin pins = new Pin()
        //                {
        //                    Type = PinType.Place,
        //                    Label = "Destino",
        //                    Address = firstAddress,
        //                    Location = new Microsoft.Maui.Devices.Sensors.Location(e.Location.Latitude, e.Location.Longitude)
        //                };

        //                double res = 0;
        //                idLugares[1] = idsPuntos(Decimal.Parse(e.Location.Latitude.ToString()), Decimal.Parse(e.Location.Longitude.ToString()));
        //                res = Double.Parse(obtenerTarifa(idLugares[1], idLugares[0]).ToString());
        //                //double res = obtenerPrecio(e.Location.Latitude, e.Location.Longitude);
        //                //DisplayAlert("test", "El precio es " + res.ToString(), "aceptar");
        //                lblPrecio.Text = res.ToString();
        //                tarifaPrecio = res.ToString();
        //                llegadaLat = e.Location.Latitude.ToString();
        //                llegadaLon = e.Location.Longitude.ToString();
        //                //if (CompruebaZonasEspeciales(double.Parse(llegadaLat), double.Parse(llegadaLon)))
        //                //{
        //                //    latEspecial = double.Parse(llegadaLat);
        //                //    lonEspecial = double.Parse(llegadaLon);
        //                //}
        //                //mapRutas.Pins[1] = pins;
        //                mapRutas.Pins.RemoveAt(1);
        //                mapRutas.Pins.Insert(1, pins);
        //                ubicacionLlegada = firstAddress;
        //                //if (res <= 0)
        //                //{
        //                //    // await DisplayAlert("Atención", "El precio de su destino no está definido, por favor realice su reserva por Whatsapp", "Cerrar");
        //                //}
        //                //else
        //                //{
        //                //await DisplayAlert("Atención", "La ruta ha sido seleccionada con éxito", "Acepto");
        //                pasajeros[pasajeroActual].RetiroLat = retiroLat;
        //                pasajeros[pasajeroActual].RetiroLon = retiroLon;
        //                pasajeros[pasajeroActual].LlegadaLat = llegadaLat;
        //                pasajeros[pasajeroActual].LlegadaLon = llegadaLon;
        //                pasajeros[pasajeroActual].Precio = Decimal.Parse(tarifaPrecio);

        //                await ProcesarRutaSeleccionada(pasajeros[pasajeroActual], Double.Parse(retiroLat), Double.Parse(retiroLon), Double.Parse(llegadaLat), Double.Parse(llegadaLon), Decimal.Parse(tarifaPrecio));
        //                //}

        //            }
        //        }
        //        else if (lblRuta.Text.StartsWith("Q"))
        //        {
        //            if (contadorPines == 0)
        //            {
        //                btnConfirmarPin.IsVisible = true;
        //                btnUbiActual.IsVisible = true;
        //                txtDireccion.Placeholder = "Dirección de Llegada";
        //                //var addresses = await _geocoder.GetAddressesForPositionAsync(e.Location);
        //                var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
        //                var placemarks = await geocoder.GetPlacemarksAsync(e.Location);

        //                var firstAddress = await ObtenerDireccionAsync(e.Location);

        //                Pin pins = new Pin()
        //                {
        //                    Type = PinType.Place,
        //                    Label = "Origen",
        //                    Address = firstAddress,
        //                    Location = new Microsoft.Maui.Devices.Sensors.Location(e.Location.Latitude, e.Location.Longitude)
        //                };
        //                retiroLat = e.Location.Latitude.ToString();
        //                retiroLon = e.Location.Longitude.ToString();
        //                //res = obtenerPrecio(e.Location.Latitude, e.Location.Longitude);
        //                //if (CompruebaZonasEspeciales(double.Parse(retiroLat), double.Parse(retiroLon)))
        //                //{
        //                //    latEspecial = double.Parse(retiroLat);
        //                //    lonEspecial = double.Parse(retiroLon);
        //                //}
        //                //mapRutas.Pins[0] = pins;
        //                mapRutas.Pins.RemoveAt(0);
        //                mapRutas.Pins.Insert(0, pins);
        //                idLugares[1] = idsPuntos(Decimal.Parse(e.Location.Latitude.ToString()), Decimal.Parse(e.Location.Longitude.ToString()));
        //                ubicacionRetiro = firstAddress;
        //            }
        //            if (contadorPines == 1)
        //            {

        //                contadorPines++;
        //                btnConfirmarPin.IsVisible = false;
        //                btnUbiActual.IsVisible = true;
        //                //mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.34857310182349616, -78.12607822936435),
        //                //                                     Distance.FromMiles(1)));
        //                PosicionCiudades(lblRuta.Text.Split('-')[1]);
        //                btnUbiDestino.Text = "Volver a seleccionar";
        //                //btnUbiDestino.HeightRequest = 40;
        //                //await DisplayAlert("Atención", "Favor seleccione la ubicación de destino en el mapa", "Acepto");
        //                string tipoTrifa = Preferences.Get("tipo", "0").ToString();
        //                if (tipoTrifa == "3")//viaje compartido
        //                {
        //                    await DisplayAlert("Seleccione la ubicación de destino", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
        //                }
        //                else
        //                {
        //                    await DisplayAlert("Seleccione la ubicación de destino", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
        //                }
        //            }
        //            else if (contadorPines == 2)
        //            {
        //                latEspecial = 0;
        //                lonEspecial = 0;
        //                LabelError.Text = "";
        //                contadorPines = 1;
        //                //var addresses = await _geocoder.GetAddressesForPositionAsync(e.Location);

        //                var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
        //                var placemarks = await geocoder.GetPlacemarksAsync(e.Location);

        //                var firstAddress = await ObtenerDireccionAsync(e.Location);

        //                Pin pins = new Pin()
        //                {
        //                    Type = PinType.Place,
        //                    Label = "Destino",
        //                    Address = firstAddress,
        //                    Location = new Microsoft.Maui.Devices.Sensors.Location(e.Location.Latitude, e.Location.Longitude)
        //                };

        //                //DisplayAlert("test", "El precio es " + res.ToString(), "aceptar");
        //                llegadaLat = e.Location.Latitude.ToString();
        //                llegadaLon = e.Location.Longitude.ToString();

        //                res = 0;
        //                idLugares[0] = idsPuntos(Decimal.Parse(e.Location.Latitude.ToString()), Decimal.Parse(e.Location.Longitude.ToString()));

        //                res = Double.Parse(obtenerTarifa(idLugares[1], idLugares[0]).ToString());
        //                //res = obtenerPrecio(double.Parse(retiroLat), double.Parse(retiroLon));
        //                lblPrecio.Text = res.ToString();
        //                tarifaPrecio = res.ToString();

        //                //mapRutas.Pins[1] = pins;
        //                mapRutas.Pins.RemoveAt(1);
        //                mapRutas.Pins.Insert(1, pins);
        //                ubicacionLlegada = firstAddress;
        //                //if (res <= 0)
        //                //{
        //                //    //await DisplayAlert("Atención", "El precio de su destino no está definido, por favor realice su reserva por Whatsapp", "Cerrar");
        //                //}
        //                //else
        //                //{
        //                //await DisplayAlert("Atención", "La ruta ha sido seleccionada con éxito", "Acepto");
        //                pasajeros[pasajeroActual].RetiroLat = retiroLat;
        //                pasajeros[pasajeroActual].RetiroLon = retiroLon;
        //                pasajeros[pasajeroActual].LlegadaLat = llegadaLat;
        //                pasajeros[pasajeroActual].LlegadaLon = llegadaLon;
        //                pasajeros[pasajeroActual].Precio = Decimal.Parse(tarifaPrecio);
        //                await ProcesarRutaSeleccionada(pasajeros[pasajeroActual], Double.Parse(retiroLat), Double.Parse(retiroLon), Double.Parse(llegadaLat), Double.Parse(llegadaLon), Decimal.Parse(tarifaPrecio));

        //                // aquí ya puedes pasar la lista completa a la siguiente pantalla
        //                //}

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }


        //}

        public async void finalizarMapa(List<PasajeroInfo> pasajero)
        {
            try
            {
                if (numeroOcupantes > 0)
                {
                    pnlPrecio.IsVisible = false;
                    pnlResumen.IsVisible = true;
                    listaResumen.Children.Clear();

                    decimal total = 0;

                    string tipoTrifa = Preferences.Get("tipo", "0").ToString();
                    if (tipoTrifa == "1")
                    {
                        // ➝ Sumatoria de todos los precios
                        foreach (var p in pasajeros)
                            total += p.Precio;
                    }
                    else
                    {
                        // ➝ Tomar solo el valor mayor
                        total = pasajeros.Max(p => p.Precio);
                    }


                    foreach (var p in pasajeros)
                    {
                        decimal valorMostrar;
                        string textoMostrar;

                        if (tipoTrifa == "1")
                        {
                            valorMostrar = p.Precio; // cada uno su valor
                        }
                        else
                        {
                            valorMostrar = total; // todos muestran el mayor
                        }


                        if (tipoTrifa == "3")
                        {
                            if (p.Precio == 0)
                            {
                                textoMostrar = $"Encomienda: Selección incorrecta o no definida";
                            }
                            else
                            {
                                textoMostrar = $"Encomienda: Selección correcta";
                            }
                        }
                        else
                        {
                            if (p.Precio == 0)
                            {
                                textoMostrar = $"{p.Nombre}: Selección incorrecta o no definida";
                            }
                            else
                            {
                                textoMostrar = $"{p.Nombre}: Selección correcta";
                            }
                        }

                        var frame = new Frame
                        {
                            Padding = new Thickness(10, 5),
                            Margin = new Thickness(0, 3),
                            CornerRadius = 10,
                            BorderColor = Colors.LightGray,
                            BackgroundColor = Colors.White,
                            Content = new Label
                            {
                                Text = textoMostrar,
                                FontSize = 15,
                                TextColor = Colors.Black,
                                HorizontalOptions = LayoutOptions.Center
                            }
                        };

                        var tap = new TapGestureRecognizer();
                        tap.Tapped += (s, z) =>
                        {
                            estaActualizando = true;
                            // resetear todos los frames
                            foreach (var child in listaResumen.Children.OfType<Frame>())
                                child.BackgroundColor = Colors.White;

                            frame.BackgroundColor = Color.FromHex("#E0F7FA");

                            // asignar el pasajero seleccionado globalmente
                            pasajeroActual = pasajeros.IndexOf(p);

                            limpiarUbicacionPrevia();

                            if (double.TryParse(p.RetiroLat, out double retiroLat) &&
                                double.TryParse(p.RetiroLon, out double retiroLon) &&
                                double.TryParse(p.LlegadaLat, out double llegadaLat) &&
                                double.TryParse(p.LlegadaLon, out double llegadaLon))
                            {
                                mapRutas.Pins.Add(new Pin
                                {
                                    Label = $"Origen {p.Nombre}",
                                    Location = new Microsoft.Maui.Devices.Sensors.Location(retiroLat, retiroLon)
                                });

                                mapRutas.Pins.Add(new Pin
                                {
                                    Label = $"Destino {p.Nombre}",
                                    Location = new Microsoft.Maui.Devices.Sensors.Location(llegadaLat, llegadaLon)
                                });

                                CentrarRuta(retiroLat, retiroLon, llegadaLat, llegadaLon);
                            }

                            // activar botón “Volver a seleccionar” si hay varios pasajeros
                            if (numeroOcupantes > 1)
                                btnUbiDestino.IsEnabled = true;
                        };


                        frame.GestureRecognizers.Add(tap);
                        listaResumen.Children.Add(frame);
                    }
                    bool viajecorrecto = false;
                    lblTotal.Text = $"TOTAL: ${total:F2}";
                    foreach (var p in pasajeros)
                    {
                        if (p.Precio <= 0)
                        {

                            //await DisplayAlert("Atención", "Uno o más recorridos son incorrectos, favor elija en la lista el/los que tienen 0 y vuelva a seleccionar", "Aceptar");
                            btnNext.IsVisible = false;
                            viajecorrecto = false;
                            break;
                        }
                        else
                        {
                            viajecorrecto = true;

                        }
                    }
                    if (viajecorrecto)
                    {
                        if (numeroOcupantes == 1)
                        {
                            //await DisplayAlert("Completado", "Se ha registrado su ruta correctamente", "Aceptar");
                            btnNext.IsVisible = true;
                        }
                        else
                        {
                            //await DisplayAlert("Completado", "Se han registrado las rutas de todos los pasajeros correctamente", "Aceptar");
                            btnNext.IsVisible = true;
                        }
                        btnUbiActual.IsVisible = false;
                        tarifaPrecio = total.ToString();
                        LoadingService.Show("Cargando");
                        await DibujarRutaCombinadaAsync();

                        LoadingService.Hide();
                        // Esperar un ciclo de renderizado para que ScrollView conozca la posición del botón
                        //await Task.Delay(50);

                        // Hacer scroll hacia el botón
                        //await scrollPrincipal.ScrollToAsync(btnNext, ScrollToPosition.End, true);
                        //await scrollPrincipal.ScrollToAsync(btnNext, ScrollToPosition.MakeVisible, true);
                        //Dispatcher.Dispatch(async () =>
                        //{
                        //    await Task.Delay(200);
                        //    await scrollPrincipal.ScrollToAsync(scrollTarget, ScrollToPosition.End, true);
                        //});
                        //Dispatcher.Dispatch(async () =>
                        //{
                        //    await Task.Delay(50);
                        //    focusNextAnchor.Focus();
                        //});




                    }
                    else
                    {

                        await MostrarAlertaAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                LoadingService.Hide();

            }
        }
        public async Task MostrarAlertaAsync()
        {
            //bool respuesta = await Application.Current.MainPage.DisplayAlert(
            //    "Atención",
            //    "Una o más ubicaciones están fuera de nuestro perímetro. Por favor vuelva a seleccionar si ocurrió un error o comuníquese con un asesor mediante WhatsApp para ayudarle con su viaje.",
            //    "Ir a WhatsApp",
            //    "Aceptar"
            //);
            bool respuesta = await Application.Current.MainPage.DisplayAlert(
            "Atención",
            "Tu selección se encuentra fuera de nuestro perímetro de operación o podría tratarse de un error. Por favor, comunícate con uno de nuestros asesores para ayudarte a coordinar tu viaje.",
            "Ir a WhatsApp",
            "Aceptar"
        );

            if (respuesta)
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
            else
            {
                // Acción si el usuario presiona "Aceptar" (solo cerrar el mensaje)
                btnUbiDestino.IsEnabled = true;
            }
        }

        private async Task ProcesarRutaSeleccionada(PasajeroInfo pasajero, double retiroLat, double retiroLon, double llegadaLat, double llegadaLon, decimal precio)
        {
            // Actualizar pasajero actual
            pasajero.RetiroLat = retiroLat.ToString();
            pasajero.RetiroLon = retiroLon.ToString();
            pasajero.LlegadaLat = llegadaLat.ToString();
            pasajero.LlegadaLon = llegadaLon.ToString();
            pasajero.Precio = precio;

            // Si estamos actualizando un pasajero, solo hacemos update y salimos
            if (estaActualizando)
            {
                finalizarMapa(pasajeros);
                return;
            }

            // Avanzamos al siguiente pasajero
            while (pasajeroActual < totalPasajeros - 1)
            {
                var siguiente = pasajeros[pasajeroActual + 1];

                bool mismaRuta = await DisplayAlert(
                    "Siguiente pasajero",
                    $"¿El pasajero {siguiente.Nombre} tiene el mismo retiro y destino?",
                    "Sí", "No"
                );

                pasajeroActual++;

                if (mismaRuta)
                {
                    // Copiar datos del pasajero anterior
                    siguiente.RetiroLat = pasajero.RetiroLat;
                    siguiente.RetiroLon = pasajero.RetiroLon;
                    siguiente.LlegadaLat = pasajero.LlegadaLat;
                    siguiente.LlegadaLon = pasajero.LlegadaLon;
                    siguiente.Precio = pasajero.Precio;

                    await DisplayAlert("Listo", $"Ruta del pasajero {siguiente.Nombre} registrada con la misma ubicación", "Aceptar");

                    // Preparamos al siguiente pasajero en el bucle
                    pasajero = siguiente;
                }
                else
                {
                    limpiarUbicacionPrevia();
                    await DisplayAlert("Atención", $"Selecciona la ruta para {siguiente.Nombre}", "Aceptar");
                    btnUbiActual.IsVisible = true;
                    return; // Esperamos a que seleccione en el mapa antes de seguir
                }
            }

            // Si llegamos aquí, todos los pasajeros ya tienen ruta
            finalizarMapa(pasajeros);
        }




        private void CentrarRuta(double lat1, double lon1, double lat2, double lon2)
        {
            // calcular centro
            var centerLat = (lat1 + lat2) / 2;
            var centerLon = (lon1 + lon2) / 2;

            // calcular distancia aprox entre puntos
            var distancia = Distance.BetweenPositions(new Microsoft.Maui.Devices.Sensors.Location(lat1, lon1), new Microsoft.Maui.Devices.Sensors.Location(lat2, lon2));

            // mover cámara con radio un poquito mayor a la distancia
            mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(
               new Microsoft.Maui.Devices.Sensors.Location(centerLat, centerLon),
                Distance.FromKilometers(Math.Max(1, distancia.Kilometers * 1.2))
            ));

            btnUbiActual.IsEnabled = true;
        }
        private void cargarPosicion()
        {
            try
            {
                lblRuta.Text = step0Sentido.tipoViaje;
                if (lblRuta.Text == "Ibarra-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.34857310182349616, -78.12607822936435),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Ibarra")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Atuntaqui-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.3320341204286948, -78.21864371911067),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Atuntaqui")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Otavalo-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.23527194260727738, -78.26138752270404),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Otavalo")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Cotacachi-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.30066352459266893, -78.26535708663006),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Cotacachi")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Cayambe-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.04194981593865252, -78.14432665374176),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Cayambe")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Ibarra-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.34857310182349616, -78.12607822936435),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Atuntaqui-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.3320341204286948, -78.21864371911067),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Otavalo-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.23527194260727738, -78.26138752270404),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Cotacachi-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.30066352459266893, -78.26535708663006),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Cayambe-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.04194981593865252, -78.14432665374176),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Ibarra")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Atuntaqui")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Otavalo")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Cotacachi")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Cayambe")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Aeropuerto-Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                else if (lblRuta.Text == "Quito-Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }

                //var locator = CrossGeolocator.Current;
                //var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1));
                //mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(position.Latitude, position.Longitude),
                //                                             Distance.FromMiles(0.2)));

            }
            catch (Exception ex)
            {
                DisplayAlert("Atención", "Favor encienda el gps para seleccionar su ubicación actual", "Acepto");
            }

        }

        private async void PosicionCiudades(string ruta)
        {
            try
            {
                //ruta = step0Sentido.tipoViaje;
                if (ruta == "Ibarra")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.34857310182349616, -78.12607822936435),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Quito")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.18224172663531893, -78.47340015676862),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Atuntaqui")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.3320341204286948, -78.21864371911067),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Otavalo")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.23527194260727738, -78.26138752270404),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Cotacachi")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.30066352459266893, -78.26535708663006),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Cayambe")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.04194981593865252, -78.14432665374176),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Cayambe")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(0.04194981593865252, -78.14432665374176),
                                                             Distance.FromMiles(1)));
                }
                else if (ruta == "Aeropuerto")
                {
                    mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(-0.125115, -78.354727),
                                                             Distance.FromMiles(1)));
                }
                //var locator = CrossGeolocator.Current;
                //var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1));
                //mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Devices.Sensors.Location(position.Latitude, position.Longitude),
                //                                             Distance.FromMiles(0.2)));

            }
            catch (Exception ex)
            {
                await DisplayAlert("Atención", "Favor encienda el gps para seleccionar su ubicación actual", "Acepto");
            }

        }

        private void seleccionarUbicaciones()
        {
            LabelError.Text = "";
            lblPrecio.Text = "";
            contadorPines = 0;
            mapRutas.Pins.Clear();
            //Pin pinOrigen = new Pin()
            //{
            //    Label = "Origen"
            //};
            //Pin pinDestino = new Pin()
            //{
            //    Label = "Destino"
            //};
            ////mapRutas.Pins.Add(pinOrigen);
            ////mapRutas.Pins.Add(pinDestino);
            ///
            var defaultLocation = new Microsoft.Maui.Devices.Sensors.Location(0, 0); // o cualquier coordenada

            Pin pinOrigen = new Pin()
            {
                Label = "Origen",
                Location = defaultLocation
            };

            Pin pinDestino = new Pin()
            {
                Label = "Destino",
                Location = defaultLocation
            };

            mapRutas.Pins.Clear();
            mapRutas.Pins.Add(pinOrigen);
            mapRutas.Pins.Add(pinDestino);
            tipoSelec = "doble";

            if (btnUbiDestino.Text == "Volver a seleccionar")
            {
                cargarPosicion();
            }

            string tipoTrifa = Preferences.Get("tipo", "0").ToString();
            if (tipoTrifa == "3")//viaje compartido
            {
                DisplayAlert("Seleccione la ubicación de retiro", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
            }
            else
            {
                DisplayAlert("Seleccione la ubicación de origen", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
            }
        }

        private void limpiarUbicacionPrevia()
        {
            LabelError.Text = "";
            lblPrecio.Text = "";
            contadorPines = 0;

            mapRutas.Pins.Clear();

            // MAUI exige Location obligatoria
            var pinOrigen = new Pin
            {
                Label = "Origen",
                Location = new Microsoft.Maui.Devices.Sensors.Location(0, 0)
            };

            var pinDestino = new Pin
            {
                Label = "Destino",
                Location = new Microsoft.Maui.Devices.Sensors.Location(0, 0)
            };

            mapRutas.Pins.Add(pinOrigen);
            mapRutas.Pins.Add(pinDestino);

            tipoSelec = "doble";


            if (btnUbiDestino.Text == "Volver a seleccionar")
            {
                cargarPosicion();
            }
        }


        //private async void btnUbiActual_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LoadingService.Show("Cargando");

        //        // Verificamos si la geolocalización está disponible
        //        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
        //        var location = await Geolocation.Default.GetLocationAsync(request);

        //        if (location != null)
        //        {
        //            // Movemos el mapa a la posición actual
        //            mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(
        //                new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude),
        //                Distance.FromMeters(200)
        //            ));

        //            //var addresses = await _geocoder.GetAddressesForPositionAsync(new Microsoft.Maui.Devices.Sensors.Location(position.Latitude, position.Longitude));

        //            var geocoder = Microsoft.Maui.Devices.Sensors.Geocoding.Default;
        //            var placemarks = await geocoder.GetPlacemarksAsync(new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude));

        //            var firstAddres = placemarks?.FirstOrDefault()?.Thoroughfare ?? "Dirección no encontrada";

        //            Pin pins = new Pin()
        //            {
        //                Type = PinType.Place,
        //                Label = "Origen",
        //                Address = firstAddres,
        //                Location = new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude)
        //            };
        //            retiroLat = location.Latitude.ToString();
        //            retiroLon = location.Longitude.ToString();

        //            //mapRutas.Pins[0] = pins;
        //            mapRutas.Pins.RemoveAt(0);
        //            mapRutas.Pins.Insert(0, pins);

        //            idLugares[0] = idsPuntos(Decimal.Parse(location.Latitude.ToString()), Decimal.Parse(location.Longitude.ToString()));
        //            ubicacionRetiro = firstAddres;

        //            btnConfirmarPin.IsVisible = true;

        //            LoadingService.Hide();
        //        }
        //        else
        //        {
        //            await DisplayAlert("Atención", "Favor encienda el gps para seleccionar su ubicación actual", "Acepto");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Atención", "Favor encienda el gps para seleccionar su ubicación actual", "Acepto");
        //    }
        //}
        private async void btnUbiActual_Clicked(object sender, EventArgs e)
        {
            try
            {

                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location == null)
                {
                    await DisplayAlert("Atención", "Favor encienda el GPS para seleccionar su ubicación actual", "Acepto");

                    return;
                }


                mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(
    new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude),
    Distance.FromMeters(200)
));

                LoadingService.Show("Cargando");
                var simulatedArgs = new Microsoft.Maui.Controls.Maps.MapClickedEventArgs(
                    new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude)
                );

                // Llamamos al mismo método del mapa
                mapRutas_MapClicked(mapRutas, simulatedArgs);

                LoadingService.Hide();
            }
            catch
            {
                await DisplayAlert("Atención", "Favor encienda el gps para seleccionar su ubicación actual", "Acepto");
            }
        }



        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");

            string tipoTrifa = Preferences.Get("tipo", "0").ToString();

            if (!String.IsNullOrWhiteSpace(lblPrecio.Text))
            {
                if (lblPrecio.Text != "0")
                {
                    LabelError.Text = "";
                    if (tipoTrifa != "3")
                    {
                        await NavigationHelper.SafePushAsync(Navigation, new step3tEspecial());
                    }
                    else
                    {
                        await NavigationHelper.SafePushAsync(Navigation, new step3Facturacion());
                    }
                }
                else
                {
                    //LabelError.Text = "El precio de su destino no es";
                    await DisplayAlert("Atención", "El precio de su destino no está definido, por favor realice su reserva por Whatsapp", "Cerrar");
                }
            }
            else
            {
                LabelError.Text = "Favor elija el destino de su viaje";
            }
            LoadingService.Hide();

        }

        public async void testPuntos()
        {
            LoadingService.Show("Cargando");
            //consultarLugares();
            consultarPuntos(0);
            LoadingService.Hide();

        }

        public async Task<decimal> calcularPrecio(int idInicio, int idFin)
        {
            string tipoTrifa = Preferences.Get("tipo", "0").ToString();
            if (tipoTrifa == "1")
            {
                libreriaDatos.PreciosLugaresOutput datos = new libreriaDatos.PreciosLugaresOutput()
                {
                    cl_id = idInicio,
                    lc_id = idFin

                };
                Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresPreciosOutputs");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(datos);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(RequestUri, contentJson);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<libreriaDatos.PreciosLugaresOutput>(content);

                    return resultado.plc_tarifa;


                    //App.Current.Logout();
                }
            }
            else
            {
                libreriaDatos.PreciosLugaresOutput datos = new libreriaDatos.PreciosLugaresOutput()
                {
                    cl_id = idInicio,
                    lc_id = idFin,
                    plc_estado = Int32.Parse(tipoTrifa)
                };
                Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresPreciosOutputs/precios-extra");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(datos);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, contentJson);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<libreriaDatos.PreciosLugaresOutput>(content);

                    return resultado.plc_tarifa;


                    //App.Current.Logout();
                }
            }
            return 0;
        }

        private async void btnUbiDestino_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            btnNext.IsVisible = false;
            btnConfirmarPin.IsVisible = false;
            btnUbiActual.IsVisible = true;
            LabelError.Text = "";
            lblPrecio.Text = "";
            contadorPines = 0;
            latEspecial = 0;
            lonEspecial = 0;
            mapRutas.Pins.Clear();
            txtDireccion.Placeholder = "Dirección de Retiro";
            txtDireccion.Text = "";
            idLugares = new int[2];
            //Pin pinOrigen = new Pin()
            //{
            //    Label = "Origen"
            //};
            //Pin pinDestino = new Pin()
            //{
            //    Label = "Destino"
            //};
            //mapRutas.Pins.Add(pinOrigen);
            //mapRutas.Pins.Add(pinDestino);
            mapRutas.Pins.Clear();
            //Pin pinOrigen = new Pin()
            //{
            //    Label = "Origen"
            //};
            //Pin pinDestino = new Pin()
            //{
            //    Label = "Destino"
            //};
            ////mapRutas.Pins.Add(pinOrigen);
            ////mapRutas.Pins.Add(pinDestino);
            ///
            var defaultLocation = new Microsoft.Maui.Devices.Sensors.Location(0, 0); // o cualquier coordenada

            Pin pinOrigen = new Pin()
            {
                Label = "Origen",
                Location = defaultLocation
            };

            Pin pinDestino = new Pin()
            {
                Label = "Destino",
                Location = defaultLocation
            };

            mapRutas.Pins.Clear();
            mapRutas.Pins.Add(pinOrigen);
            mapRutas.Pins.Add(pinDestino);
            tipoSelec = "doble";

            if (btnUbiDestino.Text == "Volver a seleccionar")
            {
                cargarPosicion();
            }
            btnUbiDestino.IsEnabled = false;

            LimpiarRuta();
            LoadingService.Hide(); string tipoTrifa = Preferences.Get("tipo", "0").ToString();
            if (tipoTrifa == "3")//viaje compartido
            {
                await DisplayAlert("Seleccione la ubicación de retiro", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
            }
            else
            {
                await DisplayAlert("Seleccione la ubicación de origen", "Desplázate sobre el mapa y seleccione pulsando una vez", "Acepto");
            }
        }

        private async void btnDireccion_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingService.Show("Cargando");

                if (!string.IsNullOrWhiteSpace(txtDireccion.Text))
                {
                    var direccion = $"{txtDireccion.Text}, Ecuador";
                    var resultado = await Geocoding.GetLocationsAsync(direccion);

                    if (resultado?.Any() == true)
                    {
                        var location = resultado.First();
                        var posicion = new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude);

                        mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(posicion, Distance.FromMiles(0.2)));
                    }
                }

                LoadingService.Hide();
            }
            catch (Exception ex)
            {
                LoadingService.Hide();
                // Log o alerta si deseas
            }
        }


        //public bool PointInPolygon(List<libreriaDatos.GeoCoordinate> polygon, libreriaDatos.GeoCoordinate coordinate)
        //{
        //    int i, j;
        //    bool isInside = false;

        //    for (i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        //    {
        //        if (((polygon[i].Latitude > coordinate.Latitude) != (polygon[j].Latitude > coordinate.Latitude)) &&
        //            (coordinate.Longitude < (polygon[j].Longitude - polygon[i].Longitude) * (coordinate.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude))
        //        {
        //            isInside = true;
        //            return isInside;
        //        }
        //    }

        //    return isInside;
        //}

        public bool PointInPolygon(List<libreriaDatos.GeoCoordinate> polygon, libreriaDatos.GeoCoordinate coordinate)
        {
            int windingNumber = 0;

            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (((polygon[i].Latitude > coordinate.Latitude) != (polygon[j].Latitude > coordinate.Latitude)) &&
                    (coordinate.Longitude < (polygon[j].Longitude - polygon[i].Longitude) * (coordinate.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude))
                {
                    windingNumber++;
                }
            }

            return windingNumber % 2 == 1;
        }
        public int idsPuntos(decimal latitud, decimal longitud)
        {
            int idPunto = 0;

            // resultadoTarifa = await PointInPolygon();
            libreriaDatos.GeoCoordinate comprueba = new libreriaDatos.GeoCoordinate(latitud, longitud);

            foreach (var item in listaCoordenadas)
            {
                if (PointInPolygon(item.GeoCoordenadas, comprueba))
                {
                    //DisplayAlert("Atención", item.Nombre +" "+item.ID, "Acepto");
                    idPunto = item.ID;
                    return idPunto;
                }
            }
            //            bool estaDentro =PointInPolygon()

            return idPunto;
        }

        //public bool PointInPolygon(List<libreriaDatos.GeoCoordinate> polygon, libreriaDatos.GeoCoordinate coordinate)
        //{
        //    int i, j;
        //    bool isInside = false;

        //    for (i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        //    {
        //        if (((polygon[i].Latitude > coordinate.Latitude) != (polygon[j].Latitude > coordinate.Latitude)) &&
        //            (coordinate.Longitud
        //            e < (polygon[j].Longitude - polygon[i].Longitude) * (coordinate.Latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude))
        //        {
        //            isInside = !isInside;
        //        }
        //    }

        //    return isInside;
        //}

        //private void consultarLugares()
        //{
        //    var request = new HttpRequestMessage();
        //    request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresCatalogoOutputs");
        //    request.Method = HttpMethod.Get;
        //    request.Headers.Add("Accept", "application/json");
        //    var client = new HttpClient();
        //    HttpResponseMessage response = client.SendAsync(request).Result;
        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        string content = response.Content.ReadAsStringAsync().Result;
        //        var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.CatalogoLugaresOutput>>(content);
        //        foreach (var item in resultado)
        //        {
        //            consultarPuntos(item.cl_id);
        //            listaCoordenadas.Add(new libreriaDatos.GeoCoordinateID(item.cl_id, item.cl_nombre, coordinates));
        //        }
        //    }
        //}
        private void consultarLugares()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresCatalogoOutputs");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                listaLugares = JsonConvert.DeserializeObject<List<libreriaDatos.CatalogoLugaresOutput>>(content);
            }
        }

        private void consultarPuntos(int idLugar)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresViajesOutputs/" + idLugar);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.LugaresViajesOutput>>(content);
                if (idLugar != 0)
                {
                    coordinates = new List<libreriaDatos.GeoCoordinate>();
                    int cont = 1;
                    foreach (var item in resultado)
                    {
                        decimal lat = Decimal.Parse(item.lc_latitud, CultureInfo.InvariantCulture);
                        decimal lon = Decimal.Parse(item.lc_longitud, CultureInfo.InvariantCulture);
                        coordinates.Add(new libreriaDatos.GeoCoordinate(lat, lon));

                    }
                }
                else
                {
                    //var agrupados = resultado.GroupBy(x => x.cl_id);

                    //foreach (var grupo in agrupados)
                    //{
                    //    var coordinates = new List<libreriaDatos.GeoCoordinate>();

                    //    foreach (var item in grupo)
                    //    {
                    //        decimal lat = decimal.Parse(item.lc_latitud, CultureInfo.InvariantCulture);
                    //        decimal lon = decimal.Parse(item.lc_longitud, CultureInfo.InvariantCulture);
                    //        coordinates.Add(new libreriaDatos.GeoCoordinate(lat, lon));
                    //    }

                    //    string nombre = grupo.First().cl_nombre; // Toma el primer nombre
                    //    listaCoordenadas.Add(new libreriaDatos.GeoCoordinateID(grupo.Key, nombre, coordinates));
                    //}
                    consultarLugares();

                    foreach (var lugar in listaLugares)
                    {
                        var coordenadas = new List<libreriaDatos.GeoCoordinate>();

                        var puntosDelLugar = resultado
                            .Where(x => x.cl_id == lugar.cl_id)
                            .ToList();

                        foreach (var punto in puntosDelLugar)
                        {
                            decimal lat = decimal.Parse(punto.lc_latitud, CultureInfo.InvariantCulture);
                            decimal lon = decimal.Parse(punto.lc_longitud, CultureInfo.InvariantCulture);
                            coordenadas.Add(new libreriaDatos.GeoCoordinate(lat, lon));
                        }

                        listaCoordenadas.Add(new libreriaDatos.GeoCoordinateID(lugar.cl_id, lugar.cl_nombre, coordenadas));
                    }
                }
            }
        }

        private decimal obtenerTarifa(int idRetiro, int idLlegada)
        {
            string tipoTrifa = Preferences.Get("tipo", "0").ToString();
            if (tipoTrifa == "1")
            {
                libreriaDatos.PreciosLugares datosPrecios;
                datosPrecios = new libreriaDatos.PreciosLugares()
                {
                    cl_id = idRetiro,
                    lc_id = idLlegada
                };
                Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresPreciosOutputs");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(datosPrecios);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(RequestUri, contentJson).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.PreciosLugares>>(content).FirstOrDefault();
                    if (resultado == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return resultado.plc_tarifa;
                    }
                }
            }
            else
            {
                libreriaDatos.PreciosLugares datosPrecios;
                datosPrecios = new libreriaDatos.PreciosLugares()
                {
                    cl_id = idRetiro,
                    lc_id = idLlegada,
                    plc_estado = Int32.Parse(tipoTrifa)
                };
                Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/LugaresPreciosOutputs/precios-extra");

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(datosPrecios);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(requestUri, contentJson).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.PreciosLugares>>(content).FirstOrDefault();
                    if (resultado == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return resultado.plc_tarifa;
                    }
                }
            }
            return 0;

        }

        CancellationTokenSource _cts;
        private async void txtDireccion_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            if (!string.IsNullOrWhiteSpace(e.NewTextValue) && e.NewTextValue.Length > 2)
            {
                try
                {
                    await Task.Delay(500, _cts.Token); // Espera 900ms para ver si siguen escribiendo
                    var sugerencias = await BuscarDirecciones(e.NewTextValue);
                    lvSugerencias.ItemsSource = sugerencias;
                    lvSugerencias.IsVisible = sugerencias.Any();
                }
                catch (TaskCanceledException) { /* Ignorar cancelación */ }
            }
            else
            {
                lvSugerencias.IsVisible = false;
            }
        }

        private async void lvSugerencias_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            // Obtener la descripción seleccionada
            string descripcion = e.SelectedItem.ToString();

            // Quitar selección antes de modificar texto
            lvSugerencias.SelectedItem = null;

            // Ocultar la lista antes de asignar texto para evitar que el TextChanged la vuelva a llenar
            lvSugerencias.IsVisible = false;

            // Desconectar el evento temporalmente para evitar que se dispare
            txtDireccion.TextChanged -= txtDireccion_TextChanged;
            txtDireccion.Text = descripcion;
            txtDireccion.TextChanged += txtDireccion_TextChanged;

            // Mover mapa
            var latlng = await ObtenerCoordenadasDesdeDescripcion(descripcion);
            if (latlng != null)
            {
                var posicion = new Microsoft.Maui.Devices.Sensors.Location(latlng.Value.lat, latlng.Value.lng);
                mapRutas.MoveToRegion(MapSpan.FromCenterAndRadius(posicion, Distance.FromMeters(70)));
            }

            // Quitar foco del Entry
            txtDireccion.Unfocus();
        }


        private async Task<List<string>> BuscarDirecciones(string input)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(input)}&key={GOOGLE_API_KEY}&components=country:ec";

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var resultado = JsonConvert.DeserializeObject<GooglePlacesResponse>(response);

                    return resultado.predictions.Select(p => p.description).ToList();
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        private async Task<(double lat, double lng)?> ObtenerCoordenadasDesdeDescripcion(string descripcion)
        {
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(descripcion)}&key={GOOGLE_API_KEY}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var resultado = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(response);

                    var location = resultado.results.FirstOrDefault()?.geometry?.location;
                    if (location != null)
                        return (location.lat, location.lng);
                }
            }
            catch { }

            return null;
        }

        private void txtDireccion_Focused(object sender, FocusEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDireccion.Text))
            {
                // Selecciona todo el texto al enfocar si ya hay algo escrito
                Device.BeginInvokeOnMainThread(() =>
                {
                    txtDireccion.CursorPosition = 0;
                    txtDireccion.SelectionLength = txtDireccion.Text.Length;
                });
            }
        }
        private void MainLayout_Tapped(object sender, EventArgs e)
        {
            // Oculta sugerencias si están visibles
            if (lvSugerencias.IsVisible)
                lvSugerencias.IsVisible = false;

            // Quita foco del Entry
            txtDireccion.Unfocus();
        }

        // Clases JSON para deserializar
        public class GooglePlacesResponse
        {
            public List<Prediction> predictions { get; set; }
        }

        public class Prediction
        {
            public string description { get; set; }
        }

        public class GoogleGeocodeResponse
        {
            public List<GeocodeResult> results { get; set; }
        }

        public class GeocodeResult
        {
            public Geometry geometry { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        private async Task DibujarRutaCombinadaAsync()
        {
            if (pasajeros == null || pasajeros.Count == 0)
                return;

            // 1. Origen = primer retiro
            var origen = $"{pasajeros[0].RetiroLat},{pasajeros[0].RetiroLon}";

            // 2. Destino = último destino
            var destino = $"{pasajeros[pasajeros.Count - 1].LlegadaLat},{pasajeros[pasajeros.Count - 1].LlegadaLon}";

            // 3. Waypoints intermedios (todos excepto el primero y último)
            var waypointsList = new List<string>();
            for (int i = 0; i < pasajeros.Count; i++)
            {
                waypointsList.Add($"{pasajeros[i].RetiroLat},{pasajeros[i].RetiroLon}");
                waypointsList.Add($"{pasajeros[i].LlegadaLat},{pasajeros[i].LlegadaLon}");
            }

            waypointsList.RemoveAt(0); // quitar el primer origen
            waypointsList.RemoveAt(waypointsList.Count - 1); // quitar el último destino

            string waypoints = string.Join("|", waypointsList);

            string apiKey = GOOGLE_API_KEY;
            string url =
                $"https://maps.googleapis.com/maps/api/directions/json?origin={origen}&destination={destino}&waypoints=optimize:true|{waypoints}&key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var directions = JsonConvert.DeserializeObject<GoogleDirectionsResponse>(response);

                if (directions.routes.Length > 0)
                {
                    // Limpiar cualquier ruta previa
                    mapRutas.MapElements.Clear();
                    mapRutas.Pins.Clear();

                    // --- Agregar pines por cada pasajero ---
                    foreach (var pasajero in pasajeros)
                    {
                        // Pin de Retiro
                        var pinRetiro = new Pin
                        {
                            Label = $"{pasajero.Nombre} - Origen",
                            Type = PinType.Place,
                            Location = new Microsoft.Maui.Devices.Sensors.Location(
                                double.Parse(pasajero.RetiroLat),
                                double.Parse(pasajero.RetiroLon))
                        };
                        mapRutas.Pins.Add(pinRetiro);

                        // Pin de Llegada
                        var pinLlegada = new Pin
                        {
                            Label = $"{pasajero.Nombre} - Destino",
                            Type = PinType.Place,
                            Location = new Microsoft.Maui.Devices.Sensors.Location(
                                double.Parse(pasajero.LlegadaLat),
                                double.Parse(pasajero.LlegadaLon))
                        };
                        mapRutas.Pins.Add(pinLlegada);
                    }

                    // --- Dibujar polyline ---
                    var polyline = new Polyline
                    {
                        StrokeColor = Colors.Blue,
                        StrokeWidth = 5
                    };

                    var puntos = DecodePolyline(directions.routes[0].overview_polyline.points);
                    foreach (var p in puntos)
                        polyline.Geopath.Add(new Microsoft.Maui.Devices.Sensors.Location(p.lat, p.lng));

                    mapRutas.MapElements.Add(polyline);

                    // --- Calcular tiempo total ---
                    int totalSegundos = directions.routes[0].legs.Sum(l => l.duration.value);
                    TimeSpan tiempoTotal = TimeSpan.FromSeconds(totalSegundos);

                    tiempoEstimado = $"{tiempoTotal.Hours}h{tiempoTotal.Minutes}";

                    //await DisplayAlert("Tiempo estimado",
                    //    $"Tiempo total aproximado: {tiempoTotal.Hours}h {tiempoTotal.Minutes}m",
                    //    "Aceptar");
                }
                else
                {
                    //await DisplayAlert("Error", "No se pudo calcular la ruta.", "OK");
                    tiempoEstimado = "0";
                }
            }
        }
        private void LimpiarRuta()
        {
            var polylines = mapRutas.MapElements.OfType<Polyline>().ToList();

            if (polylines.Count == 0)
                return; // 👉 No hay rutas dibujadas, salir sin hacer nada

            foreach (var line in polylines)
            {
                mapRutas.MapElements.Remove(line);
            }
        }


        // Clase mínima para deserializar Directions API
        public class GoogleDirectionsResponse
        {
            public Route[] routes { get; set; }
        }

        public class Route
        {
            public Leg[] legs { get; set; }
            public OverviewPolyline overview_polyline { get; set; }
        }

        public class Leg
        {
            public Duration duration { get; set; }
        }

        public class Duration
        {
            public int value { get; set; } // segundos
        }

        public class OverviewPolyline
        {
            public string points { get; set; }
        }

        public List<Location> DecodePolyline(string encoded)
        {
            var poly = new List<Location>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                poly.Add(new Location
                {
                    lat = (double)(lat / 1E5),
                    lng = (double)(lng / 1E5)
                });
            }

            return poly;
        }


    }
}
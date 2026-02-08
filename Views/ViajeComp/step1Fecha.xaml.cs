using Acr.UserDialogs;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Newtonsoft.Json;
using PeterTours.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PeterTours.libreriaDatos.DescuentosViajesRealizadosOutput;


namespace PeterTours.Views.ViajeComp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step1Fecha : ContentPage
    {
        public List<libreriaDatos.QhorarioData> resultado;
        public static string auxFecha = "";
        public static string fecha = "";
        public static string hora = "";
        public static string fechaLet = "";
        public static string ocupantes = "";
        public DateTime fechaRepite;
        public static int idSentidoViaje = 0;
        //public static string tipoViaje = "";
        public bool fechaCorrect = false;
        string tipoServicio = Preferences.Get("tipo", "0").ToString();
        public step1Fecha()
        {

            CultureInfo Culture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentCulture = Culture;
            InitializeComponent();

            //string tipoServicio = "3";
            if (tipoServicio == "3")
            {
                lblFechave.Text = "Seleccione la fecha de la encomienda";
                pnlValorEnc.IsVisible = false;
            }
            else
            {
                lblFechave.Text = "Seleccione la fecha de su viaje";
            }

            if (tipoServicio == "2")
            {
                piHoras.IsVisible = false;
                //pnlParrilla.IsVisible = true;
                hora = "00h00";
            }
            else
            {
                timePicker.IsVisible = false;
            }
            lblRuta.Text = step0Sentido.tipoViaje;

            //ControlTemplate diasTemplate = new ControlTemplate();

            //diasTemplate = cvFecha.SelectedDate.Value.DayNamesTemplate;

            //DataTemplate diaTemplate = cvFecha.SelectedDate.Value.DayNameTemplate;

            var navigationPage = Application.Current.MainPage as NavigationPage;
            navigationPage.BarBackgroundColor = Color.FromHex("#fc940c");


            //cvFecha.SelectedDate.Value.SpecialDates = new List<SpecialDate>
            //{
            //    new SpecialDate(DateTime.Now)
            //    {
            //        Selectable = true,
            //        TextColor = Color.CornflowerBlue,

            //    },
            //};


            //cvFecha.SelectedDate.Value.DayNameTemplate.SetValue("Lunes",DayOfWeek.Monday);
            // NavigationPage.SetHasNavigationBar(this, false);
            //dpFecha.Date = DateTime.Now;
            cvFecha.Culture = new CultureInfo("es-ES");
            this.BindingContext = this;

            // Opcional: Establecer una fecha inicial
            //SelectedDate = DateTime.Today;
        }
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                try
                {
                    if (_selectedDate != value)
                    {
                        _selectedDate = value;
                        // Notifica a la UI que el valor ha cambiado (necesario para el binding)
                        OnPropertyChanged();

                        // *** Aquí puedes llamar a una función para procesar la fecha ***
                        if (_selectedDate.HasValue)
                        {
                            this.Calendar_DateClicked(null);
                            //Console.WriteLine($"Nueva Fecha Seleccionada (Get Date): {_selectedDate.Value:yyyy-MM-dd}");
                            // Puedes ejecutar una lógica, guardar la fecha, etc.
                        }
                        else
                        {
                            pnlHora.IsVisible = false;
                            btnNext.IsVisible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string res = ex.Message;
                }

            }
        }

        public class SpecialModel
        {
            public string Description { set; get; }    //Mass
            public DateTime Date { set; get; }    //  2019-09-30
        }


        protected override async void OnAppearing()
        {
            //var request = new HttpRequestMessage();
            //request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/UsuariosOutputs");
            //request.Method = HttpMethod.Get;
            //request.Headers.Add("Accept", "application/json");
            //var client = new HttpClient();
            //HttpResponseMessage response = await client.SendAsync(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    string content = await response.Content.ReadAsStringAsync();
            //    var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.UsuariosData>>(content);
            //    lvHorario.ItemsSource = resultado;
            //}

            //List<string> datos = new List<string> { "05h00", "09h00", "13h00", "17h00", "20h00" };
            //lvHorario.ItemsSource = datos;
            //var template = new DataTemplate(typeof(TextCell));
            //template.SetValue(TextCell.TextColorProperty, Color.Black);
            //template.SetBinding(TextCell.TextColorProperty, ".");
            //lvHorario.ItemTemplate = template;
        }

        private void timePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Time")
            {
                var horaSeleccionada = timePicker.Time; // esto es un TimeSpan
                hora = $"{horaSeleccionada.Hours:D2}h{horaSeleccionada.Minutes:D2}";

                //DisplayAlert("Hora seleccionada", horaFormateada, "OK");
            }
        }
        private async Task ComprobarHorarioAsync(
    DateTime fechaSeleccionada,
    List<string> horasIniciales)
        {
            // ===== VALIDACIONES CLAVE =====
            if (horasIniciales == null || horasIniciales.Count == 0)
                return;

            if (string.IsNullOrEmpty(step0Sentido.tipoViaje))
                return;

            string auxSentido = step0Sentido.tipoViaje;
            string sentidoVaje = auxSentido.StartsWith("Q") || auxSentido.StartsWith("Aer")
                ? "Quito-Ibarra"
                : "Ibarra-Quito";

            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(
                "http://quantumdsec-001-site1.gtempurl.com/api/QhorariosOutputs");

            if (response.StatusCode != HttpStatusCode.OK)
                return;

            string content = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.QhorarioData>>(content);

            if (resultado == null || resultado.Count == 0)
                return;

            HashSet<string> horasFinales = new HashSet<string>(horasIniciales);

            foreach (var item in resultado)
            {
                if ((item.qh_estado != 1 && item.qh_estado != 2) ||
                    item.qh_sentido != sentidoVaje ||
                    item.qh_fecha.Date != fechaSeleccionada.Date ||
                    !TryNormalizarHora(item.qh_hora, auxSentido, out string horaNormalizada))
                    continue;

                if (item.qh_estado == 1)
                    horasFinales.Remove(horaNormalizada);
                else
                    horasFinales.Add(horaNormalizada);
            }

            var resultadoFinal = fechaSeleccionada.Date == DateTime.Today
                ? horasFinales
                    .Where(h => int.Parse(h.Split('h')[0]) > DateTime.Now.Hour + 2)
                    .OrderBy(h => h)
                    .ToList()
                : horasFinales.OrderBy(h => h).ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                piHoras.Items.Clear();
                foreach (var h in resultadoFinal)
                    piHoras.Items.Add(h);
            });
        }

        private bool TryNormalizarHora(
    string hora,
    string auxSentido,
    out string horaNormalizada)
        {
            horaNormalizada = null;

            try
            {
                int h, m;

                if (hora.Contains("h"))
                {
                    var p = hora.Split('h');
                    h = int.Parse(p[0]);
                    m = int.Parse(p[1]);
                }
                else if (hora.Contains(":"))
                {
                    var p = hora.Split(':');
                    h = int.Parse(p[0]);
                    m = int.Parse(p[1]);
                }
                else
                {
                    return false;
                }

                DateTime dt = new DateTime(2000, 1, 1, h, m, 0);

                if (auxSentido.StartsWith("OTA") || auxSentido.StartsWith("COT"))
                    dt = dt.AddMinutes(30);
                else if (auxSentido.StartsWith("ATUN"))
                    dt = dt.AddMinutes(15);

                horaNormalizada = dt.ToString("HH'h'mm");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task comprobarHorario()
        {

            LoadingService.Show("Cargando");
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/QhorariosOutputs");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                resultado = JsonConvert.DeserializeObject<List<libreriaDatos.QhorarioData>>(content);
                //lvCitas.ItemsSource = resultado;
                string sentidoVaje = step0Sentido.tipoViaje;
                string auxSentido = sentidoVaje;
                if (sentidoVaje.StartsWith("Q") || sentidoVaje.StartsWith("Aer"))
                {
                    sentidoVaje = "Quito-Ibarra";
                }
                else
                {
                    sentidoVaje = "Ibarra-Quito";
                }

                foreach (var item in resultado)
                {
                    DateTime fecha = item.qh_fecha;
                    string hora = item.qh_hora;
                    //if (auxSentido.ToUpper().StartsWith("OTA") || auxSentido.ToUpper().StartsWith("COT"))
                    //{

                    //    if (hora.Split('h')[1] == "30")
                    //    {
                    //        hora = (Int32.Parse(hora.Split('h')[0]) + 1) + "h00";
                    //    }
                    //    else
                    //    {
                    //        hora = hora.Split('h')[0] + "h30";
                    //    }
                    //}
                    //else if (auxSentido.ToUpper().StartsWith("ATUN"))
                    //{

                    //    if (hora.Split('h')[1] == "30")
                    //    {
                    //        hora = (Int32.Parse(hora.Split('h')[0])) + "h45";
                    //    }
                    //    else
                    //    {
                    //        hora = hora.Split('h')[0] + "h15";
                    //    }
                    //}
                    // hora viene como "21h00", "07h45", etc.
                    try
                    {
                        int h, m;

                        if (hora.Contains("h"))
                        {
                            var partes = hora.Split('h');
                            h = int.Parse(partes[0]);
                            m = int.Parse(partes[1]);
                        }
                        else if (hora.Contains(":"))
                        {
                            var partes = hora.Split(':');
                            h = int.Parse(partes[0]);
                            m = int.Parse(partes[1]);
                        }
                        else
                        {
                            // formato desconocido → no procesar
                            continue;
                        }

                        DateTime fechaHora = new DateTime(2000, 1, 1, h, m, 0);

                        if (auxSentido.ToUpper().StartsWith("OTA") || auxSentido.ToUpper().StartsWith("COT"))
                        {
                            fechaHora = fechaHora.AddMinutes(30);
                        }
                        else if (auxSentido.ToUpper().StartsWith("ATUN"))
                        {
                            fechaHora = fechaHora.AddMinutes(15);
                        }

                        // siempre devolver como 21h30
                        hora = fechaHora.ToString("HH'h'mm");
                    }
                    catch (Exception ex)
                    {
                        // Opcional: loguear el error
                        System.Diagnostics.Debug.WriteLine("Error procesando hora: " + hora + " -> " + ex.Message);

                        // Saltar este registro y seguir con el siguiente
                        continue;
                    }


                    //else if (auxSentido.ToUpper().StartsWith("CAY"))
                    //{
                    //    hora = (Int32.Parse(hora.Split('h')[0]) + 1) + "h" + hora.Split('h')[1];
                    //}
                    if ((cvFecha.SelectedDate.Value.Day == fecha.Day && cvFecha.SelectedDate.Value.Month == fecha.Month && cvFecha.SelectedDate.Value.Year == fecha.Year) && item.qh_estado == 1 && item.qh_sentido == sentidoVaje)
                    {
                        piHoras.Items.Remove(hora);
                    }
                    else if ((cvFecha.SelectedDate.Value.Day == fecha.Day && cvFecha.SelectedDate.Value.Month == fecha.Month && cvFecha.SelectedDate.Value.Year == fecha.Year) && item.qh_estado == 2 && item.qh_sentido == sentidoVaje)
                    {
                        piHoras.Items.Add(hora);
                    }
                }

                List<string> datos = new List<string> { };
                foreach (var item in piHoras.Items)
                {
                    datos.Add(item);
                }
                datos.Sort();
                piHoras.Items.Clear();

                int primeraHora = 0;
                int horaMax = (DateTime.Now.Hour + 2);
                if (cvFecha.SelectedDate.Value.Day == DateTime.Now.Day && cvFecha.SelectedDate.Value.Month == DateTime.Now.Month && cvFecha.SelectedDate.Value.Year == DateTime.Now.Year)
                {
                    for (int j = 0; j < datos.Count; j++)
                    {
                        primeraHora = Int32.Parse(datos[j].Split('h')[0]);
                        if (primeraHora > horaMax)
                        {
                            piHoras.Items.Add(datos[j]);
                        }
                    }
                }
                else
                {
                    foreach (var item in datos)
                    {
                        piHoras.Items.Add(item);
                    }
                }

                //comprobarAsientos();

            }
            LoadingService.Hide();



        }




        private bool _calendarProcessing;
        private async void Calendar_DateClicked(object sender)
        {
            if (_calendarProcessing)
                return;

            _calendarProcessing = true;

            try
            {
                LoadingService.Show("Cargando");

                if (!cvFecha.SelectedDate.HasValue)
                    return;

                DateTime fechaSeleccionada = cvFecha.SelectedDate.Value.Date;
                fechaRepite = fechaSeleccionada;

                string tipoServicio = Preferences.Get("tipo", "0");

                // tipo 3 se trata como tipo 1
                if (tipoServicio == "3")
                    tipoServicio = "1";

                // ================================
                // SENTIDO NORMALIZADO
                // ================================
                string sentidoRaw = step0Sentido.tipoViaje;

                string sentido =
                    sentidoRaw.StartsWith("Q", StringComparison.OrdinalIgnoreCase) ||
                    sentidoRaw.StartsWith("Aer", StringComparison.OrdinalIgnoreCase)
                        ? "Quito-Ibarra"
                        : "Ibarra-Quito";

                // ================================
                // ESPECIAL (solo si tipo = 1)
                // ================================
                string tipoFinal = tipoServicio;

                if (tipoServicio == "1")
                {
                    string ciudad = sentidoRaw.Split('-')[0].ToUpper();

                    if (ciudad == "OTAVALO" ||
                        ciudad == "COTACACHI" ||
                        ciudad == "CAYAMBE" ||
                        ciudad == "ATUNTAQUI")
                    {
                        tipoFinal = ciudad;
                    }
                }

                var payload = new
                {
                    fecha = fechaSeleccionada,
                    tipo = tipoFinal,
                    sentido = sentido
                };

                using HttpClient client = new HttpClient();
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "http://quantumdsec-001-site1.gtempurl.com/api/QhorariosOutputs/horarios-disponibles",
                    content
                );

                if (!response.IsSuccessStatusCode)
                    return;

                var data = await response.Content.ReadAsStringAsync();
                var horarios = JsonConvert.DeserializeObject<List<HorariosNewOutput>>(data);

                if (horarios == null || horarios.Count == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        piHoras.Items.Clear();
                        pnlHora.IsVisible = false;
                        btnNext.IsVisible = false;
                    });
                    return;
                }

                // ===== UI =====
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    piHoras.Items.Clear();
                    piOcupantes.Items.Clear();

                    foreach (var h in horarios)
                        piHoras.Items.Add(h.ho_hora);

                    foreach (var o in new[] { "1", "2", "3", "4" })
                        piOcupantes.Items.Add(o);

                    pnlHora.IsVisible = true;
                    btnNext.IsVisible = true;

                    piOcupantes.SelectedIndex =
                        Preferences.Get("tipo", "0") == "3" ? 0 : -1;

                    pnlViajes.IsVisible = Preferences.Get("tipo", "0") != "3";
                });

                string date = $"{fechaSeleccionada.Day}/{fechaSeleccionada.Month}/{fechaSeleccionada.Year}";
                fechaLet = fechaLetras(date, fechaSeleccionada.DayOfWeek.ToString());
                fecha = date;
                fechaCorrect = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    pnlHora.IsVisible = false;
                    btnNext.IsVisible = false;
                });
            }
            finally
            {
                _calendarProcessing = false;
                LoadingService.Hide();
            }
        }

        //private async void Calendar_DateClicked(object sender)
        //{
        //    if (_calendarProcessing)
        //        return;

        //    _calendarProcessing = true;

        //    try
        //    {
        //        LoadingService.Show("Cargando");

        //        if (!cvFecha.SelectedDate.HasValue)
        //            return;

        //        DateTime fechaSeleccionada = cvFecha.SelectedDate.Value.Date;
        //        fechaRepite = fechaSeleccionada;

        //        string tipoTrifa = Preferences.Get("tipo", "0");
        //        if (tipoTrifa == "3")
        //            tipoTrifa = "1";

        //        string sentidoVaje = step0Sentido.tipoViaje;
        //        string ciudad = sentidoVaje.Split('-')[0].ToUpper();

        //        libreriaDatos.HorariosDataInsert datosHorarios;

        //        if (ciudad == "OTAVALO" || ciudad == "COTACACHI" ||
        //            ciudad == "CAYAMBE" || ciudad == "ATUNTAQUI")
        //        {
        //            datosHorarios = new libreriaDatos.HorariosDataInsert
        //            {
        //                ho_especial = ciudad
        //            };
        //        }
        //        else
        //        {
        //            datosHorarios = new libreriaDatos.HorariosDataInsert
        //            {
        //                ho_especial = tipoTrifa
        //            };
        //        }

        //        Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/HorariosOutputs");

        //        using HttpClient client = new HttpClient();
        //        var json = JsonConvert.SerializeObject(datosHorarios);
        //        var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync(requestUri, contentJson);
        //        if (response.StatusCode != HttpStatusCode.OK)
        //            return;

        //        var content = await response.Content.ReadAsStringAsync();
        //        var datos = JsonConvert.DeserializeObject<List<libreriaDatos.HorariosData>>(content);

        //        if (datos == null || datos.Count == 0)
        //            return;

        //        // ================= UI =================
        //        MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            piHoras.Items.Clear();
        //            piOcupantes.Items.Clear();

        //            foreach (var o in new[] { "1", "2", "3", "4" })
        //                piOcupantes.Items.Add(o);

        //            foreach (var item in datos)
        //                piHoras.Items.Add(item.ho_hora);

        //            pnlHora.IsVisible = true;
        //            btnNext.IsVisible = true;

        //            string tipoServicio = Preferences.Get("tipo", "0");
        //            piOcupantes.SelectedIndex = tipoServicio == "3" ? 0 : -1;
        //            pnlViajes.IsVisible = tipoServicio != "3";
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        _calendarProcessing = false;
        //        LoadingService.Hide();
        //    }
        //}

        //private async void Calendar_DateClicked(object sender)
        //{
        //    if (_calendarProcessing)
        //        return;

        //    _calendarProcessing = true;
        //    try
        //    {
        //        LoadingService.Show("Cargando");

        //        try
        //        {
        //            // ===== VALIDACIÓN SEGURA DE FECHA =====
        //            if (!cvFecha.SelectedDate.HasValue)
        //                return;

        //            DateTime fechaSeleccionada = cvFecha.SelectedDate.Value.Date;

        //            // ===== EVITA DISPARO DUPLICADO =====
        //            //if (fechaSeleccionada == fechaRepite.Date)
        //            //{
        //            //    MainThread.BeginInvokeOnMainThread(() =>
        //            //    {
        //            //        pnlHora.IsVisible = false;
        //            //        btnNext.IsVisible = false;
        //            //    });

        //            //    fechaRepite = new DateTime();
        //            //    auxFecha = "";
        //            //    return;
        //            //}

        //            fechaRepite = fechaSeleccionada;

        //            string tipoTrifa = Preferences.Get("tipo", "0").ToString();
        //            if (tipoTrifa == "3")
        //                tipoTrifa = "1";

        //            string sentidoVaje = step0Sentido.tipoViaje;
        //            libreriaDatos.HorariosDataInsert datosHorarios;

        //            string ciudad = sentidoVaje.Split('-')[0].ToUpper();

        //            if (ciudad == "OTAVALO" || ciudad == "COTACACHI" ||
        //                ciudad == "CAYAMBE" || ciudad == "ATUNTAQUI")
        //            {
        //                datosHorarios = new libreriaDatos.HorariosDataInsert
        //                {
        //                    ho_especial = ciudad
        //                };
        //            }
        //            else
        //            {
        //                datosHorarios = new libreriaDatos.HorariosDataInsert
        //                {
        //                    ho_especial = tipoTrifa
        //                };
        //            }

        //            // ===== REQUEST =====
        //            Uri requestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/HorariosOutputs");

        //            using HttpClient client = new HttpClient();
        //            var json = JsonConvert.SerializeObject(datosHorarios);
        //            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

        //            var response = await client.PostAsync(requestUri, contentJson);

        //            if (response.StatusCode != HttpStatusCode.OK)
        //                return;

        //            string content = await response.Content.ReadAsStringAsync();
        //            var datos = JsonConvert.DeserializeObject<List<libreriaDatos.HorariosData>>(content);

        //            if (datos == null || datos.Count == 0)
        //                return;

        //            // ===== LIMPIEZA + OCUPANTES (UI THREAD) =====
        //            MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                if (piOcupantes?.Items == null || piHoras?.Items == null)
        //                {

        //                    LoadingService.Hide();
        //                    return;
        //                }

        //                piOcupantes.Items.Clear();
        //                piHoras.Items.Clear();

        //                var ocup = new[] { "1", "2", "3", "4" };
        //                foreach (var item in ocup)
        //                    piOcupantes.Items.Add(item);
        //            });

        //            int horaMax = DateTime.Now.Hour + 2;

        //            string date = $"{fechaSeleccionada.Day}/{fechaSeleccionada.Month}/{fechaSeleccionada.Year}";


        //            fechaLet = fechaLetras(date, fechaSeleccionada.DayOfWeek.ToString());
        //            fecha = date;

        //            if (fechaSeleccionada >= DateTime.Today)
        //            {
        //                fechaCorrect = true;

        //                // ===== HORAS =====
        //                MainThread.BeginInvokeOnMainThread(() =>
        //                {
        //                    if (piHoras?.Items == null)
        //                        return;

        //                    if (fechaSeleccionada == DateTime.Today)
        //                    {
        //                        for (int j = 1; j < datos.Count; j++)
        //                        {
        //                            int hora = int.Parse(datos[j].ho_hora.Split('h')[0]);
        //                            if (hora > horaMax)
        //                                piHoras.Items.Add(datos[j].ho_hora);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        foreach (var item in datos)
        //                            piHoras.Items.Add(item.ho_hora);
        //                    }
        //                });

        //                if (tipoTrifa == "1" || tipoTrifa == "3")
        //                {
        //                    //await comprobarHorario();
        //                    var horasSnapshot = piHoras.Items?.Cast<string>().ToList();
        //                    if (horasSnapshot != null)
        //                        await ComprobarHorarioAsync(fechaSeleccionada, horasSnapshot);
        //                }


        //                MainThread.BeginInvokeOnMainThread(() =>
        //                {
        //                    pnlHora.IsVisible = true;
        //                    btnNext.IsVisible = true;

        //                    string tipoServicio = Preferences.Get("tipo", "0").ToString();

        //                    if (piOcupantes.Items.Count > 0)
        //                        piOcupantes.SelectedIndex = tipoServicio == "3" ? 0 : -1;

        //                    pnlViajes.IsVisible = tipoServicio != "3";
        //                });
        //            }
        //            else
        //            {
        //                await DisplayAlert(
        //                    "Fecha Incorrecta",
        //                    "Por favor seleccione una fecha actual o una fecha futura",
        //                    "Aceptar");

        //                fechaCorrect = false;

        //                MainThread.BeginInvokeOnMainThread(() =>
        //                {
        //                    pnlHora.IsVisible = false;
        //                    btnNext.IsVisible = false;
        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.ToString());

        //            MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                pnlHora.IsVisible = false;
        //                btnNext.IsVisible = false;

        //                LoadingService.Hide();
        //            });

        //            fechaRepite = new DateTime();
        //            auxFecha = "";
        //        }
        //        finally
        //        {
        //            _calendarProcessing = false;
        //            LoadingService.Hide();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _calendarProcessing = false;
        //        LoadingService.Hide();
        //    }

        //}
        public async void comprobarAsientos()
        {
            try
            {

                string sentidoVaje = step0Sentido.tipoViaje;
                string auxSentido = sentidoVaje;
                if (sentidoVaje.StartsWith("Q") || sentidoVaje.StartsWith("Aer"))
                {
                    //sentidoVaje = "Quito-Ibarra";
                    idSentidoViaje = 2;
                }
                else
                {
                    //sentidoVaje = "Ibarra-Quito";
                    idSentidoViaje = 1;
                }
                DateTime horaDt = DateTime.ParseExact(hora, "HH'h'mm", CultureInfo.InvariantCulture);

                // 2️⃣ Aplicar la lógica RESTANDO minutos
                if (auxSentido.ToUpper().StartsWith("OTA") || auxSentido.ToUpper().StartsWith("COT"))
                {
                    horaDt = horaDt.AddMinutes(-30);
                }
                else if (auxSentido.ToUpper().StartsWith("ATUN"))
                {
                    horaDt = horaDt.AddMinutes(-15);
                }

                // 3️⃣ Volver a string con el mismo formato
                string auxHora = horaDt.ToString("HH'h'mm");

                var request = new DisponibilidadAsientosInput
                {
                    Fecha = cvFecha.SelectedDate.Value.Date,
                    Hora = auxHora,
                    Sentido = idSentidoViaje
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Se recomienda usar un timeout para evitar que la app se quede colgada
                    client.Timeout = TimeSpan.FromSeconds(30);


                    var response = await client.PostAsync("http://quantumdsec-001-site1.gtempurl.com/api/CitasOutputs/obtener-disponibilidad-asientos", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string contenido = await response.Content.ReadAsStringAsync();
                        var resultado = JsonConvert.DeserializeObject<List<DisponibilidadAsientosOutput>>(contenido).FirstOrDefault().Asientos_Disponibles;

                        piOcupantes.Items.Clear();

                        for (int i = 1; i <= resultado; i++)
                        {
                            piOcupantes.Items.Add(i.ToString());
                        }

                        // Limpiar los datos de factura de las preferencias después del éxito (opcional)
                        // LimpiarFacturaPreferences();


                    }
                    else
                    {
                        //var errorContent = await response.Content.ReadAsStringAsync();
                        //await DisplayAlert("Error", "No se pudo registrar la cita. Servidor respondió: " + response.StatusCode, "Cerrar");
                        piOcupantes.Items.Clear();

                        List<string> ocup = new List<string> { "1", "2", "3", "4" };
                        foreach (var item in ocup)
                        {
                            piOcupantes.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                piOcupantes.Items.Clear();
                piHoras.Items.Clear();

                List<string> ocup = new List<string> { "1", "2", "3", "4" };
                foreach (var item in ocup)
                {
                    piOcupantes.Items.Add(item);
                }
            }

        }
        //private void chkParrilla_CheckedChanged(object sender, CheckedChangedEventArgs e)
        //{
        //    necesitaParrilla = e.Value ? 1 : 0;
        //    // Aquí ya tienes 0 o 1
        //    Console.WriteLine("Parrilla: " + necesitaParrilla);
        //}
        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");

            if (tipoServicio == "2")
            {
                if (!String.IsNullOrEmpty(hora) && piOcupantes.SelectedIndex != -1)
                {
                    LabelError.Text = "";
                    //await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajero());
                    await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmarParrilla());
                }
                else
                {
                    LabelError.Text = "Favor rellene todos los campos";
                }
            }
            else if (tipoServicio == "3")
            {
                if (!String.IsNullOrEmpty(hora))
                {
                    LabelError.Text = "";
                    //await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajero());
                    await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajeroOtro());
                }
                else
                {
                    LabelError.Text = "Favor rellene todos los campos";
                }
            }
            else
            {
                if (piHoras.SelectedIndex != -1 && piOcupantes.SelectedIndex != -1)
                {
                    LabelError.Text = "";
                    //await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajero());
                    await NavigationHelper.SafePushAsync(Navigation, new step2ConfirmaPasajeroOtro());
                }
                else
                {
                    LabelError.Text = "Favor rellene todos los campos";
                }
            }
            LoadingService.Hide();
        }

        private void piHoras_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                hora = piHoras.SelectedItem.ToString();

                string tipoTrifa = Preferences.Get("tipo", "0").ToString();
                if (tipoTrifa == "1")
                {
                    comprobarAsientos();
                }

            }
            catch (Exception ex)
            {

            }
        }

        public string fechaLetras(string fec, string diaLetras)
        {
            string fecResult;
            string Mes = "";
            int mes = Int32.Parse(fec.Split('/')[1]);
            string dia = fec.Split('/')[0];
            string anio = fec.Split('/')[2];
            string diaLet = "";
            switch (mes)
            {
                case 1:
                    Mes = "Enero";
                    break;
                case 2:
                    Mes = "Febrero";
                    break;
                case 3:
                    Mes = "Marzo";
                    break;
                case 4:
                    Mes = "Abril";
                    break;
                case 5:
                    Mes = "Mayo";
                    break;
                case 6:
                    Mes = "Junio";
                    break;
                case 7:
                    Mes = "Julio";
                    break;
                case 8:
                    Mes = "Agosto";
                    break;
                case 9:
                    Mes = "Septiembre";
                    break;
                case 10:
                    Mes = "Octubre";
                    break;
                case 11:
                    Mes = "Noviembre";
                    break;
                case 12:
                    Mes = "Diciembre";
                    break;
                default:
                    Mes = "";
                    break;
            }
            if (diaLetras.ToUpper() == "MONDAY")
            {
                diaLet = "Lunes";
            }
            else if (diaLetras.ToUpper() == "TUESDAY")
            {
                diaLet = "Martes";
            }
            else if (diaLetras.ToUpper() == "WEDNESDAY")
            {
                diaLet = "Miércoles";
            }
            else if (diaLetras.ToUpper() == "THURSDAY")
            {
                diaLet = "Jueves";
            }
            else if (diaLetras.ToUpper() == "FRIDAY")
            {
                diaLet = "Viernes";
            }
            else if (diaLetras.ToUpper() == "SATURDAY")
            {
                diaLet = "Sábado";
            }
            else if (diaLetras.ToUpper() == "SUNDAY")
            {
                diaLet = "Domingo";
            }
            fecResult = diaLet + ", " + dia + " de " + Mes + " del " + anio;
            return fecResult;
        }

        private void piOcupantes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                ocupantes = piOcupantes.SelectedItem.ToString();
            }
            catch (Exception ex)
            {

            }

        }

        //private void piViaje_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        tipoViaje = piViaje.SelectedItem.ToString();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //https://maps.google.com/?q=<lat>,<lng>
        private async void btnComWpp_Clicked(object sender, EventArgs e)
        {
            //LoadingService.Show("Cargando");
            //try
            //{
            //    Chat.Open("+593995951038", "Me gustaría agendar una cita con un nuevo horario");
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Error", ex.Message, "Cerrar");
            //}
            //LoadingService.Hide();

            LoadingService.Show("Cargando");

            try
            {
                string numero = "+593995951038";
                string mensaje = "Me gustaría agendar una cita con un nuevo horario";

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
        //private void cvFecha_DateSelectionChanged(object sender, XCalendar.DateSelectionChangedEventArgs e)
        //{
        //    piOcupantes.Items.Clear();
        //    piHoras.Items.Clear();
        //    //piViaje.Items.Clear();
        //    //List<string> tipoViaje = new List<string> { "Ibarra-Quito", "Quito-Ibarra" };
        //    //foreach (var item in tipoViaje)
        //    //{
        //    //    piViaje.Items.Add(item);
        //    //}

        //    List<string> ocup = new List<string> { "1", "2", "3", "4" };
        //    foreach (var item in ocup)
        //    {
        //        piOcupantes.Items.Add(item);
        //    }
        //    //piOcupantes.SelectedIndex = 0;
        //    string date = "";
        //    string tipoTrifa = Application.Current.Properties["tipo"].ToString();
        //    List<string> datos = new List<string> { };
        //    if (tipoTrifa == "2")
        //    {
        //        datos = new List<string> { "00h00","01h00", "02h00", "03h00", "04h00", "05h00",
        //            "06h00","07h00","08h00","09h00","10h00","11h00","12h00", "13h00",
        //            "14h00","15h00","16h00","17h00","18h00","19h00", "20h00","21h00","22h00","23h00", };
        //    }
        //    else
        //    {
        //        datos = new List<string> { "05h00", "09h00", "13h00", "17h00", "20h00" };
        //    }

        //    int i = 0;
        //    int primeraHora = 0;
        //    int horaMax = (DateTime.Now.Hour + 2);

        //    string dat = cvFecha.SelectedDate.Value.NavigatedDates[0].ToString().Split(' ')[0];

        //    date = cvFecha.SelectedDate.Value.NavigatedDates[0].Day + "/" + cvFecha.SelectedDate.Value.NavigatedDates[0].Month + "/" + cvFecha.SelectedDate.Value.NavigatedDates[0].Year;
        //    fechaLet = fechaLetras(date, cvFecha.SelectedDate.Value.NavigatedDates[0].DayOfWeek.ToString());
        //    fecha = date;
        //    if (cvFecha.SelectedDate.Value.NavigatedDates[0].Month >= DateTime.Now.Month && cvFecha.SelectedDate.Value.NavigatedDates[0].Year >= DateTime.Now.Year)
        //    {
        //        if ((cvFecha.SelectedDate.Value.NavigatedDates[0].Day >= DateTime.Now.Day && cvFecha.SelectedDate.Value.NavigatedDates[0].Month == DateTime.Now.Month) || (cvFecha.SelectedDate.Value.NavigatedDates[0].Month > DateTime.Now.Month))
        //        {
        //            DisplayAlert("Fecha Seleccionada", fechaLet, "Aceptar");
        //            fechaCorrect = true;

        //            if (cvFecha.SelectedDate.Value.NavigatedDates[0].Day == DateTime.Now.Day && cvFecha.SelectedDate.Value.NavigatedDates[0].Month == DateTime.Now.Month && cvFecha.SelectedDate.Value.NavigatedDates[0].Year == DateTime.Now.Year)
        //            {
        //                for (int j = 1; j < datos.Count; j++)
        //                {
        //                    primeraHora = Int32.Parse(datos[j].Split('h')[0]);
        //                    if (primeraHora > horaMax)
        //                    {
        //                        piHoras.Items.Add(datos[j]);
        //                    }
        //                }
        //                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                {
        //                    comprobarHorario();
        //                }
        //            }
        //            else if ((((((cvFecha.SelectedDate.Value.NavigatedDates[0].Day == (DateTime.Now.Day + 1)) || (((DateTime.Now.Day + 1) >= 28) && (cvFecha.SelectedDate.Value.NavigatedDates[0].Day == 1)))) && cvFecha.SelectedDate.Value.NavigatedDates[0].Month >= DateTime.Now.Month && cvFecha.SelectedDate.Value.NavigatedDates[0].Year == DateTime.Now.Year) && DateTime.Now.Hour >= 20) && (tipoTrifa == "1" || tipoTrifa == "3"))
        //            {
        //                for (int j = 1; j < datos.Count; j++)
        //                {
        //                    piHoras.Items.Add(datos[j]);
        //                }
        //                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                {
        //                    comprobarHorario();
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in datos)
        //                {
        //                    piHoras.Items.Add(item);
        //                    i++;
        //                }
        //                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                {
        //                    comprobarHorario();
        //                }
        //            }
        //            pnlHora.IsVisible = true;
        //            btnNext.IsVisible = true;
        //            string tipoServicio = Application.Current.Properties["tipo"].ToString();
        //            if (tipoServicio == "3")
        //            {
        //                pnlViajes.IsVisible = false;
        //                piOcupantes.SelectedIndex = 0;
        //            }
        //            else
        //            {
        //                pnlViajes.IsVisible = true;
        //                piOcupantes.SelectedIndex = -1;
        //            }
        //        }
        //        else
        //        {
        //            DisplayAlert("Fecha Incorrecta", "Por favor seleccione una fecha actual o una fecha futura", "Aceptar");
        //            fechaCorrect = false;
        //            pnlHora.IsVisible = false;
        //            btnNext.IsVisible = false;
        //        }

        //    }
        //    else
        //    {
        //        DisplayAlert("Fecha Incorrecta", "Por favor seleccione una fecha actual o una fecha futura", "Aceptar");
        //        fechaCorrect = false;
        //        pnlHora.IsVisible = false;
        //        btnNext.IsVisible = false;
        //    }

        //}

        //private async void Calendar_DateClicked(object sender)
        //{
        //    LoadingService.Show("Cargando");

        //    try
        //    {
        //        // ===== VALIDACIÓN SEGURA DE FECHA =====
        //        if (!cvFecha.SelectedDate.HasValue)
        //            return;

        //        DateTime fechaSeleccionada = cvFecha.SelectedDate.Value.Date;

        //        // ===== EVITA DISPARO DUPLICADO =====
        //        if (fechaSeleccionada == fechaRepite.Date)
        //        {
        //            pnlHora.IsVisible = false;
        //            btnNext.IsVisible = false;
        //            fechaRepite = new DateTime();
        //            auxFecha = "";
        //            return;
        //        }

        //        fechaRepite = fechaSeleccionada;

        //        string tipoTrifa = Preferences.Get("tipo", "0").ToString();
        //        if (tipoTrifa == "3")
        //        {
        //            tipoTrifa = "1";
        //        }

        //        string sentidoVaje = step0Sentido.tipoViaje;
        //        libreriaDatos.HorariosDataInsert datosHorarios;

        //        // ===== MISMA LÓGICA DE CIUDADES =====
        //        if (sentidoVaje.Split('-')[0].ToUpper() == "OTAVALO" ||
        //            sentidoVaje.Split('-')[0].ToUpper() == "COTACACHI" ||
        //            sentidoVaje.Split('-')[0].ToUpper() == "CAYAMBE" ||
        //            sentidoVaje.Split('-')[0].ToUpper() == "ATUNTAQUI")
        //        {
        //            datosHorarios = new libreriaDatos.HorariosDataInsert()
        //            {
        //                ho_especial = sentidoVaje.Split('-')[0].ToUpper()
        //            };
        //        }
        //        else
        //        {
        //            datosHorarios = new libreriaDatos.HorariosDataInsert()
        //            {
        //                ho_especial = tipoTrifa
        //            };
        //        }

        //        // ===== REQUEST =====
        //        Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/HorariosOutputs");

        //        var client = new HttpClient();
        //        var json = JsonConvert.SerializeObject(datosHorarios);
        //        var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync(RequestUri, contentJson);

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            string content = await response.Content.ReadAsStringAsync();
        //            var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.HorariosData>>(content);

        //            // ===== LIMPIEZA =====
        //            piOcupantes.Items.Clear();
        //            piHoras.Items.Clear();

        //            List<string> ocup = new List<string> { "1", "2", "3", "4" };
        //            foreach (var item in ocup)
        //            {
        //                piOcupantes.Items.Add(item);
        //            }

        //            List<libreriaDatos.HorariosData> datos = resultado;

        //            int primeraHora = 0;
        //            int horaMax = DateTime.Now.Hour + 2;

        //            // ===== FORMATO FECHA (MISMO RESULTADO) =====
        //            string date = fechaSeleccionada.Day + "/" +
        //                          fechaSeleccionada.Month + "/" +
        //                          fechaSeleccionada.Year;

        //            if (date != auxFecha)
        //            {
        //                auxFecha = date;
        //                fechaLet = fechaLetras(date, fechaSeleccionada.DayOfWeek.ToString());
        //                fecha = date;

        //                // ===== VALIDACIÓN DE FECHA FUTURA / ACTUAL =====
        //                if (fechaSeleccionada >= DateTime.Today)
        //                {
        //                    //await DisplayAlert("Fecha Seleccionada", fechaLet, "Aceptar");
        //                    fechaCorrect = true;

        //                    // ===== MISMA LÓGICA DE HORAS =====
        //                    if (fechaSeleccionada == DateTime.Today)
        //                    {
        //                        for (int j = 1; j < datos.Count; j++)
        //                        {
        //                            primeraHora = Int32.Parse(datos[j].ho_hora.Split('h')[0]);
        //                            if (primeraHora > horaMax)
        //                            {
        //                                piHoras.Items.Add(datos[j].ho_hora);
        //                            }
        //                        }

        //                        if (tipoTrifa == "1" || tipoTrifa == "3")
        //                        {
        //                            comprobarHorario();
        //                        }
        //                    }
        //                    else if (
        //                        (
        //                            (
        //                                (fechaSeleccionada.Day == DateTime.Now.Day + 1) ||
        //                                ((DateTime.Now.Day + 1 >= 28) && fechaSeleccionada.Day == 1)
        //                            ) &&
        //                            fechaSeleccionada.Month >= DateTime.Now.Month &&
        //                            fechaSeleccionada.Year == DateTime.Now.Year &&
        //                            DateTime.Now.Hour >= 20
        //                        ) &&
        //                        (tipoTrifa == "1" || tipoTrifa == "3")
        //                    )
        //                    {
        //                        for (int j = 1; j < datos.Count; j++)
        //                        {
        //                            piHoras.Items.Add(datos[j].ho_hora);
        //                        }

        //                        comprobarHorario();
        //                    }
        //                    else
        //                    {
        //                        foreach (var item in datos)
        //                        {
        //                            piHoras.Items.Add(item.ho_hora);
        //                        }

        //                        if (tipoTrifa == "1" || tipoTrifa == "3")
        //                        {
        //                            comprobarHorario();
        //                        }
        //                    }

        //                    pnlHora.IsVisible = true;
        //                    btnNext.IsVisible = true;

        //                    string tipoServicio = Preferences.Get("tipo", "0").ToString();
        //                    if (tipoServicio == "3")
        //                    {
        //                        pnlViajes.IsVisible = false;
        //                        piOcupantes.SelectedIndex = 0;
        //                    }
        //                    else
        //                    {
        //                        pnlViajes.IsVisible = true;
        //                        piOcupantes.SelectedIndex = -1;
        //                    }
        //                }
        //                else
        //                {
        //                    await DisplayAlert("Fecha Incorrecta",
        //                        "Por favor seleccione una fecha actual o una fecha futura",
        //                        "Aceptar");

        //                    fechaCorrect = false;
        //                    pnlHora.IsVisible = false;
        //                    btnNext.IsVisible = false;
        //                }
        //            }
        //            else
        //            {
        //                fechaCorrect = false;
        //                pnlHora.IsVisible = false;
        //                btnNext.IsVisible = false;
        //                auxFecha = "";
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        pnlHora.IsVisible = false;
        //        btnNext.IsVisible = false;
        //        fechaRepite = new DateTime();
        //        auxFecha = "";
        //    }
        //    finally
        //    {
        //        LoadingService.Hide();
        //    }
        //}

        //private async void Calendar_DateClicked(object sender)
        //{
        //    try
        //    {
        //        if (cvFecha.SelectedDate.Value != fechaRepite)
        //        {
        //            fechaRepite = (DateTime)cvFecha.SelectedDate.Value;
        //            LoadingService.Show("Cargando");
        //            string tipoTrifa = Preferences.Get("tipo","0").ToString();
        //            if (tipoTrifa == "3")
        //            {
        //                tipoTrifa = "1";
        //            }
        //            string sentidoVaje = step0Sentido.tipoViaje;
        //            libreriaDatos.HorariosDataInsert datosHorarios;
        //            if (sentidoVaje.Split('-')[0].ToUpper() == "OTAVALO" || sentidoVaje.Split('-')[0].ToUpper() == "COTACACHI" || sentidoVaje.Split('-')[0].ToUpper() == "CAYAMBE" || sentidoVaje.Split('-')[0].ToUpper() == "ATUNTAQUI")
        //            {
        //                datosHorarios = new libreriaDatos.HorariosDataInsert()
        //                {
        //                    ho_especial = sentidoVaje.Split('-')[0].ToUpper()
        //                };
        //            }
        //            else
        //            {
        //                datosHorarios = new libreriaDatos.HorariosDataInsert()
        //                {
        //                    ho_especial = tipoTrifa
        //                };
        //            }


        //            //var request = new HttpRequestMessage();
        //            //request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/QhorariosOutputs");
        //            //request.Method = HttpMethod.Get;
        //            //request.Headers.Add("Accept", "application/json");
        //            //var client = new HttpClient();
        //            //HttpResponseMessage response = await client.SendAsync(request);
        //            Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/HorariosOutputs");

        //            var client = new HttpClient();
        //            var json = JsonConvert.SerializeObject(datosHorarios);
        //            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
        //            var response = await client.PostAsync(RequestUri, contentJson);
        //            if (response.StatusCode == HttpStatusCode.OK)
        //            {
        //                string content = await response.Content.ReadAsStringAsync();
        //                var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.HorariosData>>(content);

        //                piOcupantes.Items.Clear();
        //                piHoras.Items.Clear();
        //                //piViaje.Items.Clear();
        //                //List<string> tipoViaje = new List<string> { "Ibarra-Quito", "Quito-Ibarra" };
        //                //foreach (var item in tipoViaje)
        //                //{
        //                //    piViaje.Items.Add(item);
        //                //}

        //                List<string> ocup = new List<string> { "1", "2", "3", "4" };
        //                foreach (var item in ocup)
        //                {
        //                    piOcupantes.Items.Add(item);
        //                }
        //                //piOcupantes.SelectedIndex = 0;
        //                string date = "";

        //                List<libreriaDatos.HorariosData> datos = resultado;
        //                //if (tipoTrifa == "2")
        //                //{
        //                //    //datos = new List<libreriaDatos.UsuariosData> { "00h00","01h00", "02h00", "03h00", "04h00", "05h00",
        //                //    //"06h00","07h00","08h00","09h00","10h00","11h00","12h00", "13h00",
        //                //    //"14h00","15h00","16h00","17h00","18h00","19h00", "20h00","21h00","22h00","23h00", };

        //                //}
        //                //else
        //                //{

        //                //    if (sentidoVaje.ToUpper().StartsWith("OTA"))
        //                //    {
        //                //        datos = new List<string> { "05h00", "09h00", "13h00", "17h00", "21h00" };
        //                //    }
        //                //    else
        //                //    {
        //                //        datos = new List<string> { "05h00", "09h00", "13h00", "17h00", "20h30" };
        //                //    }

        //                //}

        //                int i = 0;
        //                int primeraHora = 0;
        //                int horaMax = (DateTime.Now.Hour + 2);

        //                //string dat = cvFecha.SelectedDate.Value.NavigatedDates[0].ToString().Split(' ')[0];

        //                date = cvFecha.SelectedDate.Value.Day + "/" + cvFecha.SelectedDate.Value.Month + "/" + cvFecha.SelectedDate.Value.Year;
        //                if (date != auxFecha)
        //                {
        //                    auxFecha = date;
        //                    fechaLet = fechaLetras(date, cvFecha.SelectedDate.Value.DayOfWeek.ToString());
        //                    fecha = date;
        //                    //if (cvFecha.SelectedDate.Value.Month >= DateTime.Now.Month && cvFecha.SelectedDate.Value.Year >= DateTime.Now.Year)
        //                    //if (cvFecha.SelectedDate.Value.HasValue && 
        //                        if(cvFecha.SelectedDate.Value.Year > DateTime.Now.Year ||
        //                        (cvFecha.SelectedDate.Value.Year == DateTime.Now.Year && cvFecha.SelectedDate.Value.Month >= DateTime.Now.Month))
        //                    {
        //                        //if ((cvFecha.SelectedDate.Value.Day >= DateTime.Now.Day && cvFecha.SelectedDate.Value.Month == DateTime.Now.Month) || (cvFecha.SelectedDate.Value.Month > DateTime.Now.Month))
        //                        //if (cvFecha.SelectedDate.Value.HasValue &&
        //                           if ((cvFecha.SelectedDate.Value.Year > DateTime.Now.Year) ||
        //                            (cvFecha.SelectedDate.Value.Year == DateTime.Now.Year && cvFecha.SelectedDate.Value.Month > DateTime.Now.Month) ||
        //                            (cvFecha.SelectedDate.Value.Year == DateTime.Now.Year && cvFecha.SelectedDate.Value.Month == DateTime.Now.Month && cvFecha.SelectedDate.Value.Day >= DateTime.Now.Day))
        //                        {
        //                            await DisplayAlert("Fecha Seleccionada", fechaLet, "Aceptar");
        //                            fechaCorrect = true;

        //                            if (cvFecha.SelectedDate.Value.Day == DateTime.Now.Day && cvFecha.SelectedDate.Value.Month == DateTime.Now.Month && cvFecha.SelectedDate.Value.Year == DateTime.Now.Year)
        //                            {
        //                                for (int j = 1; j < datos.Count; j++)
        //                                {
        //                                    primeraHora = Int32.Parse(datos[j].ho_hora.Split('h')[0]);
        //                                    if (primeraHora > horaMax)
        //                                    {
        //                                        piHoras.Items.Add(datos[j].ho_hora);
        //                                    }
        //                                }
        //                                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                                {
        //                                    comprobarHorario();
        //                                }
        //                            }
        //                            else if ((((((cvFecha.SelectedDate.Value.Day == (DateTime.Now.Day + 1)) || (((DateTime.Now.Day + 1) >= 28) && (cvFecha.SelectedDate.Value.Day == 1)))) && cvFecha.SelectedDate.Value.Month >= DateTime.Now.Month && cvFecha.SelectedDate.Value.Year == DateTime.Now.Year) && DateTime.Now.Hour >= 20) && (tipoTrifa == "1" || tipoTrifa == "3"))
        //                            {
        //                                for (int j = 1; j < datos.Count; j++)
        //                                {
        //                                    piHoras.Items.Add(datos[j].ho_hora);
        //                                }
        //                                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                                {
        //                                    comprobarHorario();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                foreach (var item in datos)
        //                                {
        //                                    piHoras.Items.Add(item.ho_hora);
        //                                    i++;
        //                                }
        //                                if ((tipoTrifa == "1" || tipoTrifa == "3"))
        //                                {
        //                                    comprobarHorario();
        //                                }
        //                            }
        //                            pnlHora.IsVisible = true;
        //                            btnNext.IsVisible = true;
        //                            string tipoServicio = Preferences.Get("tipo","0").ToString();
        //                            if (tipoServicio == "3")
        //                            {
        //                                pnlViajes.IsVisible = false;
        //                                piOcupantes.SelectedIndex = 0;
        //                            }
        //                            else
        //                            {
        //                                pnlViajes.IsVisible = true;
        //                                piOcupantes.SelectedIndex = -1;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            await DisplayAlert("Fecha Incorrecta", "Por favor seleccione una fecha actual o una fecha futura", "Aceptar");
        //                            fechaCorrect = false;
        //                            pnlHora.IsVisible = false;
        //                            btnNext.IsVisible = false;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        await DisplayAlert("Fecha Incorrecta", "Por favor seleccione una fecha actual o una fecha futura", "Aceptar");
        //                        fechaCorrect = false;
        //                        pnlHora.IsVisible = false;
        //                        btnNext.IsVisible = false;
        //                    }
        //                }
        //                else
        //                {
        //                    fechaCorrect = false;
        //                    pnlHora.IsVisible = false;
        //                    btnNext.IsVisible = false;
        //                    auxFecha = "";
        //                }
        //            }

        //            LoadingService.Hide();
        //        }
        //        else
        //        {
        //            pnlHora.IsVisible = false;
        //            btnNext.IsVisible = false;
        //            fechaRepite = new DateTime();
        //            auxFecha = "";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        pnlHora.IsVisible = false;
        //        btnNext.IsVisible = false;
        //        fechaRepite = new DateTime();
        //        auxFecha = "";
        //        LoadingService.Hide();
        //    }

        //}


    }
}
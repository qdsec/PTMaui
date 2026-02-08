using Acr.UserDialogs;
using Newtonsoft.Json;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step2identificacion : ContentPage
    {
        public static string identi = "";
        public step2identificacion()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void Next_Click(object sender, EventArgs e)
        {
            LoadingService.Show("Cargando");
            bool existe = false;
            string cedula = txtCedula.Text;
            var request = new HttpRequestMessage();
            var client = new HttpClient();
            request.RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/UsuariosOutputs");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<libreriaDatos.UsuariosData>>(content);

                foreach (var item in resultado)
                {
                    if (item.usu_ci==txtCedula.Text)
                    {
                        existe = true;
                        break;
                    }
                }
                if (existe==false)
                {
                    if (cedula.Length==10)
                    {
                        if (!String.IsNullOrWhiteSpace(cedula))
                        {
                            if (cedula.Contains(".") || cedula.Contains(","))
                            {
                                LabelError.Text = "Favor verifique que su cédula sea correcta";
                            }
                            else
                            {
                                if (VerificaCedula(cedula.ToCharArray()) == true)
                                {
                                    LabelError.Text = "";
                                    identi = cedula;
                                    await NavigationHelper.SafePushAsync(Navigation, new step3telefono());
                                }
                                else
                                {
                                    LabelError.Text = "La cédula ingresada es incorrecta";
                                }

                            }

                        }
                        else
                        {
                            LabelError.Text = "Favor ingrese su cédula";
                        }
                    }
                    else
                    {
                        LabelError.Text = "Favor verifique que su cédula sea correcta";
                    }
                    
                }
                else
                {
                    LabelError.Text = "El usuario ya se encuentra registrado";
                }
                
                
            }
            else
            {
                LabelError.Text = "Favor intente mas tarde";
            }
            LoadingService.Hide();

        }
        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public static bool VerificaCedula(char[] validarCedula)
        {

            int aux = 0, par = 0, impar = 0, verifi;
            for (int i = 0; i < 9; i += 2)
            {
                aux = 2 * int.Parse(validarCedula[i].ToString());
                if (aux > 9)
                    aux -= 9;
                par += aux;
            }
            for (int i = 1; i < 9; i += 2)
            {
                impar += int.Parse(validarCedula[i].ToString());
            }

            aux = par + impar;
            if (aux % 10 != 0)
            {
                verifi = 10 - (aux % 10);
            }
            else
                verifi = 0;
            if (verifi == int.Parse(validarCedula[9].ToString()))
                return true;
            else
                return false;
        }
    }
}
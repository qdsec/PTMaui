using Acr.UserDialogs;
using Newtonsoft.Json;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using Windows.Graphics.Imaging;
//using Windows.Storage.Streams;
//using Xamarin.Essentials;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step5foto : ContentPage
    {
        public string sourc = "";
        public step5foto()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            string nombre = step1nombre.nom;
            string apellido = step1nombre.apell;
            string cedula = step2identificacion.identi;
            string telefono = step3telefono.telf;
            string email = step4email.correo;

            txtNombres.Text = nombre;
            txtEmail.Text = email;
            txtIdenti.Text = cedula;
            txtTelf.Text = telefono;

        }
        private async void Next_Click(object sender, EventArgs e)
        {
            string nombre = step1nombre.nom;
            string apellido = step1nombre.apell;
            string cedula = step2identificacion.identi;
            string telefono = step3telefono.telf;
            string email = step4email.correo;
            string contra = step1nombre.contra;

            LoadingService.Show("Cargando");            

                if (!String.IsNullOrWhiteSpace(nombre) && !String.IsNullOrWhiteSpace(apellido) &&
                !String.IsNullOrWhiteSpace(cedula) && !String.IsNullOrWhiteSpace(telefono) && !String.IsNullOrWhiteSpace(email))
                {
                    libreriaDatos.UsuariosDataInsert datos = new libreriaDatos.UsuariosDataInsert()
                    {
                        usu_ci = cedula,
                        usu_nombre = nombre,
                        usu_apellido = apellido,
                        usu_contrasena = contra,
                        usu_email = email,
                        usu_telefono = telefono,
                        usu_pic = ""

                    };
                    Uri RequestUri = new Uri("http://quantumdsec-001-site1.gtempurl.com/api/UsuariosOutputs");

                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(datos);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(RequestUri, contentJson);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await DisplayAlert("Finalizado", "Registro Completado", "Cerrar");
                        await NavigationHelper.SafePushAsync(Navigation, new LoginPage());
                        //App.Current.Logout();
                    }
                    else
                    {
                        await DisplayAlert("Atención", "Favor intente crear su cuenta mas tarde", "Cerrar");
                    }

                    
                }
                else
                {
                    await DisplayAlert("Atención", "Favor intente crear su cuenta mas tarde", "Cerrar");
                }
            LoadingService.Hide();
        }
        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btnImagePicker_Clicked(object sender, EventArgs e)
        {
            try
            {
                //var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                //{
                //    Title = "Por favor elija una foto"
                //});

                //var stream = await result.OpenReadAsync();
                //if (result != null)
                //{

                //    imgPickImage.Source = ImageSource.FromStream(() => stream);

                //    sourc = result.FullPath;

                //}

                //if (!String.IsNullOrWhiteSpace(imgPickImage.Source.ToString()))
                //{
                //    imgPickImage.Source = ImageSource.FromStream(() => stream);
                //}
                //else
                //{
                //    DisplayAlert("Mensaje", "Favor seleccione una imagen", "Aceptar");
                //}
            }
            catch (Exception ex)
            {

            }

        }

    }
}
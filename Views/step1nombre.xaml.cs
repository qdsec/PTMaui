using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step1nombre : ContentPage
    {
        public static string nom = "";
        public static string apell = "";
        public static string contra = "";
        public step1nombre()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void Next_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;
            string apellido = txtApellido.Text;
            string contrasena = txtContra.Text;
            string auxContrasena = txtAuxContra.Text;



            if (!String.IsNullOrWhiteSpace(nombre)&& !String.IsNullOrWhiteSpace(apellido)&& !String.IsNullOrWhiteSpace(contrasena) &&!String.IsNullOrWhiteSpace(auxContrasena))
            {
                if (contrasena.Length>3)
                {
                    if (contrasena == auxContrasena)
                    {
                        LabelError.Text = "";
                        nom = nombre;
                        apell = apellido;
                        contra = contrasena;
                        await NavigationHelper.SafePushAsync(Navigation, new step2identificacion());
                    }
                    else
                    {
                        LabelError.Text = "Las contraseñas no coinciden";
                    }
                }
                else
                {
                    LabelError.Text = "La contraseña debe tener almenos 4 caracteres";
                } 
                
            }
            else
            {
                LabelError.Text = "Favor rellene todos los campos";
            }
            
        }
        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void PasswordIcon_Clicked(object sender, EventArgs e)
        {
            txtAuxContra.IsPassword = !txtAuxContra.IsPassword;
            txtContra.IsPassword = !txtContra.IsPassword;

            if (txtAuxContra.IsPassword)
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
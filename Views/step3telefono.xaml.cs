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
    public partial class step3telefono : ContentPage
    {
        public static string telf = "";
        public step3telefono()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void Next_Click(object sender, EventArgs e)
        {
            string celular = txtCelular.Text;
            if (!String.IsNullOrWhiteSpace(celular))
            {
                if (celular.Contains(".") || celular.Contains(",")||celular.Length<10)
                {
                    LabelError.Text = "Favor verifique que su número de celular sea correcto";
                }
                else
                {
                    LabelError.Text = "";
                    telf = celular;
                    await NavigationHelper.SafePushAsync(Navigation, new step4email());
                }
            }
            else
            {
                LabelError.Text = "Favor ingrese su número de celular";
            }
            
        }
        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
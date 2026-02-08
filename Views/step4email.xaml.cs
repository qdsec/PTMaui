using Acr.UserDialogs;
using PeterTours.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using static PeterTours.GlobalData;

namespace PeterTours.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class step4email : ContentPage
    {
        SmtpClient SmtpServer;
        int codigo = 0;
        public static string correo = "";
        public step4email()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void VerificarMail_Click(object sender, EventArgs e)
        {
           // LoadingService.Show("Cargando");
            try
            {
                var email = txtEmail.Text.Trim();
                var emailPattern = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";


                if (!String.IsNullOrWhiteSpace(email) && (Regex.IsMatch(email, emailPattern)))
                {
                //    RandomNumber();
                //    //string mensaje = "Gracias por usar Peter Tours,\nIngrese el siguiente código de verificación en la aplicación: "+codigo;

                //    //            Label htmlLabel = new Label() { TextType = TextType.Html };
                //    string mensaje = @"<body style='background: #FAFAFA; color: #333333;'>
                //<center>
                //    <table border ='0' cellpadding ='20' cellspacing ='0' height ='100%' width ='600' style ='background: #ffffff; border: 1px solid #DDDDDD;' >
                //                   <tbody>
                //                       <tr>
                //                           <td><b>Gracias por usar Peter Tours,</b>
                //                                    <p> Ingrese el siguiente código de verificación en la aplicación: <h4> <strong>" + codigo.ToString() + @" </strong></h4></p>
                //                               </tr>
                //                               <tr>
                //                                   <td> --
                //                                       <br/> Peter Tours </ td >
                //                                 </tr>
                //                             </tbody>
                //                         </table>
                //                     </center>
                //                 </body> ";

                //    //LabelError.Text = "";
                //    //SmtpServer = new SmtpClient("smtp.gmail.com");
                //    //SmtpServer.Port = 587;
                //    //SmtpServer.Host = "smtp.gmail.com";
                //    //SmtpServer.EnableSsl = true;
                //    //SmtpServer.UseDefaultCredentials = false;
                //    //SmtpServer.html
                //    //SmtpServer.Credentials = new System.Net.NetworkCredential("qdssoporte@gmail.com", GlobalVariables.mailPass);
                //    //SmtpServer.SendAsync(GlobalVariables.fromMail, txtEmail.Text, "Verificación de cuenta", mensaje, codigo);
                //    //SmtpServer.SendCompleted += emailSendCompleted;

                //    MailMessage message = new MailMessage();
                //    message.From = new MailAddress(GlobalVariables.fromMail);
                //    message.To.Add(txtEmail.Text);
                //    message.Subject = "Verificación de cuenta";
                //    message.IsBodyHtml = true;
                //    message.Body = mensaje;

                //    SmtpClient smtpClient = new SmtpClient();
                //    smtpClient.UseDefaultCredentials = false;

                //    smtpClient.Host = "smtp.gmail.com";
                //    smtpClient.Port = 587;
                //    smtpClient.EnableSsl = true;
                //    smtpClient.Credentials = new System.Net.NetworkCredential("qdssoporte@gmail.com", GlobalVariables.mailPass);
                //    smtpClient.Send(message);
                //    //smtpClient.SendCompleted += emailSendCompleted;
                //    LabelError.Text = "";
                //    await DisplayAlert("Mensaje", "Favor revise su correo electrónico", "Aceptar");
                //    txtCodigo.IsVisible = true;
                //    btnNext.IsVisible = true;
                    //await NavigationHelper.SafePushAsync(Navigation, new step5foto());


                }
                else
                {
                    txtCodigo.IsVisible = false;
                    btnNext.IsVisible = false;
                    LabelError.Text = "Ingrese un correo electrónico válido";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Cerrar");
            }
        //    LoadingService.Hide();
            
        }
        public int RandomNumber()
        {
              Random _random = new Random();
            codigo= _random.Next(1000, 9999);
            return codigo;
        }

        private void emailSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DisplayAlert("Mensaje", "Favor revise su correo electrónico", "Aceptar");
        }
        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            //if (txtCodigo.Text == codigo.ToString())
            //{
            //    LabelError.Text = "";
            //    //LabelErrorCodigo.Text = "";
            //    correo = txtEmail.Text;
            //    await NavigationHelper.SafePushAsync(Navigation, new step5foto());
            //}
            var email = txtEmail.Text.Trim();
            var emailPattern = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";


            if (!String.IsNullOrWhiteSpace(email) && (Regex.IsMatch(email, emailPattern)))
            {
                LabelError.Text = "";
                //LabelErrorCodigo.Text = "";
                correo = txtEmail.Text.Trim();
                await NavigationHelper.SafePushAsync(Navigation, new step5foto());
            }
            else
            {
                LabelError.Text = "Ingrese un correo electrónico válido";
            }
        }
    }
}
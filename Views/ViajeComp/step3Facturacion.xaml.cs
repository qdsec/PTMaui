using PeterTours.Utils;
using Microsoft.Maui.Storage;

namespace PeterTours.Views.ViajeComp
{
    public partial class step3Facturacion : ContentPage
    {
        public step3Facturacion()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Preferences.Get("factura_guardada", false))
            {
                txtIdentificacion.Text = Preferences.Get("factura_id_saved", "");
                txtNombre.Text = Preferences.Get("factura_nombre_saved", "");
                txtCorreo.Text = Preferences.Get("factura_correo_saved", "");
                txtTelefono.Text = Preferences.Get("factura_telefono_saved", "");
                txtDireccion.Text = Preferences.Get("factura_direccion_saved", "");

                chkGuardarFacturacion.IsChecked = true;
            }
        }


        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            LabelError.Text = "";

            string cedula = txtIdentificacion.Text?.Trim() ?? "";
            string nombre = txtNombre.Text?.Trim() ?? "";
            string correo = txtCorreo.Text?.Trim() ?? "";
            string telefono = txtTelefono.Text?.Trim() ?? "";
            string direccion = txtDireccion.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(nombre) ||
                string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(telefono) ||
                string.IsNullOrEmpty(direccion))
            {
                LabelError.Text = "Por favor, complete todos los campos obligatorios.";
                return;
            }

            if (!correo.Contains("@") || !correo.Contains("."))
            {
                LabelError.Text = "Por favor, ingrese un correo electrónico válido.";
                return;
            }

            try
            {
                LoadingService.Show("Guardando datos");

                // 👉 Siempre se guardan para el flujo actual
                Preferences.Set("factura_id", cedula);
                Preferences.Set("factura_nombre", nombre);
                Preferences.Set("factura_correo", correo);
                Preferences.Set("factura_telefono", telefono);
                Preferences.Set("factura_direccion", direccion);

                // 👉 SOLO si el usuario acepta guardar
                if (chkGuardarFacturacion.IsChecked)
                {
                    Preferences.Set("factura_guardada", true);
                    Preferences.Set("factura_id_saved", cedula);
                    Preferences.Set("factura_nombre_saved", nombre);
                    Preferences.Set("factura_correo_saved", correo);
                    Preferences.Set("factura_telefono_saved", telefono);
                    Preferences.Set("factura_direccion_saved", direccion);
                }
                else
                {
                    Preferences.Remove("factura_guardada");
                }

                await NavigationHelper.SafePushAsync(Navigation, new step4Confirmar());
            }
            catch (Exception ex)
            {
                LabelError.Text = "Ocurrió un error al procesar la información.";
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                LoadingService.Hide();
            }
        }

    }
}
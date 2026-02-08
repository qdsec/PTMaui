using Microsoft.Maui.Controls;
using System.Globalization;
using System.Windows.Input;
using static PeterTours.libreriaDatos;

namespace PeterTours.Views.ViajeComp
{
    public partial class ViajeDetallePage : ContentPage
    {
        public ViajeDetallePage(CitasOutputXCedula viaje)
        {
            InitializeComponent();
            BindingContext = new ViajeDetalleViewModel(viaje);
        }
    }

    public class ViajeDetalleViewModel
    {
        // ===== COMUNES =====
        public string Titulo { get; }
        public string Fecha { get; }
        public string Hora { get; }
        public string Origen { get; }
        public string Destino { get; }
        public string Total { get; }

        // ===== FLAGS =====
        public bool EsViaje { get; }
        public bool EsEncomienda { get; }
        public bool MostrarParrilla { get; }
        public bool MostrarFoto { get; }

        // ===== VIAJE =====
        public string Nombre { get; }
        public string Telefono { get; }
        public string Pasajeros { get; }
        public string Requerimientos { get; }
        public string Hijos { get; }
        public string ParrillaTexto { get; }

        // ===== ENCOMIENDA =====
        public string Remitente { get; }
        public string TelRemitente { get; }
        public string Receptor { get; }
        public string TelReceptor { get; }
        public string DetalleEncomienda { get; }
        public string GestionRetiroTexto { get; }
        public string GestionEntregaTexto { get; }
        public string ValorEnvioTexto { get; }

        // ===== FOTO =====
        public ICommand VerImagenCommand { get; }

        // ===== TERMINOS =====
        public ICommand TapCommand { get; }
        public string DynamicUrl { get; }

        public ViajeDetalleViewModel(CitasOutputXCedula v)
        {
            Fecha = v.cit_fecha?.ToString("D", new CultureInfo("es-EC"));
            Hora = v.cit_hora;
            Origen = v.cit_origen;
            Destino = v.cit_destino;
            Total = v.cit_precio.HasValue ? $"${v.cit_precio:F2}" : "";

            EsViaje = v.cit_tipo == 1 || v.cit_tipo == 2;
            EsEncomienda = v.cit_tipo == 3;

            // ===== VIAJE =====
            if (EsViaje)
            {
                Nombre = v.cit_nombre;
                Telefono = v.cit_telf;
                Pasajeros = v.cit_cantidad_pasajeros?.ToString();
                Requerimientos = v.cit_detalles;
                Hijos = v.cit_hijos;
                MostrarParrilla = v.cit_tipo == 2;
                ParrillaTexto = v.cit_necesita_parrilla ? "Sí" : "No";

                Titulo = v.cit_tipo == 1
                    ? "🚌 Viaje Compartido"
                    : "🚐 Viaje VIP";
            }

            // ===== ENCOMIENDA =====
            if (EsEncomienda)
            {
                Remitente = v.cit_contacto_retiro_nombre;
                TelRemitente = v.cit_contacto_retiro_celular;
                Receptor = v.cit_contacto_destino_nombre;
                TelReceptor = v.cit_contacto_destino_celular;
                DetalleEncomienda = v.cit_descripcion_envio;

                GestionRetiroTexto = $"📦 Gestión Retiro: {(v.cit_gestion_retiro ? "Sí" : "No")}";
                GestionEntregaTexto = $"📦 Gestión Entrega: {(v.cit_gestion_entrega ? "Sí" : "No")}";
                ValorEnvioTexto = v.cit_valor_envio.HasValue ? $"💰 Valor de paquete: ${v.cit_valor_envio:F2}" : "";

                MostrarFoto = !string.IsNullOrWhiteSpace(v.cit_foto_encomienda);

                VerImagenCommand = new Command(async () =>
                {
                    await Launcher.OpenAsync(v.cit_foto_encomienda);
                });

                Titulo = "📦 Encomienda";
            }

            // ===== TERMINOS =====
            DynamicUrl = GetUrlForTime(v.cit_hora, (int)v.cit_tipo);
            TapCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(DynamicUrl))
                    await Launcher.OpenAsync(DynamicUrl);
            });
        }

        private string GetUrlForTime(string hora, int tipo)
        {
            int h = int.Parse(hora.Substring(0, 2));

            return tipo switch
            {
                1 when h <= 5 => "https://transpeters.com/terminos-y-condiciones-noche/",
                1 when h <= 9 => "https://transpeters.com/terminos-y-condiciones-nueveam/",
                1 => "https://transpeters.com/terminos-y-condiciones/",
                2 => "https://transpeters.com/terminos-y-condiciones-express/",
                3 => "https://transpeters.com/terminos-y-condiciones-encomiendas/",
                _ => "https://transpeters.com/terminos-y-condiciones/"
            };
        }
    }
}

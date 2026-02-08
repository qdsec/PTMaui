using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PeterTours
{
    public class libreriaDatos
    {
        #region GeoCoordinates
        public class GeoCoordinate
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }

            public GeoCoordinate(decimal latitude, decimal longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
        }
        public class GeoCoordinateID
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public List<GeoCoordinate> GeoCoordenadas { get; set; }

            public GeoCoordinateID(int iD, string nombre, List<GeoCoordinate> geoCoordenadas)
            {
                ID = iD;
                GeoCoordenadas = geoCoordenadas;
                Nombre = nombre;
            }
        }
        #endregion
        #region Usuarios
        public class UsuariosData
        {
            public int usu_id { get; set; }
            public string usu_ci { get; set; }
            public string usu_contrasena { get; set; }
            public string usu_nombre { get; set; }
            public string usu_apellido { get; set; }
            public string usu_email { get; set; }
            public string usu_telefono { get; set; }
            public string usu_pic { get; set; }
        }
        public class UsuariosDataInsert
        {
            public string usu_ci { get; set; }
            public string usu_contrasena { get; set; }
            public string usu_nombre { get; set; }
            public string usu_apellido { get; set; }
            public string usu_email { get; set; }
            public string usu_telefono { get; set; }
            public string usu_pic { get; set; }
        }
        #endregion

        #region Citas
        public class CitasDataInsert
        {
            public string cit_ci { get; set; }
            public string cit_nombre { get; set; }
            public int cit_cantidad_pasajeros { get; set; }
            public DateTime cit_fecha { get; set; }
            public string cit_hora { get; set; }
            public string cit_origen { get; set; }
            public string cit_destino { get; set; }
            public string cit_origen_lat { get; set; }
            public string cit_origen_lon { get; set; }
            public string cit_destino_lat { get; set; }
            public string cit_destino_lon { get; set; }
            public int cit_tipo { get; set; }
            public int cit_enviado { get; set; }
            public decimal cit_precio { get; set; }
            public string cit_rec_encom { get; set; }
            public string cit_rec_telf { get; set; }
            public string cit_rec_detalle { get; set; }
            public string cit_telf { get; set; }
            public string cit_detalles { get; set; }

        }
        public class CitasData
        {
            public int cit_id { get; set; }
            public string cit_ci { get; set; }
            public string cit_nombre { get; set; }
            public int cit_cantidad_pasajeros { get; set; }
            public DateTime cit_fecha { get; set; }
            public string cit_hora { get; set; }
            public string cit_origen { get; set; }
            public string cit_destino { get; set; }
            public string cit_origen_lat { get; set; }
            public string cit_origen_lon { get; set; }
            public string cit_destino_lat { get; set; }
            public string cit_destino_lon { get; set; }
            public int cit_tipo { get; set; }
            public int cit_enviado { get; set; }
            public decimal cit_precio { get; set; }
            public string cit_tipo_pal { get; set; }
            public string cit_rec_encom { get; set; }
            public string cit_rec_telf { get; set; }
            public string cit_rec_detalle { get; set; }
            public string cit_telf { get; set; }
            public string cit_detalles { get; set; }
            public bool IsVisible { get; set; } = false;
        }
        public class CitaOutput
        {
            public int cit_id { get; set; }
            public string cit_ci { get; set; }
            public string cit_nombre { get; set; }
            public int cit_cantidad_pasajeros { get; set; }
            public DateTime cit_fecha { get; set; }
            public string cit_hora { get; set; }
            public string cit_origen { get; set; }
            public string cit_destino { get; set; }
            public string cit_origen_lat { get; set; }
            public string cit_origen_lon { get; set; }
            public string cit_destino_lat { get; set; }
            public string cit_destino_lon { get; set; }
            public int cit_tipo { get; set; }
            public int cit_enviado { get; set; }
            public decimal cit_precio { get; set; }
            public string cit_tipo_pal { get; set; }
            public string cit_rec_encom { get; set; }
            public string cit_rec_telf { get; set; }
            public string cit_rec_detalle { get; set; }
            public string cit_detalles { get; set; }
            public string correo { get; set; }
            public string cit_telf { get; set; }
        }
        public class CitasOutputXCedula
        {
            /* =========================
               PK
               ========================= */
            [Key]
            public int cit_id { get; set; }

            /* =========================
               DATOS PRINCIPALES
               ========================= */
            public string cit_ci { get; set; }
            public string cit_nombre { get; set; }
            public int? cit_cantidad_pasajeros { get; set; }

            public DateTime? cit_fecha { get; set; }
            public string cit_hora { get; set; }

            public string cit_origen { get; set; }
            public string cit_destino { get; set; }

            public string cit_origen_lat { get; set; }
            public string cit_origen_lon { get; set; }
            public string cit_destino_lat { get; set; }
            public string cit_destino_lon { get; set; }

            /* =========================
               TIPO
               ========================= */
            public int? cit_tipo { get; set; }
            public string cit_tipo_pal { get; set; }

            /* =========================
               ESTADO / PRECIO
               ========================= */
            public int? cit_enviado { get; set; }
            public decimal? cit_precio { get; set; }

            /* =========================
               ENCOMIENDA BÁSICA
               ========================= */
            public string cit_rec_encom { get; set; }
            public string cit_rec_telf { get; set; }
            public string cit_rec_detalle { get; set; }

            public string cit_telf { get; set; }

            /* =========================
               DETALLES CALCULADOS
               ========================= */
            public string cit_detalles { get; set; }
            public string cit_hijos { get; set; }

            /* =========================
               CONFIRMACIÓN
               ========================= */
            public int? cit_confirmado { get; set; }
            public DateTime? cit_fecha_confirmación { get; set; }

            /* =========================
               REFERIDOS / RESPONSABLE
               ========================= */
            public string cit_cedula_referido { get; set; }
            public string cit_cedula_responsable { get; set; }

            /* =========================
               ENVÍO
               ========================= */
            public string cit_descripcion_envio { get; set; }
            public decimal? cit_valor_envio { get; set; }

            /* =========================
               RETIRO
               ========================= */
            public string cit_contacto_retiro_nombre { get; set; }
            public string cit_contacto_retiro_celular { get; set; }
            public bool cit_gestion_retiro { get; set; }

            /* =========================
               ENTREGA
               ========================= */
            public string cit_contacto_destino_nombre { get; set; }
            public string cit_contacto_destino_celular { get; set; }
            public bool cit_gestion_entrega { get; set; }

            /* =========================
               OTROS
               ========================= */
            public string cit_foto_encomienda { get; set; }
            public bool cit_necesita_parrilla { get; set; }

            /* =========================
               USUARIO
               ========================= */
            public string usu_email { get; set; }

            /* =========================
               FACTURACIÓN
               ========================= */
            public string cit_cedula_fac { get; set; }
            public string cit_nombre_rc_fac { get; set; }
            public string cit_correo_fac { get; set; }
            public string cit_telefono_fac { get; set; }
            public string cit_direccion_fac { get; set; }

            /* =========================
               SENTIDO
               ========================= */
            public int? cit_sentido { get; set; }
            public string cit_sentido_origen { get; set; }
            public string cit_sentido_destino { get; set; }
        }


        #endregion

        #region QHorario
        public class QhorarioDataInsert
        {
            public DateTime qh_fecha { get; set; }
            public string qh_hora { get; set; }
            public int qh_estado { get; set; }
            public string qh_sentido { get; set; }

        }
        public class QhorarioData
        {
            public int qh_id { get; set; }
            public DateTime qh_fecha { get; set; }
            public string qh_hora { get; set; }
            public int qh_estado { get; set; }
            public string qh_sentido { get; set; }
        }
        #endregion

        #region Os
        public class OsDataInsert
        {
            public string ov_detalle { get; set; }
            public string ov_version { get; set; }

        }
        public class OsData
        {
            public int ov_id { get; set; }
            public string ov_detalle { get; set; }
            public string ov_version { get; set; }
        }
        #endregion
        #region Horarios
        public class HorariosDataInsert
        {
            public string ho_hora { get; set; }
            public string ho_especial { get; set; }
            public int ho_estado { get; set; }

        }
        public class HorariosData
        {
            public int ho_id { get; set; }
            public string ho_hora { get; set; }
            public string ho_especial { get; set; }
            public int ho_estado { get; set; }
        }
        #endregion
        #region QExpress
        public class QExpressDataInsert
        {
            public DateTime qhs_fecha { get; set; }
            public int qhs_estado { get; set; }

        }
        public class QExpressData
        {
            public int qhs_id { get; set; }
            public DateTime qhs_fecha { get; set; }
            public int qhs_estado { get; set; }
            public string qhs_estado_pal { get; set; }
        }
        #endregion

        #region 
        public class LugaresViajes
        {
            public string cl_nombre { get; set; }
            public string lc_latitud { get; set; }
            public string lc_longitud { get; set; }
        }
        public class LugaresViajesOutput
        {
            public int cl_id { get; set; }
            public string cl_nombre { get; set; }
            public string lc_latitud { get; set; }
            public string lc_longitud { get; set; }
        }
        public class CatalogoLugares
        {
            public string cl_nombre { get; set; }
            public int cl_estado { get; set; }
        }
        public class CatalogoLugaresOutput
        {
            public int cl_id { get; set; }
            public string cl_nombre { get; set; }
            public int cl_estado { get; set; }
        }
        public class PreciosLugares
        {
            public int cl_id { get; set; }
            public int lc_id { get; set; }
            public decimal plc_tarifa { get; set; }
            public int plc_estado { get; set; }
        }
        public class PreciosLugaresOutput
        {
            public int plc_id { get; set; }
            public int cl_id { get; set; }
            public int lc_id { get; set; }
            public decimal plc_tarifa { get; set; }
            public int plc_estado { get; set; }
        }
        public class DescuentosViajes
        {
            public string dv_nombre { get; set; }
            public decimal dv_porcentaje { get; set; }
            public DateTime? dv_fecha_inicio { get; set; }
            public DateTime? dv_fecha_fin { get; set; }
            public int dv_estado { get; set; }
        }
        public class DescuentosViajesOutput
        {
            public int dv_id { get; set; }
            public string dv_nombre { get; set; }
            public decimal dv_porcentaje { get; set; }
            public DateTime? dv_fecha_inicio { get; set; }
            public DateTime? dv_fecha_fin { get; set; }
            public int dv_estado { get; set; }
        }
        public class DescuentosViajesRealizados
        {
            public int dv_id { get; set; }
            public string vcd_ci_usuario { get; set; }
        }
        public class DescuentosViajesRealizadosOutput
        {
            public int vcd_id { get; set; }
            public int dv_id { get; set; }
            #endregion


            public class DisponibilidadAsientosInput
            {
                public int Asientos_extra { get; set; }
                public DateTime Fecha { get; set; }
                public string Hora { get; set; }
                public int Accion { get; set; }
                public int Sentido { get; set; }
            }
            public class DisponibilidadAsientosOutput
            {
                public DateTime Fecha { get; set; }
                public string Hora { get; set; }
                public int Asientos_Disponibles { get; set; }
            }
            public class HorariosNewOutput
            {
                public int ho_id { get; set; }
                public string ho_hora { get; set; }

            }
        }
    }
}

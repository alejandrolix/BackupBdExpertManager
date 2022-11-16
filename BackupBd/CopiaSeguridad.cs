using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupBd
{
    internal class CopiaSeguridad
    {
        private string? CadConexionBd;
        private string? UrlArchivoBackup { get; set; }
        private string EsContenedor;
        public string NombreBackupAGenerar { get; set; }

        public CopiaSeguridad(string esContenedor)
        {
            EsContenedor = esContenedor;

            CargarVariablesEntorno();
            ComprobarValorVariablesEntorno();
        }

        private void CargarVariablesEntorno()
        {
            CadConexionBd = Environment.GetEnvironmentVariable("CAD_CONEXION_BD");
            UrlArchivoBackup = Environment.GetEnvironmentVariable("URL_ARCHIVO_BACKUP");
        }

        private void ComprobarValorVariablesEntorno()
        {
            if (EsContenedor == "S")
            {
                if (CadConexionBd is null)
                {
                    throw new Exception("Variable de entorno CAD_CONEXION_BD no definida");
                }

                if (UrlArchivoBackup is null)
                {
                    throw new Exception("Variable de entorno UrlArchivoBackup no definida");
                }
            }
            else
            {
                if (CadConexionBd is null)
                {
                    // Localhost.
                    CadConexionBd = "Server=(localdb)\\mssqllocaldb;Database=ExpertManager;Trusted_Connection=True;MultipleActiveResultSets=true";
                }

                if (UrlArchivoBackup is null)
                {
                    UrlArchivoBackup = $@"C:\Users\admin\Desktop\backup_{DateTime.Now:dd}_{DateTime.Now:MM}_{DateTime.Now:yyyy}.bak";
                }
            }
        }

        public void HacerCopia()
        {
            if (EsContenedor == "S")
            {
                NombreBackupAGenerar = $"{UrlArchivoBackup}backup_{DateTime.Now:dd}_{DateTime.Now:MM}_{DateTime.Now:yyyy}.bak";
            }
            else
            {
                NombreBackupAGenerar = UrlArchivoBackup;
            }         

            if (File.Exists(NombreBackupAGenerar))
            {
                try
                {
                    File.Delete(NombreBackupAGenerar);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                Console.WriteLine($"Eliminado archivo {NombreBackupAGenerar} porque ya existe");
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(CadConexionBd))
                {
                    string sql = $@"BACKUP DATABASE [ExpertManager] TO DISK='{NombreBackupAGenerar}';";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void EliminarBackupsMasDe5Dias()
        {
            string separador;

            if (EsContenedor == "S")
            {
                separador = "/";
            }
            else
            {
                separador = "\\";
            }

            foreach (string archivo in Directory.GetFiles(UrlArchivoBackup))
            {
                if (!archivo.Contains("backup_"))
                {
                    continue;
                }
                
                string[] partesNombreArchivo = archivo.Split(separador);
                string nombreArchivoConFecha = partesNombreArchivo[partesNombreArchivo.Length - 1];
                int dia = int.Parse(nombreArchivoConFecha.Substring(7, 2));
                int mes = int.Parse(nombreArchivoConFecha.Substring(10, 2));
                int anio = int.Parse(nombreArchivoConFecha.Substring(13, 4));

                DateTime fechaBackup = new DateTime(anio, mes, dia);
                double numDiasDiferencia = (DateTime.Now.Date - fechaBackup).TotalDays;

                if (numDiasDiferencia >= 5)
                {
                    File.Delete(archivo);
                    Console.WriteLine($"Eliminado archivo backup {archivo} porque tiene 5 o más días antigüedad");
                }
            }
        }
    }
}

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
        private string? UrlCarpetaBackups { get; set; }
        private string EsContenedor;
        private string SeparadorCarpetas { get; set; }
        public string UrlBackupAGenerar { get; set; }

        public CopiaSeguridad(string esContenedor)
        {
            EsContenedor = esContenedor;

            CargarVariablesEntorno();
            ComprobarValorVariablesEntorno();
        }

        private void CargarVariablesEntorno()
        {
            CadConexionBd = Environment.GetEnvironmentVariable("CAD_CONEXION_BD");
            UrlCarpetaBackups = Environment.GetEnvironmentVariable("URL_CARPETA_BACKUPS");
        }

        private void ComprobarValorVariablesEntorno()
        {
            if (EsContenedor == "S")
            {
                if (CadConexionBd is null)
                {
                    throw new Exception("Variable de entorno CAD_CONEXION_BD no definida");
                }

                if (UrlCarpetaBackups is null)
                {
                    throw new Exception("Variable de entorno URL_CARPETA_BACKUPS no definida");
                }

                SeparadorCarpetas = "/";
            }
            else
            {
                if (CadConexionBd is null)
                {
                    // Localhost.
                    CadConexionBd = "Server=(localdb)\\mssqllocaldb;Database=ExpertManager;Trusted_Connection=True;MultipleActiveResultSets=true";
                }

                if (UrlCarpetaBackups is null)
                {
                    UrlCarpetaBackups = $@"C:\Users\admin\Desktop";
                }

                SeparadorCarpetas = "\\";
            }
        }

        public void HacerCopia()
        {
            UrlBackupAGenerar = $"{UrlCarpetaBackups}{SeparadorCarpetas}backup_{DateTime.Now:dd}_{DateTime.Now:MM}_{DateTime.Now:yyyy}.bak";    

            if (File.Exists(UrlBackupAGenerar))
            {
                try
                {
                    File.Delete(UrlBackupAGenerar);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                Console.WriteLine($"Eliminado archivo {UrlBackupAGenerar} porque ya existe");
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(CadConexionBd))
                {
                    string sql = $@"BACKUP DATABASE [ExpertManager] TO DISK='{UrlBackupAGenerar}';";

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

            Console.WriteLine("bd exportada. Enviando por mail");
        }

        public void EliminarBackupsMasDe5Dias()
        {
            List<string> archivos = Directory.GetFiles(UrlCarpetaBackups)
                                             .Where(archivo => archivo.Contains("backup_"))
                                             .ToList();
            try
            {
                foreach (string archivo in archivos)
                {
                    string[] partesNombreArchivo = archivo.Split(SeparadorCarpetas);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

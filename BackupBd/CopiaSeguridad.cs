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
        public string? UrlArchivoBackup { get; set; }
        private string EsContenedor;

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
                UrlArchivoBackup = UrlArchivoBackup + $"backup_{DateTime.Now:dd}_{DateTime.Now:MM}_{DateTime.Now:yyyy}.bak";
            }

            try
            {
                using (SqlConnection conexion = new SqlConnection(CadConexionBd))
                {
                    string sql = $@"BACKUP DATABASE [ExpertManager] TO DISK='{UrlArchivoBackup}';";

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
    }
}

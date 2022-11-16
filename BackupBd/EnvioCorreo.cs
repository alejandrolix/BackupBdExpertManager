using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackupBd
{
    internal class EnvioCorreo
    {
        private string? EmailOrigen;
        private string? EmailDestino;
        private string? PasswordGmail;
        private string UrlArchivoBackup;

        public EnvioCorreo(string esContenedor, string urlArchivoBackup)
        {
            CargarVariablesEntorno();
            UrlArchivoBackup = urlArchivoBackup;

            ComprobarValorVariablesEntorno(esContenedor);
        }

        private void CargarVariablesEntorno()
        {
            EmailOrigen = Environment.GetEnvironmentVariable("EMAIL_ORIGEN");
            EmailDestino = Environment.GetEnvironmentVariable("EMAIL_DESTINO");
            PasswordGmail = Environment.GetEnvironmentVariable("PASSWORD_GMAIL");
        }

        private void ComprobarValorVariablesEntorno(string esContenedor)
        {
            if (esContenedor == "S")
            {
                if (EmailOrigen is null)
                {
                    throw new Exception("Variable de entorno EMAIL_ORIGEN no definida");
                }

                if (EmailDestino is null)
                {
                    throw new Exception("Variable de entorno EMAIL_DESTINO no definida");
                }

                if (PasswordGmail is null)
                {
                    throw new Exception("Variable de entorno PASSWORD_GMAIL no definida");
                }
            }
            else
            {
                if (EmailOrigen is null)
                {
                    EmailOrigen = "alejandro.pons98@gmail.com";
                }

                if (EmailDestino is null)
                {
                    EmailDestino = "alejandro.pons98@gmail.com";
                }
            }
        }

        public void EnviarCorreo()
        {
            MailAddress origen = new MailAddress($"{EmailOrigen}", "BackupBD");
            MailAddress destino = new MailAddress($"{EmailDestino}", "BackupBD");

            string asunto = $"Backup {DateTime.Now.ToString("dd/MM/yyyy")}";

            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(origen.Address, PasswordGmail)
                };

                Attachment adjunto;
                adjunto = new Attachment($"{UrlArchivoBackup}");

                MailMessage mensaje = new MailMessage(origen, destino)
                {
                    Subject = asunto
                };

                mensaje.Attachments.Add(adjunto);
                smtp.Send(mensaje);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine("Correo enviado!");
        }
    }
}

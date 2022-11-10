using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

string? cadConexionBd = Environment.GetEnvironmentVariable("CAD_CONEXION_BD");

if (cadConexionBd is null)
{
    cadConexionBd = "Server=(localdb)\\mssqllocaldb;Database=ExpertManager;Trusted_Connection=True;MultipleActiveResultSets=true";
}

try
{
    using (SqlConnection conexion = new SqlConnection(cadConexionBd))
    {
        string sql = $@"BACKUP DATABASE [ExpertManager] TO DISK='C:\Users\admin\Desktop\bd.bak';";

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

try
{
    var fromAddress = new MailAddress("alejandro.pons98@gmail.com", "BackupBD");
    var toAddress = new MailAddress("alejandro.pons98@gmail.com", "BackupBD");
    const string fromPassword = "rcyzxndfyhbewqrf";
    const string subject = "Subject";
    const string body = "Body";

    var smtp = new SmtpClient
    {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
    };

    System.Net.Mail.Attachment attachment;
    attachment = new System.Net.Mail.Attachment("C:\\Users\\admin\\Desktop\\bd.bak");

    using (var message = new MailMessage(fromAddress, toAddress)
    {
        Subject = subject,
        Body = body
    })
    {
        message.Attachments.Add(attachment);
        smtp.Send(message);
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
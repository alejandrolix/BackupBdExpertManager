using BackupBd;

string? esContenedor = Environment.GetEnvironmentVariable("ES_CONTENEDOR");

if (esContenedor is null)
{
    esContenedor = "N";
}

CopiaSeguridad copiaSeguridad = new CopiaSeguridad(esContenedor);
copiaSeguridad.HacerCopia();
copiaSeguridad.EliminarBackupsMasDe5Dias();

Console.WriteLine("bd exportada. Enviando por mail");

EnvioCorreo envioCorreo = new EnvioCorreo(esContenedor, copiaSeguridad.NombreBackupAGenerar);
envioCorreo.EnviarCorreo();

Console.WriteLine("Correo enviado!");
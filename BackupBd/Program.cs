using BackupBd;

string? esContenedor = Environment.GetEnvironmentVariable("ES_CONTENEDOR");

if (esContenedor is null)
{
    esContenedor = "N";
}

CopiaSeguridad copiaSeguridad = new CopiaSeguridad(esContenedor);
copiaSeguridad.HacerCopia();
copiaSeguridad.EliminarBackupsMasDe5Dias();

EnvioCorreo envioCorreo = new EnvioCorreo(esContenedor, copiaSeguridad.UrlBackupAGenerar);
envioCorreo.EnviarCorreo();

Console.WriteLine("Correo enviado!");
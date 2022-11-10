using BackupBd;

string? esContenedor = Environment.GetEnvironmentVariable("ES_CONTENEDOR");

if (esContenedor is null)
{
    esContenedor = "N";
}

CopiaSeguridad copiaSeguridad = new CopiaSeguridad(esContenedor);
copiaSeguridad.HacerCopia();

Console.WriteLine("bd exportada. Enviando por mail");

EnvioCorreo envioCorreo = new EnvioCorreo(esContenedor, copiaSeguridad.UrlArchivoBackup);
envioCorreo.EnviarCorreo();
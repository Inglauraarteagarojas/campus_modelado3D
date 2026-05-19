using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var contentTypeProvider = new FileExtensionContentTypeProvider();

contentTypeProvider.Mappings[".glb"] = "model/gltf-binary";
contentTypeProvider.Mappings[".gltf"] = "model/gltf+json";
contentTypeProvider.Mappings[".bin"] = "application/octet-stream";
contentTypeProvider.Mappings[".png"] = "image/png";
contentTypeProvider.Mappings[".jpg"] = "image/jpeg";
contentTypeProvider.Mappings[".jpeg"] = "image/jpeg";
contentTypeProvider.Mappings[".webp"] = "image/webp";

app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider,
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=3600");
    }
});

app.MapGet("/api/estado", () =>
{
    return Results.Json(new
    {
        mensaje = "Campus VR funcionando correctamente",
        modelo = "campusv2.glb",
        entorno = "Realidad Virtual WebXR"
    });
});

app.MapGet("/api/verificar-modelo", () =>
{
    var rutaModelo = Path.Combine(app.Environment.WebRootPath, "campusv2.glb");

    if (!File.Exists(rutaModelo))
    {
        return Results.Json(new
        {
            existe = false,
            mensaje = "No existe wwwroot/campusv2.glb en el servidor Render."
        });
    }

    var info = new FileInfo(rutaModelo);

    string inicioArchivo = "";
    using (var reader = new StreamReader(rutaModelo))
    {
        char[] buffer = new char[120];
        reader.Read(buffer, 0, buffer.Length);
        inicioArchivo = new string(buffer);
    }

    var esPunteroGitLfs = inicioArchivo.Contains("git-lfs.github.com/spec");

    return Results.Json(new
    {
        existe = true,
        archivo = "wwwroot/campusv2.glb",
        pesoBytes = info.Length,
        pesoMB = Math.Round(info.Length / 1024.0 / 1024.0, 2),
        esPunteroGitLfs,
        mensaje = esPunteroGitLfs
            ? "El archivo en Render es solo un puntero Git LFS. Render no descargó el modelo real."
            : "El modelo GLB sí está cargado como archivo real."
    });
});

app.Run();

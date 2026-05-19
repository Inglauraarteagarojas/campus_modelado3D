using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".glb"] = "model/gltf-binary";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
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

app.Run();
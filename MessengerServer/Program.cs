using Firebase.Database;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using MessengerService.CollectionExtensions;
using MessengerService.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Firebase
var firebaseConfig = builder.Configuration.GetSection("Firebase");
var databaseUrl = firebaseConfig["DatabaseUrl"];

// Agrega el servicio FirebaseClient usando la URL de la base de datos
builder.Services.AddSingleton<FirebaseClient>(provider =>
{
    return new FirebaseClient(databaseUrl);
});

// Registrar los servicios de Messenger, incluyendo los repositorios y entidades
builder.Services.AddMessengerServices();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

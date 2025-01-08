using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using MessengerService.IServices;
using MessengerService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:7279");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddScoped<IGenericRepository<MessengerDomain.Entities.User>>(provider =>
{
    var firebaseClient = provider.GetRequiredService<FirebaseClient>();
    return new GenericRepository<MessengerDomain.Entities.User>(firebaseClient, "user");
});

builder.Services.AddScoped<IGenericRepository<Chat>>(provider =>
{
    var firebaseClient = provider.GetRequiredService<FirebaseClient>();
    return new GenericRepository<Chat>(firebaseClient, "chats");
});

builder.Services.AddScoped<IGenericRepository<UserConnection>>(provider =>
{
    var firebaseClient = provider.GetRequiredService<FirebaseClient>();
    return new GenericRepository<UserConnection>(firebaseClient, "userConnection");
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configuraci�n de Firebase
var firebaseConfig = builder.Configuration.GetSection("Firebase");
var databaseUrl = firebaseConfig["DatabaseUrl"];
var storageUrl = firebaseConfig["StorageUrl"];
var credentialsFile = firebaseConfig["CredentialsFile"];

// Agrega el servicio FirebaseClient usando la URL de la base de datos
builder.Services.AddSingleton<FirebaseClient>(provider =>
{
    return new FirebaseClient(databaseUrl);
});

builder.Services.AddSingleton<FirebaseStorage>(provider =>
{
    return new FirebaseStorage(storageUrl);
});

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(credentialsFile)
});

builder.Services.AddScoped<FirebaseStorageService>();

var apiKey = firebaseConfig["ApiKey"];
var authDomain = firebaseConfig["AuthDomain"];

builder.Services.AddSingleton<FirebaseAuthClient>(firebaseAuth =>
{
    return new FirebaseAuthClient(new FirebaseAuthConfig {
        ApiKey = apiKey,
        AuthDomain = authDomain,
        Providers = [new EmailProvider()],
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                   .AllowCredentials();
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

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SignalRHub>("/chatHub");
});

app.Run();

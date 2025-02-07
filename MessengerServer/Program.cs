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
using MessengerService.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
var projectId = firebaseConfig["ProjectID"];

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

builder.Services.AddSingleton<IFirebaseAuthClient>(firebaseAuth =>
{
    return new FirebaseAuthClient(new FirebaseAuthConfig {
        ApiKey = apiKey,
        AuthDomain = authDomain,
        Providers = [new EmailProvider()],
    });
});

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<String>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = $"https://securetoken.google.com/{projectId}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/{projectId}",

        ValidateAudience = true,
        ValidAudience = $"{projectId}",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    // Habilitar SignalR para leer tokens desde la QueryString
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // Si la solicitud es para SignalR, asigna el token
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatHub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
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

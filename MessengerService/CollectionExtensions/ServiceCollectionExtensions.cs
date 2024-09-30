using Firebase.Database;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using MessengerService.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.CollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessengerServices(this IServiceCollection services)
        {
            // Registrar el repositorio genérico para User (entidad)
            services.AddScoped<IGenericRepository<User>>(provider =>
            {
                var firebaseClient = provider.GetRequiredService<FirebaseClient>();
                return new GenericRepository<User>(firebaseClient, "user");
            });

            services.AddScoped<IGenericRepository<Chat>>(provider =>
            {
                var firebaseClient = provider.GetRequiredService<FirebaseClient>();
                return new GenericRepository<Chat>(firebaseClient, "chats");
            });

            // Registrar otros servicios o repositorios si es necesario

            // Registrar UserService
            services.AddScoped<UserService>();
            services.AddScoped<ChatService>();
            return services;
        }
    }
}

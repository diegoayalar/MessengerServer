using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerPersistency.Repository
{
    public class FirebaseStorageService
    {
        private readonly FirebaseStorage _firebaseStorage;

        public FirebaseStorageService(FirebaseStorage firebaseStorage)
        {
            _firebaseStorage = firebaseStorage;
        }
        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                 await _firebaseStorage
                    .Child("profile_pictures")   // Carpeta en Firebase Storage
                    .Child(fileName)             // Nombre del archivo
                    .PutAsync(fileStream);       // Archivo como stream
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir el archivo: {ex.Message}", ex);
            }
        }

        public async Task<string> GetFileAsync(string fileName)
        {
            try 
            {
                return await _firebaseStorage
                    .Child("profile_pictures")
                    .Child(fileName)
                    .GetDownloadUrlAsync();
            }catch (Exception ex) 
            {
                throw new Exception($"Error al obtener el archivo: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                await _firebaseStorage
                    .Child("profile_pictures")   // Carpeta en Firebase Storage
                    .Child(fileName)             // Nombre del archivo
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el archivo: {ex.Message}", ex);
            }
        }

    }
}

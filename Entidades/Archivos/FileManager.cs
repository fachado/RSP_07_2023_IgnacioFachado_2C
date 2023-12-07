using System;
using System.IO;
using Newtonsoft.Json;

namespace Entidades.Files
{
    public static class FileManager
    {
        private static readonly string path;

        static FileManager()
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SP_07122023_ALUMNO");
            ValidaExistenciaDeDirectorio();
        }

        private static void ValidaExistenciaDeDirectorio()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                // Utiliza el método Guardar para escribir en el archivo logs.txt
                Guardar($"Error al crear el directorio: {ex.Message}", "logs.txt", true);
                // Relanza la excepción original
                throw new FileManagerException("Error al crear el directorio", ex);
            }
        }

        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            string ruta = Path.Combine(path, nombreArchivo);

            try
            {
                ValidaExistenciaDeDirectorio();

                using (StreamWriter sw = new StreamWriter(ruta, append))
                {
                    sw.WriteLine(data);
                }

               
            }
            catch (Exception ex)
            {
                // Utiliza el método Guardar para escribir en el archivo logs.txt
                Guardar($"Error al guardar el archivo '{nombreArchivo}': {ex.Message}", "logs.txt", true);
                // Relanza la excepción original
                throw new FileManagerException($"Error al guardar el archivo '{nombreArchivo}'", ex);
            }
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo) where T : class
        {
            try
            {
                string ruta = Path.Combine(path, nombreArchivo);
                string data = JsonConvert.SerializeObject(elemento);
                File.WriteAllText(ruta, data);
                return true;
            }
            catch (Exception ex)
            {
                // Utiliza el método Guardar para escribir en el archivo logs.txt
                Guardar($"Error al serializar el archivo '{nombreArchivo}': {ex.Message}", "logs.txt", true);
                // Relanza la excepción original
                throw new FileManagerException($"Error al serializar el archivo '{nombreArchivo}'", ex);
            }
        }
    }

    public class FileManagerException : Exception
    {
        public FileManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
using Entidades.Interfaces;
using System;
using System.Data.SqlClient;
using Entidades.Files;

namespace Entidades
{
    public static class DataBaseManager
    {
        private static readonly string connectionString;

        static DataBaseManager()
        {
            // Inicializar el string de conexión en el constructor estático
            connectionString = "Server=.;Database=20230622SP;Trusted_Connection=True;";
        }

        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT imagen FROM Comidas WHERE tipo_comida = @tipo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tipo", tipo);

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else
                    {

                        throw new ComidaInvalidaException($"Tipo de comida '{tipo}' no encontrado en la base de datos.");
                    }
                }
            }
            catch (SqlException ex)
            {
                FileManager.Guardar($"Error al leer datos de la base de datos: {ex.Message}", "logs.txt", true);

                throw new DataBaseManagerException($"Error al leer datos de la base de datos: {ex.Message}", ex);
            }
        }

        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string descripcionComida = comida.Ticket;
                    string query = $"INSERT INTO Tickets (empleado, ticket) VALUES (@nombreEmpleado, @descripcionComida)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombreEmpleado",nombreEmpleado);
                    command.Parameters.AddWithValue("@descripcionComida", descripcionComida);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqlException ex)
            {
                FileManager.Guardar($"Error al escribir datos en la base de datos: {ex.Message}", "logs.txt", true);

                throw new DataBaseManagerException($"Error al escribir datos en la base de datos: {ex.Message}", ex);
            }
        }
    }



    public class ComidaInvalidaException : Exception
    {
        public ComidaInvalidaException(string message) : base(message) { }
    }

    // Nueva excepción personalizada para manejar excepciones de DataBaseManager
    public class DataBaseManagerException : Exception
    {
        public DataBaseManagerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
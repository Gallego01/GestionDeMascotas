using GestionDeMascotas.Context;
using GestionDeMascotas.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeMascotas.Services
{
    public class PerroService
    {
        private readonly MascotaContext mascotaContext;

        // Constructor que inicializa el contexto de base de datos
        public PerroService()
        {
            mascotaContext = new MascotaContext();
        }

        // Agrega un perro a la tabla Perro con los datos vinculados al ID de mascota
        public void AgregarPerroASQL(Perro perro, int mascotaId, SqlConnection connection)
        {
            string query = @"INSERT INTO Perro (mascotaId, raza, color, vacunado)
                        VALUES (@mascotaId, @raza, @color, @vacunado)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@mascotaId", mascotaId);
                command.Parameters.AddWithValue("@raza", perro.Raza);
                command.Parameters.AddWithValue("@color", perro.Color);
                command.Parameters.AddWithValue("@vacunado", perro.Vacunado);

                command.ExecuteNonQuery();
            }
        }

        // Solicita datos por consola para cargar un perro y devuelve una instancia con esos datos
        public Perro CargarDatosDePerro(string nombre, string especie, string sexo, float peso, DateTime dateTime, int edadEnMeses)
        {
            Console.WriteLine("Ingrese la raza: ");
            string raza = Console.ReadLine();

            Console.WriteLine("Ingrese el color: ");
            string color = Console.ReadLine();

            Console.WriteLine("verificando si esta vacunado...");
            bool vacunado = GuardClause.GuardClause.VerificarVacunacionPorFecha(dateTime);

            try
            {
                string vacuna = GuardClause.GuardClause.esquemaVacunas[edadEnMeses];
                Console.WriteLine($"\n{nombre} ({especie}) - Edad: {edadEnMeses} meses, Vacuna: {vacuna}");
            }
            catch
            {
                Console.WriteLine($"\n{nombre} ({especie}) - Edad: {edadEnMeses} meses, no corresponde vacunacion en este momento");
            }

            return new Perro
            {
                Nombre = nombre,
                Especie = especie,
                Sexo = sexo,
                Peso = peso,
                FechaNacimiento = dateTime,
                EdadEnMeses = edadEnMeses,
                Raza = raza,
                Color = color,
                Vacunado = vacunado,
            };
        }

        // Obtiene los datos del perro desde un SqlDataReader
        public Perro TraerDatosDePerro(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string nombre = reader.GetString(1);
            string especie = reader.GetString(2);
            string sexo = reader.GetString(3);
            float peso = (float)reader.GetDouble(4);
            DateTime fechaNacimiento = reader.GetDateTime(5);
            int edadEnMeses = reader.GetInt32(6);

            string raza = reader.IsDBNull(7) ? "" : reader.GetString(7);
            string color = reader.IsDBNull(8) ? "" : reader.GetString(8);
            bool vacunado = !reader.IsDBNull(9) && reader.GetBoolean(9);

            return new Perro(id, nombre, especie, sexo, peso, fechaNacimiento, edadEnMeses, raza, color, vacunado);
        }

        // Edita un campo específico del perro en la base de datos
        public void EditarDatoDePerro(int mascotaId, string nombreCampo, object nuevoValor)
        {
            string query = $"UPDATE Perro SET {nombreCampo} = @valor WHERE mascotaId = @id";
            using (var connection = mascotaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@valor", nuevoValor);
                command.Parameters.AddWithValue("@id", mascotaId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Elimina los datos de un perro según el ID de mascota
        public void BorrarDatosDePerro(int id)
        {
            var query = "DELETE FROM Perro WHERE mascotaId = @id";
            using (var connection = mascotaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}

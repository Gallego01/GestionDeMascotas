using GestionDeMascotas.Context;
using GestionDeMascotas.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeMascotas.Services
{
    public class MascotaService
    {
        private readonly MascotaContext mascotaContext;
        PerroService perroService = new PerroService();
        GatoService gatoService = new GatoService();

        public MascotaService()
        {
            mascotaContext = new MascotaContext();
        }

        // Agrega una mascota a la base de datos incluyendo datos especificos de perro o gato
        public void AgregarMascotaASQL(Mascota mascota)
        {
            try
            {
                using (var connection = mascotaContext.GetConnection())
                {
                    connection.Open();

                    // Consulta para insertar datos basicos en tabla Mascota y obtener el id generado
                    string query = @"INSERT INTO Mascota (nombre, especie, sexo, peso, fechaNacimiento, edadEnMeses)
                        VALUES (@nombre, @especie, @sexo, @peso, @fechaNacimiento, @edadEnMeses);
                        SELECT CAST(scope_identity() AS int)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", mascota.Nombre);
                    command.Parameters.AddWithValue("@especie", mascota.Especie);
                    command.Parameters.AddWithValue("@sexo", mascota.Sexo);
                    command.Parameters.AddWithValue("@peso", mascota.Peso);
                    command.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);
                    command.Parameters.AddWithValue("@edadEnMeses", mascota.EdadEnMeses);

                    // Ejecuta el insert y obtiene el id generado
                    int mascotaId = (int)command.ExecuteScalar();

                    try
                    {
                        // Segun la especie, guarda los datos adicionales en tabla Perro o Gato
                        switch (mascota.Especie.Trim().ToLower())
                        {
                            case "perro":
                                perroService.AgregarPerroASQL((Perro)mascota, mascotaId, connection);
                                break;
                            case "gato":
                                gatoService.AgregarGatoASQL((Gato)mascota, mascotaId, connection);
                                break;
                        }
                    }
                    catch { }
                    Console.WriteLine("\nMascota agregada exitosamente");
                }
            }
            catch
            {
                Console.WriteLine("\nError al cargar la mascota");
            }
        }

        // Permite cargar una mascota desde consola y la guarda en la base de datos
        public void CargarMascota()
        {
            int opcionSeguir;
            do
            {
                Console.WriteLine("Ingrese el nombre: ");
                string nombre = Console.ReadLine();

                Console.WriteLine("Ingrese la especie: ");
                string especie = GuardClause.GuardClause.MostrarMenuMascota(false); 

                Console.WriteLine("Ingrese el sexo: ");
                string sexo = GuardClause.GuardClause.MostrarMenuSexo();

                Console.WriteLine("Ingrese el peso: ");
                float peso = float.Parse(Console.ReadLine());

                Console.WriteLine("¿Desea agregar una Fecha aleatoria?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                int opcionFecha = GuardClause.GuardClause.ValidarOpcion(1, 2);
                DateTime dateTime;
                if (opcionFecha == 1)
                {
                   dateTime = GuardClause.GuardClause.AsignarEdadAleatoria();
                }
                else
                {
                    Console.WriteLine("Ingrese la fecha de nacimiento: ");
                    dateTime = DateTime.Parse(Console.ReadLine());
                }

                // Calcula edad en meses a partir de la fecha de nacimiento
                int edadEnMeses = GuardClause.GuardClause.CalcularEdadEnMeses(dateTime);
                Console.WriteLine($"Edad en Meses: {edadEnMeses}");

                Mascota mascota = null;

                // Crea objeto correspondiente segun especie
                if (especie.Trim().ToLower() == "perro")
                {
                    mascota = perroService.CargarDatosDePerro(nombre, especie, sexo, peso, dateTime, edadEnMeses);
                }
                else if (especie.Trim().ToLower() == "gato")
                {
                    mascota = gatoService.CargarDatosDeGato(nombre, especie, sexo, peso, dateTime, edadEnMeses);
                }
                else if (especie.Trim().ToLower() == "otros")
                {
                    mascota = new Mascota
                    {
                        Nombre = nombre,
                        Especie = especie,
                        Sexo = sexo,
                        Peso = peso,
                        FechaNacimiento = dateTime,
                        EdadEnMeses = edadEnMeses
                    };
                }

                // Inserta la mascota en la base de datos
                AgregarMascotaASQL(mascota);

                Console.WriteLine("\n¿Desea agregar otra mascota?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcionSeguir = GuardClause.GuardClause.ValidarOpcion(1, 2);

            } while (opcionSeguir != 2);
        }

        // Devuelve una lista de todas las mascotas aplicando un filtro por especie
        public List<Mascota> ObtenerTodasLasMascotas(string filtro)
        {
            List<Mascota> mascotas = new List<Mascota>();

            // Consulta para traer datos basicos de mascotas y datos adicionales de perros o gatos
            string query = @"SELECT m.id, m.nombre, m.especie, m.sexo, m.peso, m.fechaNacimiento, m.edadEnMeses,
                                    p.raza AS razaPerro, p.color AS colorPerro, p.vacunado AS vacunadoPerro,
                                    g.raza AS razaGato, g.color AS colorGato, g.vacunado AS vacunadoGato
                            FROM Mascota m
                            LEFT JOIN Perro p ON m.id = p.mascotaId
                            LEFT JOIN Gato g ON m.id = g.mascotaId";

            using (SqlConnection connection = mascotaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string especie = reader.GetString(2);

                    // Si el filtro no es "todos" y la especie no coincide, salta el registro
                    if (filtro.Trim().ToLower() != "todos" && especie != filtro)
                    {
                        continue;
                    }

                    Mascota mascota = null;

                    // Crea objeto segun especie con datos completos
                    switch (especie.Trim().ToLower())
                    {
                        case "perro":
                            mascota = perroService.TraerDatosDePerro(reader);
                            break;

                        case "gato":
                            mascota = gatoService.TraerDatosDeGato(reader);
                            break;

                        case "otros":
                            mascota = TraerDatosDeOtros(reader);
                            break;
                    }
                    mascotas.Add(mascota);
                }
                reader.Close();
            }
            return mascotas;
        }

        // Permite modificar los datos de una mascota seleccionada por nombre
        public void ModificarMascotaPorNombre()
        {
            int opcion;
            do
            {
                Mascota mascotaSeleccionada = SeleccionarMascotaPorNombre();


                if (mascotaSeleccionada != null)
                {
                    Console.WriteLine("\nDatos actuales de la mascota: ");
                    Console.WriteLine(mascotaSeleccionada.ToString());
                }
                else { return; }
                
                Console.WriteLine("\nSeleccione el campo que desea modificar:");
                string[] camposDeMascota = GuardClause.GuardClause.ObtenerDatosDeMascotaModificables(mascotaSeleccionada);

                for (int i = 0; i < camposDeMascota.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {camposDeMascota[i]}");
                }

                Console.Write("Ingresar opcion: ");
                int opcionDeCampo = GuardClause.GuardClause.ValidarOpcion(1, camposDeMascota.Length);
                
                string campoSeleccionado = camposDeMascota[opcionDeCampo - 1];
                string nuevoValor;

                // Segun el campo, pide el nuevo valor con menus especificos o lectura directa
                if (campoSeleccionado.Trim().ToLower() == "especie")
                {
                    Console.Write($"\nIngrese nuevo valor para {campoSeleccionado}:\n");
                    nuevoValor = GuardClause.GuardClause.MostrarMenuMascota(false);
                }
                else if (campoSeleccionado.Trim().ToLower() == "sexo")
                {
                    Console.Write($"\nIngrese nuevo valor para {campoSeleccionado}:\n");
                    nuevoValor = GuardClause.GuardClause.MostrarMenuSexo();
                }
                else
                {
                    Console.Write($"\nIngrese nuevo valor para {campoSeleccionado}: ");
                    nuevoValor = Console.ReadLine();
                }

                var camposBaseDeMascota = new[] { "nombre", "especie", "sexo", "peso", "fechaNacimiento", "edadEnMeses" };

                // Si el campo es base, edita en tabla Mascota, sino en tabla especifica de Perro o Gato
                if (camposBaseDeMascota.Contains(campoSeleccionado.Trim().ToLower()))
                {
                    EditarCampoMascota(mascotaSeleccionada.Id, campoSeleccionado, nuevoValor);
                }
                else
                {
                    switch (mascotaSeleccionada.Especie.Trim().ToLower())
                    {
                        case "perro":
                            perroService.EditarDatoDePerro(mascotaSeleccionada.Id, campoSeleccionado, nuevoValor);
                            break;
                        
                        case "gato":
                            gatoService.EditarDatoDeGato(mascotaSeleccionada.Id, campoSeleccionado, nuevoValor);
                            break;

                        case "otros":
                            EditarCampoMascota(mascotaSeleccionada.Id, campoSeleccionado, nuevoValor);
                            break;
                    }
                }

                Console.WriteLine("\nCampo modificado correctamente");

                Console.WriteLine("\n¿Desea modificar otro campo de mascota?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcion = GuardClause.GuardClause.ValidarOpcion(1, 2);
            } while (opcion != 2);
        }

        // Edita un campo de la tabla Mascota por id
        public void EditarCampoMascota(int id, string nombreCampo, object nuevoValor)
        {
            string query = $"UPDATE Mascota SET {nombreCampo} = @valor WHERE id = @id";

            using (var connection = mascotaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@valor", nuevoValor);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Permite buscar una mascota por nombre y borrarla si el usuario confirma
        public void BuscarYBorrarMascota()
        {
            int opcion;
            do
            {
                Mascota mascotaSeleccionada = SeleccionarMascotaPorNombre();
                
                if (mascotaSeleccionada != null)
                {
                    Console.WriteLine("\nDatos actuales de la mascota: ");
                    Console.WriteLine(mascotaSeleccionada.ToString());

                    Console.WriteLine("\n¿Desea borrar esta mascota?");
                    Console.WriteLine("1 - SI");
                    Console.WriteLine("2 - NO");
                    Console.Write("Ingresar opcion: ");

                    int opcionBorrar = GuardClause.GuardClause.ValidarOpcion(1, 2);

                    if (opcionBorrar == 1)
                    {
                        // Borra datos adicionales segun especie en tablas Perro o Gato
                        switch (mascotaSeleccionada.Especie.Trim().ToLower())
                        {
                            case "perro":
                                perroService.BorrarDatosDePerro(mascotaSeleccionada.Id);
                                break;
                            case "gato":
                                gatoService.BorrarDatosDeGato(mascotaSeleccionada.Id);
                                break;
                        }

                        // Borra la mascota en la tabla Mascota
                        BorrarMascota(mascotaSeleccionada.Id);
                        Console.WriteLine("\nMascota eliminado exitosamente");
                    }
                    else
                    {
                        Console.WriteLine("\nMascota no eliminada");
                    }
                }

                Console.WriteLine("\n¿Desea borrar otra mascota?");
                Console.WriteLine("1 - SI");
                Console.WriteLine("2 - NO");
                opcion = GuardClause.GuardClause.ValidarOpcion(1, 2);
            } while (opcion != 2);
        }

        // Borra una mascota de la tabla Mascota por id
        public void BorrarMascota(int id)
        {
            using (var connection = mascotaContext.GetConnection())
            {
                string query = "DELETE FROM Mascota WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Muestra las mascotas que deberian recibir una vacuna segun su edad
        public void ObtenerMascotasVacunables()
        {
            string filtro = GuardClause.GuardClause.MostrarMenuMascota();
            List<Mascota> todas = ObtenerTodasLasMascotas(filtro);

            foreach (var mascota in todas)
            {
                if (GuardClause.GuardClause.VerificarVacunacionPorFecha(mascota.FechaNacimiento))
                {
                    int edad = GuardClause.GuardClause.CalcularEdadEnMeses(mascota.FechaNacimiento);
                    string vacuna = GuardClause.GuardClause.esquemaVacunas[edad];
                    Console.WriteLine($"{mascota.Nombre} ({mascota.Especie}) - Edad: {edad} meses, Vacuna: {vacuna}");
                }
            }
        }

        // Crea una mascota del tipo Otros a partir de la columna "especie" de la base de datos
        public Mascota TraerDatosDeOtros(SqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string nombre = reader.GetString(1);
            string especie = reader.GetString(2);
            string sexo = reader.GetString(3);
            float peso = (float)reader.GetDouble(4);
            DateTime fechaNacimiento = reader.GetDateTime(5);
            int edadEnMeses = reader.GetInt32(6);

            return new Mascota(id, nombre, especie, sexo, peso, fechaNacimiento, edadEnMeses);
        }

        // Permite buscar una mascota por nombre o parte del nombre y seleccionarla
        public Mascota SeleccionarMascotaPorNombre()
        {
            Console.Write("Ingrese el nombre o parte del nombre a buscar: ");
            string buscarMascota = Console.ReadLine();

            List<Mascota> listaMascota = BuscarMascotaPorFiltro(buscarMascota);

            if (listaMascota.Count == 0)
            {
                Console.WriteLine("\nNo se encontro ninguna mascota");
                return null;
            }

            if (listaMascota.Count == 1)
            {
                return listaMascota[0];
            }

            Console.WriteLine("\nSe encontraron varias mascotas, seleccione una: ");
            for (int i = 0; i < listaMascota.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {listaMascota[i].Nombre} ({listaMascota[i].Especie})");
            }

            int seleccion = GuardClause.GuardClause.ValidarOpcion(1, listaMascota.Count);
            return listaMascota[seleccion - 1];
        }

        // Realiza una busqueda de mascotas por nombre parcial usando SQL LIKE y devuelve la lista
        public List<Mascota> BuscarMascotaPorFiltro(string filtro)
        {
            List<Mascota> mascotas = new List<Mascota>();

            string query = @"SELECT m.id, m.nombre, m.especie, m.sexo, m.peso, m.fechaNacimiento, m.edadEnMeses,
                                    p.raza AS razaPerro, p.color AS colorPerro, p.vacunado AS vacunadoPerro,
                                    g.raza AS razaGato, g.color AS colorGato, g.vacunado AS vacunadoGato
                            FROM Mascota m
                            LEFT JOIN Perro p ON m.id = p.mascotaId
                            LEFT JOIN Gato g ON m.id = g.mascotaId
                            WHERE m.nombre LIKE @nombre";

            using (SqlConnection connection = mascotaContext.GetConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@nombre", $"%{filtro}%");

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string especie = reader.GetString(2);
                    Mascota mascota = null;

                    switch (especie.Trim().ToLower())
                    {
                        case "perro":
                            mascota = perroService.TraerDatosDePerro(reader);
                            break;

                        case "gato":
                            mascota = gatoService.TraerDatosDeGato(reader);
                            break;

                        case "otros":
                            mascota = TraerDatosDeOtros(reader);
                            break;
                    }

                    mascotas.Add(mascota);
                }

                reader.Close();
            }

            return mascotas;
        }
    }
}

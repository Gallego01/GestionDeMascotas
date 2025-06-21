using GestionDeMascotas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeMascotas.GuardClause
{
    public class GuardClause
    {
        // Valida si una opcion ingresada por el usuario esta dentro del rango especificado
        // Solicita repetidamente un numero hasta que sea valido
        // Devuelve la opcion validada
        public static int ValidarOpcion(int minimo, int maximo)
        {
            bool pudo = false;
            int opcion = 0;
            while (!pudo)
            {
                pudo = int.TryParse(Console.ReadLine(), out opcion);
                if (!pudo || opcion < minimo || opcion > maximo)
                {
                    pudo = false;
                    Console.WriteLine(string.Concat("Solo numeros entre ", minimo, " y ", maximo, ".\nIntente nuevamente: "));
                }
            }
            return opcion;
        }

        // Muestra un menu con los tipos de mascotas disponibles
        // Devuelve el string correspondiente a la opcion seleccionada.
        public static string MostrarMenuMascota(bool incluirTodos = true)
        {
            List<string> tiposDeMascotas = new List<string> { "Perro", "Gato", "Otros" };

            if (incluirTodos)
            {
                tiposDeMascotas.Add("Todos");
            }

            for (int i = 0; i < tiposDeMascotas.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {tiposDeMascotas[i]}");
            }

            Console.Write("Seleccionar la especie: ");
            int opcion = ValidarOpcion(1, tiposDeMascotas.Count);

            return tiposDeMascotas[opcion - 1];
        }

        // Muestra un menu con los tipos de sexo disponibles para mascotas
        // Devuelve el string seleccionado por el usuario
        public static string MostrarMenuSexo()
        {
            List<string> tiposDeSexo = new List<string> { "Macho", "Hembra", "Otros" };

            for (int i = 0; i < tiposDeSexo.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {tiposDeSexo[i]}");
            }

            Console.Write("Seleccionar la especie: ");
            int opcion = ValidarOpcion(1, tiposDeSexo.Count);

            return tiposDeSexo[opcion - 1];
        }

        // Asigna una fecha de nacimiento aleatoria
        // Calcula una fecha restando un numero aleatorio de años (entre 1 y 15) a la fecha actual
        public static DateTime AsignarEdadAleatoria()
        {
            Random random = new Random();
            int edadEnAnios = random.Next(1, 15);
            DateTime fechaAleatoria = DateTime.Today.AddYears(-edadEnAnios);
            return fechaAleatoria;
        }

        // Diccionario que representa el esquema de vacunas segun edad en meses
        public static Dictionary<int, string> esquemaVacunas = new Dictionary<int, string>
        {
            {2, "Vacuna multiple (Puppy o Triple felina)"},
            {3, "Refuerzo multiple + Rabia"},
            {4, "Refuerzo final (opcional)"},
            {12, "Refuerzo anual de todas las vacunas"},
            {24, "Segundo refuerzo anual"},
            {36, "Tercer refuerzo anual"},
            {48, "Cuarto refuerzo anual"},
            {60, "Quinto refuerzo anual"},
        };

        // Calcula la edad en meses a partir de la fecha de nacimiento
        public static int CalcularEdadEnMeses(DateTime fechaNacimiento)
        {
            DateTime hoy = DateTime.Today;
            int años = hoy.Year - fechaNacimiento.Year;
            int meses = hoy.Month - fechaNacimiento.Month;

            if (hoy.Day < fechaNacimiento.Day)
            {
                meses--;
            }

            int edadTotalEnMeses = (años * 12) + meses;
            return edadTotalEnMeses;
        }

        // Verifica si una mascota debe vacunarse segun su edad y el esquema de vacunas
        // Devuelve true si existe una vacuna para la edad calculada
        public static bool VerificarVacunacionPorFecha(DateTime fechaNacimiento)
        {
            int edadEnMeses = CalcularEdadEnMeses(fechaNacimiento);
            return esquemaVacunas.ContainsKey(edadEnMeses);
        }

        // Convierte un valor booleano nullable a string "Si" o "No"
        // Si el valor es null devuelve null
        public static string ConversorBool(bool? valor)
        {
            if (valor == null)
            {
                return null;
            }

            return valor.Value ? "Si" : "No";
        }

        // Obtiene los campos modificables para una mascota
        // Devuelve un arreglo con los nombres de los campos que pueden modificarse segun la especie
        public static string[] ObtenerDatosDeMascotaModificables(Mascota mascota)
        {
            List<string> camposDeMascota = new List<string>
            {
                "Nombre", "Especie", "Sexo", "Peso", "Fecha Nacimiento", "Edad (meses)"
            };

            switch (mascota.Especie.Trim().ToLower())
            {
                case "perro":
                    camposDeMascota.AddRange(new string[] { "Raza", "Color", "Vacunado" });
                    break;

                case "gato":
                    camposDeMascota.AddRange(new string[] { "Raza", "Color", "Vacunado" });
                    break;
            }

            return camposDeMascota.ToArray();
        }
    }
}

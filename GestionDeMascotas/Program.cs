using GestionDeMascotas.Models;
using GestionDeMascotas.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeMascotas
{
    class Program
    {
        static void Main(string[] args)
        {
            MascotaService mascotaService = new MascotaService();
            JsonSerializerService jsonSerializerService = new JsonSerializerService();

            int opcion;
            do
            {
                opcion = Menu();

                switch (opcion)
                {
                    case 1:
                        mascotaService.CargarMascota();
                        break;
                    case 2:
                        string filtro = GuardClause.GuardClause.MostrarMenuMascota();
                        List<Mascota> mascotas = mascotaService.ObtenerTodasLasMascotas(filtro);
                        foreach (var mascota in mascotas)
                        {
                            Console.WriteLine(mascota.ToString());
                        }
                        break;
                    case 3:
                        mascotaService.ModificarMascotaPorNombre();
                        break;
                    case 4:
                        mascotaService.BuscarYBorrarMascota();
                        break;
                    case 5:
                        mascotaService.ObtenerMascotasVacunables();
                        break;
                    case 6:
                        Mascota mascotaSeleccionada = mascotaService.SeleccionarMascotaPorNombre();
                        Console.WriteLine("\nDatos actuales de la mascota: ");
                        Console.WriteLine(mascotaSeleccionada.ToString()); ;
                        break;
                    case 7:
                        string filtroXml = GuardClause.GuardClause.MostrarMenuMascota(false);
                        List<Mascota> mascotasXml = mascotaService.ObtenerTodasLasMascotas(filtroXml);
                        jsonSerializerService.GuardarMascotasComoJson(mascotasXml, filtroXml, $"{filtroXml}.txt");
                        break;
                    case 8:
                        string filtroJson = GuardClause.GuardClause.MostrarMenuMascota(false);
                        List<Mascota> mascotasJson = mascotaService.ObtenerTodasLasMascotas(filtroJson);
                        jsonSerializerService.LeerYMostrarMascotasDesdeJson($"{filtroJson}.txt", filtroJson);
                        break;
                }
            } while (opcion != 0);
        }

        private static int Menu()
        {
            Console.WriteLine("\n############# MENU DEL PRINCIPAL #############\n");
            Console.WriteLine("1 - AGREGAR MASCOTA");
            Console.WriteLine("2 - MOSTRAR MASCOTAS");
            Console.WriteLine("3 - MODIFICAR MASCOTA");
            Console.WriteLine("4 - ELIMINAR MASCOTA");
            Console.WriteLine("5 - MOSTRAR MASCOTAS VACUNABLES");
            Console.WriteLine("6 - VER DETALLES DE UNA MASCOTA");
            Console.WriteLine("7 - DESCARGAR ARCHIVO (MASCOTA).TXT");
            Console.WriteLine("8 - MOSTRAR ARCHIVO (MASCOTA).TXT");
            Console.WriteLine("0 - SALIR\n");

            int opcion = GuardClause.GuardClause.ValidarOpcion(0, 8);

            return opcion;
        }
    }
}

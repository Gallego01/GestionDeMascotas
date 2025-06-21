using GestionDeMascotas.Context;
using GestionDeMascotas.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GestionDeMascotas.Services
{
    public class JsonSerializerService
    {
        private readonly MascotaContext mascotaContext;

        public JsonSerializerService()
        {
            mascotaContext = new MascotaContext();
        }

        // Guarda una lista de mascotas en formato json dependiendo de la especie seleccionada
        // Si la especie es perro se filtran las mascotas que son perros y se serializan como lista de Perro
        // Si la especie es gato se filtran las mascotas que son gatos y se serializan como lista de Gato
        // Si la especie es otros se filtran las mascotas con especie otros y se serializan como lista base Mascota
        // El archivo json se guarda por nombreArchivo
        public void GuardarMascotasComoJson(List<Mascota> mascotas, string especie, string nombreArchivo)
        {
            if (especie.Trim().ToLower() == "perro")
            {
                List<Perro> perros = new List<Perro>();

                foreach (var mascota in mascotas)
                {
                    if (mascota.Especie.Trim().ToLower() == "perro" && mascota is Perro perro)
                    {
                        perros.Add(perro);
                    }
                }

                string perrosJson = JsonSerializer.Serialize(perros, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(nombreArchivo, perrosJson);
            }
            else if (especie.Trim().ToLower() == "gato")
            {
                List<Gato> gatos = new List<Gato>();

                foreach (var mascota in mascotas)
                {
                    if (mascota.Especie.Trim().ToLower() == "gato" && mascota is Gato gato)
                    {
                        gatos.Add(gato);
                    }
                }

                string gatosJson = JsonSerializer.Serialize(gatos, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(nombreArchivo, gatosJson);
            }
            else if (especie.Trim().ToLower() == "otros")
            {
                var otros = mascotas.Where(m => m.Especie.Trim().ToLower() == "otros").ToList();

                string otrosJson = JsonSerializer.Serialize(otros, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(nombreArchivo, otrosJson);
            }

            Console.WriteLine($"\nArchivo JSON se guardado como: {nombreArchivo}");
        }

        // Lee un archivo json y muestra en consola los datos de las mascotas segun su especie
        public void LeerYMostrarMascotasDesdeJson(string nombreArchivo, string especie)
        {
            try
            {
                // Evalua la especie para deserializar la lista correspondiente
                switch (especie.Trim().ToLower())
                {
                    case "perro":
                        List<Perro> perros = JsonSerializer.Deserialize<List<Perro>>(File.ReadAllText(nombreArchivo));
                        foreach (var perro in perros)
                        {
                            Console.WriteLine($"[Perro] Id: {perro.Id}, Nombre: {perro.Nombre}, Especie: {perro.Especie}, Sexo: {perro.Sexo}, Peso: {perro.Peso}kg, " +
                                              $"\nFecha Nac: {perro.FechaNacimiento:dd/MM/yyyy}, Edad: {perro.EdadEnMeses} meses, Raza: {perro.Raza}, Color: {perro.Color}, " +
                                              $"Vacunado: {perro.Vacunado}");
                        }
                        break;

                    case "gato":
                        List<Gato> gatos = JsonSerializer.Deserialize<List<Gato>>(File.ReadAllText(nombreArchivo));
                        foreach (var gato in gatos)
                        {
                            Console.WriteLine($"[Gato] Id: {gato.Id}, Nombre: {gato.Nombre}, Especie: {gato.Especie}, Sexo: {gato.Sexo}, Peso: {gato.Peso}kg, " +
                                              $"\nFecha Nac: {gato.FechaNacimiento:dd/MM/yyyy}, Edad: {gato.EdadEnMeses} meses, Raza: {gato.Raza}, Color: {gato.Color}, " +
                                              $"Vacunado: {gato.Vacunado}");
                        }
                        break;

                    case "otros":
                        List<Mascota> otros = JsonSerializer.Deserialize<List<Mascota>>(File.ReadAllText(nombreArchivo));
                        foreach (var m in otros)
                        {
                            Console.WriteLine($"[Otros] Id: {m.Id}, Nombre: {m.Nombre}, Especie: {m.Especie}, Sexo: {m.Sexo}, Peso: {m.Peso}kg, " +
                                              $"\nFecha Nac: {m.FechaNacimiento:dd/MM/yyyy}, Edad: {m.EdadEnMeses} meses");
                        }
                        break;
                }
            }
            catch
            {
                Console.WriteLine($"\nError al leer el archivo {nombreArchivo}");
            }
        }
    }
}

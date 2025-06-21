using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GestionDeMascotas.Models
{
    [Serializable]
    [XmlInclude(typeof(Perro))]
    [XmlInclude(typeof(Gato))]
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Sexo { get; set; }
        public float Peso { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int EdadEnMeses { get; set; }

        public Mascota() { }

        public Mascota(int id, string nombre, string especie, string sexo, float peso, DateTime fechaNacimiento, int edadEnMeses)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Especie = especie;
            this.Sexo = sexo;
            this.Peso = peso;
            this.FechaNacimiento = fechaNacimiento;
            this.EdadEnMeses = edadEnMeses;
        }

        public override string ToString()
        {
            return string.Concat($"\nId: {Id}",
                                 $"\nNombre: {Nombre}",
                                 $"\nEspecie: {Especie}",
                                 $"\nSexo: {Sexo}",
                                 $"\nPeso: {Peso}Kg",
                                 $"\nFecha De Nacimiento: {FechaNacimiento.ToShortDateString()}",
                                 $"\nEdad (Meses): {EdadEnMeses}");
        }
    }
}

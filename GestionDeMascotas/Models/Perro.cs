using GestionDeMascotas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeMascotas.Models
{
    public class Perro : Mascota, IVacunable
    {
        public string Raza { get; set; }
        public string Color { get; set; }
        public bool Vacunado { get; set; }

        public Perro() { }

        public Perro(int id, string nombre, string especie, string sexo, float peso, DateTime fechaNacimiento, int edadEnMeses, string raza, string color, bool vacunado)
            : base(id, nombre, especie, sexo, peso, fechaNacimiento, edadEnMeses)
        {
            this.Raza = raza;
            this.Color = color;
            this.Vacunado = vacunado;
        }

        public override string ToString()
        {
            return base.ToString() + string.Concat($"\nRaza: {Raza}",
                                                   $"\nColor: {Color}",
                                                   $"\nVacunado?: {GuardClause.GuardClause.ConversorBool(Vacunado)}");
        }

        public void Vacunar()
        {
            GuardClause.GuardClause.VerificarVacunacionPorFecha(this.FechaNacimiento);
        }
    }
}

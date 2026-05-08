using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Services
{
    public class EstrategiaEquilibrada : IEstrategiaAsignacion
    {
        public string Nombre => "Equilibrada";

        public Dictionary<int, decimal> Asignar(List<Meta> metas, decimal excedente)
        {
            var resultado = new Dictionary<int, decimal>();

            var metasActivas = metas
                .Where(m => m.PorcentajeCompletado < 100)
                .ToList();

            if (!metasActivas.Any())
                return resultado;

            decimal montoPorMeta = Math.Round(excedente / metasActivas.Count, 2);

            foreach (var meta in metasActivas)
            {
                decimal faltante = meta.MontoObjetivo - meta.MontoActual;
                resultado[meta.Id] = Math.Min(faltante, montoPorMeta);
            }

            return resultado;
        }
    }
}
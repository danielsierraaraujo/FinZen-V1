using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Services
{
    public class EstrategiaPorPrioridad : IEstrategiaAsignacion
    {
        public string Nombre => "Por Prioridad";

        public Dictionary<int, decimal> Asignar(List<Meta> metas, decimal excedente)
        {
            var resultado = new Dictionary<int, decimal>();

            var metasActivas = metas
                .Where(m => m.PorcentajeCompletado < 100)
                .ToList();

            if (!metasActivas.Any())
                return resultado;

            decimal pesoTotal = metasActivas.Sum(m => m.Prioridad);

            foreach (var meta in metasActivas)
            {
                decimal proporcion = meta.Prioridad / pesoTotal;
                decimal montoAsignado = Math.Round(excedente * proporcion, 2);
                resultado[meta.Id] = montoAsignado;
            }

            return resultado;
        }
    }
}
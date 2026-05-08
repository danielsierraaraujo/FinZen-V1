using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Services
{
    public class EstrategiaPorUrgencia : IEstrategiaAsignacion
    {
        public string Nombre => "Por Urgencia";

        public Dictionary<int, decimal> Asignar(List<Meta> metas, decimal excedente)
        {
            var resultado = new Dictionary<int, decimal>();

            var metasActivas = metas
                .Where(m => m.PorcentajeCompletado < 100)
                .ToList();

            if (!metasActivas.Any())
                return resultado;

            var metaCasi = metasActivas
                .FirstOrDefault(m => m.PorcentajeCompletado >= 95);

            if (metaCasi != null)
            {
                decimal faltante = metaCasi.MontoObjetivo - metaCasi.MontoActual;
                decimal asignado = Math.Min(faltante, excedente);
                resultado[metaCasi.Id] = asignado;
                excedente -= asignado;
                metasActivas.Remove(metaCasi);
            }

            if (!metasActivas.Any() || excedente <= 0)
                return resultado;

            var metasConUrgencia = metasActivas
                .Select(m => new
                {
                    Meta = m,
                    Urgencia = m.DiasRestantes <= 0 ? 1000m : 1m / m.DiasRestantes
                })
                .ToList();

            decimal urgenciaTotal = metasConUrgencia.Sum(m => m.Urgencia);

            foreach (var item in metasConUrgencia)
            {
                decimal proporcion = item.Urgencia / urgenciaTotal;
                decimal montoAsignado = Math.Round(excedente * proporcion, 2);
                resultado[item.Meta.Id] = montoAsignado;
            }

            return resultado;
        }
    }
}
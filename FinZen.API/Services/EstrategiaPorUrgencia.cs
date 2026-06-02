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
            decimal dineroSobrante = excedente;

            var metasActivas = metas.Where(m => m.PorcentajeCompletado < 100).ToList();
            foreach (var m in metasActivas) { resultado[m.Id] = 0; }

            if (!metasActivas.Any()) return resultado;

            // 1. EL QUICK WIN (La victoria rápida)
            var metaCasi = metasActivas.FirstOrDefault(m => (m.MontoActual / m.MontoObjetivo) >= 0.95m);
            if (metaCasi != null)
            {
                decimal faltante = metaCasi.MontoObjetivo - metaCasi.MontoActual;
                decimal asignado = Math.Min(faltante, dineroSobrante);
                
                if (asignado > 0)
                {
                    resultado[metaCasi.Id] += asignado;
                    dineroSobrante -= asignado;
                }
                
                // Actualizamos la lista para sacar la que acabamos de llenar
                metasActivas = metasActivas.Where(m => (m.MontoObjetivo - m.MontoActual - resultado[m.Id]) > 0).ToList();
            }

            // 2. EL BUCLE DE REDISTRIBUCIÓN (Para el resto del dinero)
            while (dineroSobrante > 0 && metasActivas.Any())
            {
                // Calculamos las urgencias SOLO de las metas que siguen en la mesa
                var metasConUrgencia = metasActivas.Select(m => new {
                    Meta = m,
                    Urgencia = m.DiasRestantes <= 0 ? 1000m : 1m / m.DiasRestantes
                }).ToList();

                decimal urgenciaTotal = metasConUrgencia.Sum(m => m.Urgencia);
                if (urgenciaTotal == 0) break;

                decimal dineroGastadoEnEstaVuelta = 0;

                // 3. Repartimos
                foreach (var item in metasConUrgencia)
                {
                    decimal proporcion = item.Urgencia / urgenciaTotal;
                    decimal montoCalculado = Math.Round(dineroSobrante * proporcion, 2);

                    decimal faltanteReal = item.Meta.MontoObjetivo - item.Meta.MontoActual - resultado[item.Meta.Id];
                    decimal asignarAhora = Math.Min(faltanteReal, montoCalculado);

                    if (asignarAhora > 0)
                    {
                        resultado[item.Meta.Id] += asignarAhora;
                        dineroGastadoEnEstaVuelta += asignarAhora;
                    }
                }

                dineroSobrante -= dineroGastadoEnEstaVuelta;
                if (dineroGastadoEnEstaVuelta <= 0) break;

                // 4. Limpiamos la mesa para la siguiente vuelta
                metasActivas = metasActivas.Where(m => (m.MontoObjetivo - m.MontoActual - resultado[m.Id]) > 0).ToList();
            }

            return resultado;
        }
    }
}
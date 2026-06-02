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
            decimal dineroSobrante = excedente;

            // 1. Filtramos las metas que aún necesitan dinero e inicializamos su cuenta en $0
            var metasActivas = metas.Where(m => m.PorcentajeCompletado < 100).ToList();
            
            foreach (var m in metasActivas)
            {
                resultado[m.Id] = 0;
            }

            // 2. Mientras haya dinero y metas por llenar...
            while (dineroSobrante > 0 && metasActivas.Any())
            {
                // Calculamos el peso total SOLO de las metas que siguen en la mesa
                decimal pesoTotal = metasActivas.Sum(m => m.Prioridad);
                if (pesoTotal == 0) break;

                decimal dineroGastadoEnEstaVuelta = 0;

                // 3. Damos una vuelta por la mesa para repartir proporcionalmente
                foreach (var meta in metasActivas.ToList())
                {
                    // Calculamos qué porcentaje le toca a esta meta
                    decimal proporcion = meta.Prioridad / pesoTotal;
                    
                    // Calculamos cuánto dinero es ese porcentaje (sobre el dinero que tenemos en la mano AHORA)
                    decimal montoCalculado = Math.Round(dineroSobrante * proporcion, 2);

                    // Calculamos cuánto le falta REALMENTE (restando lo que ya le dimos en vueltas anteriores)
                    decimal faltanteReal = meta.MontoObjetivo - meta.MontoActual - resultado[meta.Id];
                    
                    // EL SEGURO: Elegimos el más pequeño entre lo que calculamos y lo que de verdad necesita
                    decimal asignarAhora = Math.Min(faltanteReal, montoCalculado);

                    if (asignarAhora > 0)
                    {
                        resultado[meta.Id] += asignarAhora;
                        dineroGastadoEnEstaVuelta += asignarAhora;
                    }
                }

                // Restamos del fajo principal lo que acabamos de repartir en esta vuelta
                dineroSobrante -= dineroGastadoEnEstaVuelta;

                // Seguro contra ciclos infinitos (si por redondeo no se pudo asignar nada más)
                if (dineroGastadoEnEstaVuelta <= 0) break;

                // 4. Actualizamos la lista: sacamos de la mesa las metas que ya se llenaron en esta vuelta
                metasActivas = metasActivas
                    .Where(m => (m.MontoObjetivo - m.MontoActual - resultado[m.Id]) > 0)
                    .ToList();
            }

            return resultado;
        }
    }
}
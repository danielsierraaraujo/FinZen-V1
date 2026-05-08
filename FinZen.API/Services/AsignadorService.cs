using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Services
{
    public class AsignadorService
    {
        private IEstrategiaAsignacion _estrategia;

        public AsignadorService(IEstrategiaAsignacion estrategia)
        {
            _estrategia = estrategia;
        }

        public void CambiarEstrategia(IEstrategiaAsignacion nuevaEstrategia)
        {
            _estrategia = nuevaEstrategia;
        }

        public string EstrategiaActual => _estrategia.Nombre;

        public Dictionary<int, decimal> Asignar(List<Meta> metas, decimal excedente)
        {
            return _estrategia.Asignar(metas, excedente);
        }
    }
}
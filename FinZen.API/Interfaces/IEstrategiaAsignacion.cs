using FinZen.API.Models;

namespace FinZen.API.Interfaces
{
    public interface IEstrategiaAsignacion
    {
        string Nombre { get; }
        Dictionary<int, decimal> Asignar(List<Meta> metas, decimal excedente);
    }
}
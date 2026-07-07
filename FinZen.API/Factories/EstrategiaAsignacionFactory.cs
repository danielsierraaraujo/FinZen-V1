using Microsoft.Extensions.DependencyInjection;
using FinZen.API.Interfaces;

namespace FinZen.API.Factories
{
    // Resuelve la estrategia concreta a partir de servicios registrados con clave (keyed DI).
    // Agregar una nueva estrategia solo requiere registrarla en Program.cs: no exige tocar
    // este Factory ni el MetaController (Open/Closed Principle).
    public class EstrategiaAsignacionFactory : IEstrategiaAsignacionFactory
    {
        private const string EstrategiaPorDefecto = "prioridad";

        private readonly IServiceProvider _serviceProvider;

        public EstrategiaAsignacionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEstrategiaAsignacion Crear(string nombreEstrategia)
        {
            var clave = string.IsNullOrWhiteSpace(nombreEstrategia) ? EstrategiaPorDefecto : nombreEstrategia;

            return _serviceProvider.GetKeyedService<IEstrategiaAsignacion>(clave)
                ?? _serviceProvider.GetRequiredKeyedService<IEstrategiaAsignacion>(EstrategiaPorDefecto);
        }
    }
}

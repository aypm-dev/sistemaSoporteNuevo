using Abstracciones.Comunes;

namespace Abstracciones.Agentes;

/// <summary>Contrato de la lógica de negocio de clientes. El API solo conoce esta interfaz.</summary>
public interface IAgenteFlujo
{
    Task<ResultadoPaginado<AgenteDto>> ListarAsync(AgenteFiltro filtro, CancellationToken ct);
    Task<AgenteDto> ObtenerAsync(int id, CancellationToken ct);
    Task<AgenteDto> CrearAsync(AgenteCrearDto dto, CancellationToken ct);
    Task ActualizarAsync(int id, AgenteActualizarDto dto, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}

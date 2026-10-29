using Abstracciones.Agentes;
using Abstracciones.Clientes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;

namespace Flujo.Agentes;

public class AgenteFlujo : IAgenteFlujo
{
    private readonly IAgenteDA _da;

    public AgenteFlujo(IAgenteDA da, TimeProvider reloj)
    {
        _da = da;
    }

    public async Task<ResultadoPaginado<AgenteDto>> ListarAsync(AgenteFiltro filtro, CancellationToken ct)
    {
        var pagina = await _da.BuscarAsync(filtro, ct);
        return new ResultadoPaginado<AgenteDto>
        {
            Items = pagina.Items.Select(AgenteMapeador.ADto).ToList(),
            Total = pagina.Total,
            Pagina = pagina.Pagina,
            Tamano = pagina.Tamano
        };
    }

    public async Task<AgenteDto> ObtenerAsync(int id, CancellationToken ct)
    {
        var agente = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Agente", id);
        return AgenteMapeador.ADto(agente);
    }

    public async Task<AgenteDto> CrearAsync(AgenteCrearDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        if (await _da.ObtenerPorCorreoAsync(correo, ct) is not null)
            throw new ConflictoException($"Ya existe un agente con el correo '{correo}'.");

        var agente = new Agente
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            Telefono = dto.Telefono.Trim(),
            CapacidadMaxima = dto.CapacidadMaxima,
        };

        await _da.AgregarAsync(agente, ct);
        return AgenteMapeador.ADto(agente);
    }

    public async Task ActualizarAsync(int id, AgenteActualizarDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Agente", id);

        // El correo puede repetirse solo si pertenece al MISMO cliente que se está editando.
        var otro = await _da.ObtenerPorCorreoAsync(correo, ct);
        if (otro is not null && otro.Id != id)
            throw new ConflictoException($"Ya existe otro agente con el correo '{correo}'.");

        var actualizado = await _da.ActualizarAsync(new Agente
        {
            Id = id,
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            Telefono = dto.Telefono.Trim(),
            CapacidadMaxima = dto.CapacidadMaxima,
            Activo = dto.Activo ?? true
        }, ct);

        if (!actualizado) throw new NoEncontradoException("Cliente", id);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Cliente", id);

        if (await _da.TieneTicketsAsync(id, ct))
            throw new ConflictoException("El cliente tiene tickets asociados y no se puede eliminar.");

        if (!await _da.EliminarAsync(id, ct))
            throw new NoEncontradoException("Cliente", id);
    }

    // UNA sola regla de normalización para TODOS los caminos de escritura.
    private static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();
}

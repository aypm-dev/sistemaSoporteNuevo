using Abstracciones.Agentes;

namespace Flujo.Agentes;

/// <summary>
/// Mapeo manual entidad -> DTO. Se hace a mano a propósito: es corto, explícito, se puede depurar
/// y no depende de librerías (AutoMapper v15 en adelante tiene licencia comercial).
/// </summary>
internal static class AgenteMapeador
{
    public static AgenteDto ADto(Agente a) =>
        new(a.Id, a.Nombre, a.Correo, a.Telefono, a.CapacidadMaxima, a.Activo, a.FechaIngreso);
}

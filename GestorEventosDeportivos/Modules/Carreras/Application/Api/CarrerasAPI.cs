using System.Net;
using GestorEventosDeportivos.Modules.Carreras.Application.Services;
using GestorEventosDeportivos.Modules.Carreras.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/carreras")]
public class CarrerasAPI : ControllerBase
{
    private readonly ICarreraService _carreraService;

    public CarrerasAPI(ICarreraService carreraService)
    {
        _carreraService = carreraService;
    }

    [HttpPost("{carreraId}/pagos/{idPago}")]
    public async Task<string> NuevoPagoExterno([FromRoute] Guid carreraId, [FromRoute] string idPago)
    {
        try
        {
            Guid.TryParse(EncriptacionIds.DesencriptarString(idPago), out Guid participanteId);
            if (participanteId == Guid.Empty)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return "ID de participante inválido.";
            }

            bool resultado = await _carreraService.ActualizarEstadoPagoParticipacion(carreraId, participanteId, EstadoPago.Confirmado);
            if (!resultado)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return "No se pudo procesar el pago. Verifique los datos e intente nuevamente.";
            }
        }
        catch (NotFoundException ex)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return ex.Message;
        }
        catch (DomainRuleException ex)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return ex.Message;
        }
        catch (Exception)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return "Ocurrió un error inesperado, intente nuevamente más tarde.";
        }
        Response.StatusCode = StatusCodes.Status202Accepted;
        return "Pago procesado correctamente.";
    }
}
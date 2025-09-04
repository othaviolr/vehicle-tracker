using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Application.Interfaces;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public AlertsController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get all stolen vehicles for security alerts
    /// </summary>
    /// <returns>List of stolen vehicles</returns>
    [HttpGet("stolen-vehicles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VehicleResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleResponseDto>>>> GetStolenVehicles()
    {
        var stolenVehicles = await _vehicleService.GetByStatusAsync(VehicleStatus.Stolen);
        return Ok(ApiResponse<IEnumerable<VehicleResponseDto>>.SuccessResult(stolenVehicles));
    }

    /// <summary>
    /// Get vehicles in maintenance status
    /// </summary>
    /// <returns>List of vehicles in maintenance</returns>
    [HttpGet("maintenance-vehicles")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VehicleResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleResponseDto>>>> GetMaintenanceVehicles()
    {
        var maintenanceVehicles = await _vehicleService.GetByStatusAsync(VehicleStatus.Maintenance);
        return Ok(ApiResponse<IEnumerable<VehicleResponseDto>>.SuccessResult(maintenanceVehicles));
    }
}
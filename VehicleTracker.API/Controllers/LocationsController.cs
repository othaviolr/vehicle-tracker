using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Location;
using VehicleTracker.Application.Interfaces;
using FluentValidation;

namespace VehicleTracker.API.Controllers;

[ApiController]
[Route("api/vehicles/{vehicleId:guid}/[controller]")]
[Produces("application/json")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly IVehicleService _vehicleService;
    private readonly IValidator<CreateLocationDto> _createValidator;

    public LocationsController(
        ILocationService locationService,
        IVehicleService vehicleService,
        IValidator<CreateLocationDto> createValidator)
    {
        _locationService = locationService;
        _vehicleService = vehicleService;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Get vehicle location history with pagination and date filters
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 50, max: 100)</param>
    /// <param name="startDate">Filter start date (optional)</param>
    /// <param name="endDate">Filter end date (optional)</param>
    /// <returns>Paginated location history</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<LocationResponseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<PagedResponse<LocationResponseDto>>>> GetVehicleLocations(
        Guid vehicleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (!await _vehicleService.ExistsAsync(vehicleId))
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        var locations = await _locationService.GetPagedByVehicleIdAsync(vehicleId, page, pageSize, startDate, endDate);
        return Ok(ApiResponse<PagedResponse<LocationResponseDto>>.SuccessResult(locations));
    }

    /// <summary>
    /// Get vehicle's latest location
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <returns>Latest location or null if no locations recorded</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(ApiResponse<LocationResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<LocationResponseDto?>>> GetLatestLocation(Guid vehicleId)
    {
        if (!await _vehicleService.ExistsAsync(vehicleId))
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        var location = await _locationService.GetLatestByVehicleIdAsync(vehicleId);
        return Ok(ApiResponse<LocationResponseDto?>.SuccessResult(location));
    }

    /// <summary>
    /// Record a new location for the vehicle
    /// </summary>
    /// <param name="vehicleId">Vehicle ID</param>
    /// <param name="dto">Location data</param>
    /// <returns>Created location</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LocationResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<LocationResponseDto>>> CreateLocation(Guid vehicleId, CreateLocationDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        try
        {
            var location = await _locationService.CreateAsync(vehicleId, dto);
            return CreatedAtAction(
                nameof(GetLatestLocation),
                new { vehicleId },
                ApiResponse<LocationResponseDto>.SuccessResult(location, "Location recorded successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}
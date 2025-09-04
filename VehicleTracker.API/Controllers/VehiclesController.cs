using Microsoft.AspNetCore.Mvc;
using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Application.Interfaces;
using VehicleTracker.Domain.Enums;
using FluentValidation;

namespace VehicleTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IValidator<CreateVehicleDto> _createValidator;
    private readonly IValidator<UpdateVehicleStatusDto> _updateStatusValidator;

    public VehiclesController(
        IVehicleService vehicleService,
        IValidator<CreateVehicleDto> createValidator,
        IValidator<UpdateVehicleStatusDto> updateStatusValidator)
    {
        _vehicleService = vehicleService;
        _createValidator = createValidator;
        _updateStatusValidator = updateStatusValidator;
    }

    /// <summary>
    /// Get all vehicles with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <returns>Paginated list of vehicles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<VehicleResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResponse<VehicleResponseDto>>>> GetVehicles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var vehicles = await _vehicleService.GetPagedAsync(page, pageSize);
        return Ok(ApiResponse<PagedResponse<VehicleResponseDto>>.SuccessResult(vehicles));
    }

    /// <summary>
    /// Get vehicle by ID
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <returns>Vehicle details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<VehicleResponseDto>>> GetVehicle(Guid id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        return Ok(ApiResponse<VehicleResponseDto>.SuccessResult(vehicle));
    }

    /// <summary>
    /// Get vehicle by plate
    /// </summary>
    /// <param name="plate">Vehicle plate number</param>
    /// <returns>Vehicle details</returns>
    [HttpGet("plate/{plate}")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<VehicleResponseDto>>> GetVehicleByPlate(string plate)
    {
        var vehicle = await _vehicleService.GetByPlateAsync(plate);

        if (vehicle == null)
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        return Ok(ApiResponse<VehicleResponseDto>.SuccessResult(vehicle));
    }

    /// <summary>
    /// Get vehicles by status
    /// </summary>
    /// <param name="status">Vehicle status filter</param>
    /// <returns>List of vehicles with specified status</returns>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VehicleResponseDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleResponseDto>>>> GetVehiclesByStatus(VehicleStatus status)
    {
        var vehicles = await _vehicleService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<VehicleResponseDto>>.SuccessResult(vehicles));
    }

    /// <summary>
    /// Create a new vehicle
    /// </summary>
    /// <param name="dto">Vehicle creation data</param>
    /// <returns>Created vehicle</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<VehicleResponseDto>>> CreateVehicle(CreateVehicleDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        try
        {
            var vehicle = await _vehicleService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = vehicle.Id },
                ApiResponse<VehicleResponseDto>.SuccessResult(vehicle, "Vehicle created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// Update vehicle status
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <param name="dto">Status update data</param>
    /// <returns>Updated vehicle</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<VehicleResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<VehicleResponseDto>>> UpdateVehicleStatus(Guid id, UpdateVehicleStatusDto dto)
    {
        var validationResult = await _updateStatusValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
        }

        var vehicle = await _vehicleService.UpdateStatusAsync(id, dto);

        if (vehicle == null)
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        return Ok(ApiResponse<VehicleResponseDto>.SuccessResult(vehicle, "Vehicle status updated successfully"));
    }

    /// <summary>
    /// Delete vehicle (soft delete)
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVehicle(Guid id)
    {
        var success = await _vehicleService.DeleteAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResult("Vehicle not found"));
        }

        return Ok(ApiResponse<object>.SuccessResult(null, "Vehicle deleted successfully"));
    }
}
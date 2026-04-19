using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TC.Micro.Core;
using TC.Micro.Sample.Application.DTOs.Requests;
using TC.Micro.Sample.Application.DTOs.Responses;
using TC.Micro.Sample.Application.Services;

namespace TC.Micro.Sample.Api.Controllers;

/// <summary>
/// Sample 资源管理接口（示例 Controller，可直接改名或删除）
/// </summary>
[ApiController]
[Route("api/samples")]
[Authorize]
public class SampleController : ControllerBase
{
    private readonly ISampleService _sampleService;
    private readonly IUserContext _userContext;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        ISampleService sampleService,
        IUserContext userContext,
        ILogger<SampleController> logger)
    {
        _sampleService = sampleService;
        _userContext   = userContext;
        _logger        = logger;
    }

    /// <summary>分页查询 Sample 列表</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult<PagedResponse<SampleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int page_index = 1,
        [FromQuery] int page_size  = 20,
        CancellationToken ct = default)
    {
        var result = await _sampleService.GetListAsync(page_index, page_size, _userContext.TenantId, ct);
        return Ok(ServiceResult<PagedResponse<SampleResponse>>.Success(result));
    }

    /// <summary>根据 ID 查询 Sample</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResult<SampleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string id,
        CancellationToken ct = default)
    {
        var item = await _sampleService.GetByIdAsync(id, _userContext.TenantId, ct);
        if (item is null)
            return NotFound(ServiceResult.Fail("资源不存在", code: 1002));

        return Ok(ServiceResult<SampleResponse>.Success(item));
    }

    /// <summary>创建 Sample</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResult<SampleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSampleRequest request,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Creating sample, user_id={UserId}, tenant_id={TenantId}",
            _userContext.UserId, _userContext.TenantId);

        var result = await _sampleService.CreateAsync(request, _userContext, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ServiceResult<SampleResponse>.Success(result));
    }

    /// <summary>更新 Sample</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ServiceResult<SampleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string id,
        [FromBody] UpdateSampleRequest request,
        CancellationToken ct = default)
    {
        var result = await _sampleService.UpdateAsync(id, request, _userContext, ct);
        if (result is null)
            return NotFound(ServiceResult.Fail("资源不存在", code: 1002));

        return Ok(ServiceResult<SampleResponse>.Success(result));
    }

    /// <summary>删除 Sample</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] string id,
        CancellationToken ct = default)
    {
        var deleted = await _sampleService.DeleteAsync(id, _userContext.TenantId, ct);
        if (!deleted)
            return NotFound(ServiceResult.Fail("资源不存在", code: 1002));

        return Ok(ServiceResult.Success());
    }
}

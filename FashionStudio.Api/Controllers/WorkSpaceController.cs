using FashionStudio.Api.Services;
using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using FashionStudio.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;
using System.Threading;

namespace FashionStudio.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workspace")]
public class WorkSpaceController : BaseController
{
    private readonly IWorkSpaceService _workSpaceService;
    public WorkSpaceController(IWorkSpaceService workSpaceService)
    {
        _workSpaceService = workSpaceService;
    }

    [HttpPost("create/{ownerId}")]
    public async Task<IActionResult> CreateWorkSpaceAsync(
        int ownerId,
        [FromBody] WorkSpaceRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            var createdWorkSpace = await _workSpaceService
                .CreateWorkSpaceAsync(request,
                GetCurrentUserId() ?? throw new InvalidOperationException("User ID not found"),
                cancellationToken);
            return Ok(createdWorkSpace);
            //return CreatedAtAction(
            //    nameof(GetWorkSpacesByIdAsync), 
            //    new { id = createdWorkSpace.Id }, createdWorkSpace);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("workspaces/{id}")]
    public async Task<IActionResult> GetWorkSpacesByIdAsync(int id)
    {
        try
        {
            var workSpaces = await _workSpaceService.GetWorkSpaceByIdAsync(id);
            return Ok(workSpaces);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
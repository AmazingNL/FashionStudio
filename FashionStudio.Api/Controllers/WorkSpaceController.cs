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
    private readonly IWorkSpaceInvitation _workSpaceInvitationService;
    public WorkSpaceController(IWorkSpaceService workSpaceService, IWorkSpaceInvitation workSpaceInvitationService)
    {
        _workSpaceService = workSpaceService;
        _workSpaceInvitationService = workSpaceInvitationService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateWorkSpaceAsync(
        [FromBody] WorkSpaceRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId() ?? throw new InvalidOperationException("User ID not found");

            var createdWorkSpace = await _workSpaceService
                .CreateWorkSpaceAsync(
                request,
                ownerId,
                cancellationToken);
            return CreatedAtAction(
                nameof(GetWorkSpacesByIdAsync),
                new { id = createdWorkSpace.Id }, createdWorkSpace);
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

    [HttpGet("list")]
    public async Task<IActionResult> GetAllWorkSpacesAsync()
    {
        try
        {
            var workSpaces = await _workSpaceService.GetAllWorkSpacesAsync();
            return Ok(workSpaces);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteUserToWorkSpaceAsync(
        [FromBody] InvitationRequestDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = GetCurrentUserId() ?? throw new InvalidOperationException("User ID not found");
            var invitationResponse = await _workSpaceInvitationService
                .SendInvitationAsync(
                request,
                ownerId,
                cancellationToken);
            return Ok(invitationResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("respond-invitation")]
    [AllowAnonymous]
    public async Task<IActionResult> RespondToInvitationAsync(
    [FromBody] AcceptInvitationDTO request,
    CancellationToken cancellationToken)
    {
        try
        {
            var response = await _workSpaceInvitationService
                .RespondToInvitationAsync(
                request.InvitationCode,
                request.Status,
                cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}

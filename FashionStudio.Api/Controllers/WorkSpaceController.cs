using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

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
        var ownerId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");

        var createdWorkSpace = await _workSpaceService
            .CreateWorkSpaceAsync(
            request,
            ownerId,
            cancellationToken);
        return CreatedAtAction(
            nameof(GetWorkSpacesByIdAsync),
            new { id = createdWorkSpace.Id }, createdWorkSpace);
    }

    [HttpGet("workspaces/{id}")]
    public async Task<IActionResult> GetWorkSpacesByIdAsync(int id)
    {
        var workSpaces = await _workSpaceService.GetWorkSpaceByIdAsync(id);
        return Ok(workSpaces);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllWorkSpacesAsync()
    {
        var workSpaces = await _workSpaceService.GetAllWorkSpacesAsync();
        return Ok(workSpaces);
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteUserToWorkSpaceAsync(
        [FromBody] InvitationRequestDTO request,
        CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId() ?? throw new InvalidOperationException("User ID not found");
        var invitationResponse = await _workSpaceInvitationService
            .SendInvitationAsync(
            request,
            ownerId,
            cancellationToken);
        return Ok(invitationResponse);
    }

    [HttpPost("respond-invitation")]
    [AllowAnonymous]
    public async Task<IActionResult> RespondToInvitationAsync(
    [FromBody] AcceptInvitationDTO request,
    CancellationToken cancellationToken)
    {
        var response = await _workSpaceInvitationService
            .RespondToInvitationAsync(
            request.InvitationCode,
            request.Status,
            cancellationToken);
        return Ok(response);
    }
}

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
    public WorkSpaceController(
        IWorkSpaceService workSpaceService,
        IWorkSpaceInvitation workSpaceInvitationService,
        IActivityLogService? activityLogService)
        : base(activityLogService)
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
        await LogActivityAsync("WorkSpace", createdWorkSpace.Id, "Created");
        return CreatedAtAction(
            nameof(GetWorkSpaceById),
            new { id = createdWorkSpace.Id }, createdWorkSpace);
    }

    [HttpGet("workspaces/{id}")]
    public async Task<IActionResult> GetWorkSpaceById(int id)
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
        await LogActivityAsync("WorkSpace", request.WorkSpaceId, "InvitationSent");
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

    [HttpPatch("{workSpaceId}/members/{memberUserId}")]
    public async Task<IActionResult> UpdateMemberRole(
        int workSpaceId,
        int memberUserId,
        [FromBody] UpdateMemberRoleDTO request,
        CancellationToken cancellationToken)
    {
        var actingUserId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
        var member = await _workSpaceService.UpdateMemberRoleAsync(workSpaceId, memberUserId, request.Role, actingUserId, cancellationToken);
        await LogActivityAsync("WorkSpace", workSpaceId, $"MemberRoleChanged:{memberUserId}");
        return Ok(member);
    }

    [HttpDelete("{workSpaceId}/members/{memberUserId}")]
    public async Task<IActionResult> RemoveMember(
        int workSpaceId,
        int memberUserId,
        CancellationToken cancellationToken)
    {
        var actingUserId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
        await _workSpaceService.RemoveMemberAsync(workSpaceId, memberUserId, actingUserId, cancellationToken);
        await LogActivityAsync("WorkSpace", workSpaceId, $"MemberRemoved:{memberUserId}");
        return NoContent();
    }
}

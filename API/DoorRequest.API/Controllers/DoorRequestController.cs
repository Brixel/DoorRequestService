﻿using DoorRequest.API.Config;
using DoorRequest.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DoorRequest.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DoorRequestController : ControllerBase
{
    private readonly IDoorService _doorService;
    private readonly LockConfiguration _lockConfiguration;

    public DoorRequestController(IDoorService doorService, IOptions<LockConfiguration> lockConfiguration)
    {
        _doorService = doorService ?? throw new System.ArgumentNullException(nameof(doorService));
        _lockConfiguration = lockConfiguration.Value;
    }

    [HttpPost("open")]
    [Authorize(Roles = Authorization.Roles.TwentyFourSevenAccess)]
    public async Task<bool> OpenDoorRequest()
    {
        return await _doorService.OpenDoor();
    }

    [HttpGet("code")]
    [Authorize(Roles = Authorization.Roles.KeyVaultCodeAccess)]
    public int GetLockCode()
    {
        return _lockConfiguration.Code;
    }
}
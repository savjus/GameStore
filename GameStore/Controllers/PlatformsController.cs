using GameStore.Models.Dtos;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
[Route("platforms")]
public class PlatformsController(IGameService gameService, IPlatformService platformService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IPlatformService _platformService = platformService;

    [HttpGet("{id:guid}/games")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<GameResponseDto>>> GetGamesByPlatform(Guid id)
    {
        var result = await _gameService.GetGamesByPlatformAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{id:guid}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<PlatformResponseDto>> GetPlatformById(Guid id)
    {
        var result = await _platformService.GetPlatformByIdAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<PlatformResponseDto>>> GetAllPlatforms()
    {
        var result = await _platformService.GetAllPlatformsAsync();
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<PlatformResponseDto>> AddPlatform([FromBody] AddPlatformRequest request)
    {
        var result = await _platformService.AddPlatformAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetPlatformById), new { id = result.Value!.Id }, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPut]
    public async Task<ActionResult<PlatformResponseDto>> UpdatePlatform([FromBody] UpdatePlatformRequest request)
    {
        var result = await _platformService.UpdatePlatformAsync(request);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<PlatformResponseDto>> DeletePlatform(Guid id)
    {
        var result = await _platformService.DeletePlatformAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }
}

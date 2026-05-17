using GameStore.Models.Dtos;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
[Route("games")]
public class GamesController(IGameService gameService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;

    [HttpGet("{key}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<GameResponseDto>> GetGameByKey(string key)
    {
        var result = await _gameService.GetGameByKeyAsync(key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("find/{id:guid}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<GameResponseDto>> GetGameById(Guid id)
    {
        var result = await _gameService.GetGameByIdAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<GameResponseDto>>> GetAllGames()
    {
        var result = await _gameService.GetAllGamesAsync();
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<GameResponseDto>> AddGame([FromBody] AddGameRequest request)
    {
        var result = await _gameService.AddGameAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetGameById), new { id = result.Value!.Id }, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPut]
    public async Task<ActionResult<GameResponseDto>> UpdateGame([FromBody] UpdateGameRequest request)
    {
        var result = await _gameService.UpdateGameAsync(request);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult<GameResponseDto>> DeleteGame(string key)
    {
        var result = await _gameService.DeleteGameAsync(key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{key}/file")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> DownloadGame(string key)
    {
        var result = await _gameService.GetGameFileAsync(key);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, result.Error);
        }

        var file = result.Value!;
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("{key}/genres")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<GenreResponseDto>>> GetGenresByGameKey(string key)
    {
        var result = await _gameService.GetGenresByGameKeyAsync(key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{key}/platforms")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<PlatformResponseDto>>> GetPlatformsByGameKey(string key)
    {
        var result = await _gameService.GetPlatformsByGameKeyAsync(key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }
}
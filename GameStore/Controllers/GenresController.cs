using GameStore.Models.Dtos;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
[Route("genres")]
public class GenresController(IGameService gameService, IGenreService genreService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IGenreService _genreService = genreService;

    [HttpGet("{id:guid}/games")]
    public async Task<ActionResult<List<GameResponseDto>>> GetGamesByGenre(Guid id)
    {
        var result = await _gameService.GetGamesByGenreAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GenreResponseDto>> GetGenreById(Guid id)
    {
        var result = await _genreService.GetGenreByIdAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreResponseDto>>> GetAllGenres()
    {
        var result = await _genreService.GetAllGenresAsync();
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{id:guid}/genres")]
    public async Task<ActionResult<List<GenreResponseDto>>> GetGenresByParentId(Guid id)
    {
        var result = await _genreService.GetGenresByParentIdAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<GenreResponseDto>> AddGenre([FromBody] AddGenreRequest request)
    {
        var result = await _genreService.AddGenreAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetGenreById), new { id = result.Value!.Id }, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPut]
    public async Task<ActionResult<GenreResponseDto>> UpdateGenre([FromBody] UpdateGenreRequest request)
    {
        var result = await _genreService.UpdateGenreAsync(request);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<GenreResponseDto>> DeleteGenre(Guid id)
    {
        var result = await _genreService.DeleteGenreAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }
}

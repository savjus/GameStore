using GameStore.Models;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    private readonly ICommentService _commentService = commentService;

    [HttpPost("games/{key}/comments")]
    public async Task<IActionResult> AddComment(string key, [FromBody] CommentRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commentService.AddCommentAsync(key, dto);
        return result.IsSuccess ? Ok() : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("games/{key}/comments")]
    public async Task<IActionResult> GetComments(string key)
    {
        var result = await _commentService.GetCommentsByGameKeyAsync(key);
        return result.IsSuccess ? Ok(result.Value) : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete("games/{key}/comments/{id:guid}")]
    public async Task<IActionResult> DeleteComment(string key, Guid id)
    {
        var result = await _commentService.DeleteCommentAsync(key, id);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("comments/ban/durations")]
    public IActionResult GetBanDurations()
    {
        return Ok(_commentService.GetBanDurations());
    }

    [HttpPost("comments/ban")]
    public async Task<IActionResult> BanUser([FromBody] BanRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _commentService.BanUserAsync(dto);
        return result.IsSuccess ? Ok() : StatusCode(result.StatusCode, result.Error);
    }
}
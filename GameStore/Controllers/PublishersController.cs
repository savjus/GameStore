using GameStore.Models.Dtos;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
[Route("publishers")]
public class PublishersController(IPublisherService publisherService) : ControllerBase
{
    private readonly IPublisherService _publisherService = publisherService;

    [HttpGet]
    public async Task<ActionResult<List<PublisherResponseDto>>> GetAllPublishers()
    {
        var result = await _publisherService.GetAllPublishersAsync();
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{companyName}")]
    public async Task<ActionResult<PublisherResponseDto>> GetPublisherByCompanyName(string companyName)
    {
        var result = await _publisherService.GetPublisherByCompanyNameAsync(companyName);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{companyName}/games")]
    public async Task<ActionResult<List<GameResponseDto>>> GetGamesByPublisher(string companyName)
    {
        var result = await _publisherService.GetGamesByPublisherNameAsync(companyName);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<PublisherResponseDto>> AddPublisher([FromBody] AddPublisherRequest request)
    {
        var result = await _publisherService.AddPublisherAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetPublisherByCompanyName), new { companyName = result.Value!.CompanyName }, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPut]
    public async Task<ActionResult<PublisherResponseDto>> UpdatePublisher([FromBody] UpdatePublisherRequest request)
    {
        var result = await _publisherService.UpdatePublisherAsync(request);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<PublisherResponseDto>> DeletePublisher(Guid id)
    {
        var result = await _publisherService.DeletePublisherAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("/games/{key}/publisher")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<PublisherResponseDto>> GetPublisherByGameKey(string key)
    {
        var result = await _publisherService.GetPublisherByGameKeyAsync(key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }
}

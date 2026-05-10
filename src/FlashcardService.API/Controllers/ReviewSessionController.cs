using Microsoft.AspNetCore.Mvc;

namespace FlashcardService.API.Controllers;

[ApiController]
[Route("review-sessions")]
public sealed class ReviewSessionController : ControllerBase
{
    [HttpPost]
    public async Task CreateReviewSession()
    {
        throw new NotImplementedException();
    }

    [HttpPut("{sessionId:guid}")]
    public async Task UpdateReviewSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task GetAllReviewSessions()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{sessionId:guid}")]
    public async Task GetReviewSession(Guid sessionId)
    {
        throw new NotImplementedException();
    }
    
    // To keep the API RESTful, I decided to think of a card request as a "resource" that a client can POST.
    // Would be strange to do it as GET because a GET response should only depend on the query params.
    [HttpPost("{sessionId:guid}/card-requests")]
    public async Task AskForCard(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    // TODO: resource structure (PUT or POST?)
    // Right now, seeing the "grading" as "updating an existing card request" (to "finish" it).
    [HttpPut("{sessionId:guid}/card-requests/{requestId:guid}")]
    public async Task GradeCard(Guid sessionId, Guid requestId)
    {
        throw new NotImplementedException();
    }
}
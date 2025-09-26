using System.Text.RegularExpressions;
using Contracts;
using SearchService.Models;
using Typesense;

namespace SearchService.MessageHandlers;

public class QuestionCreatedHandler(ITypesenseClient client)
{
    public async Task HandleAsync(QuestionCreated message)
    {
        var createdAt = new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds();
        var document = SearchQuestion.Create(message.QuestionId, message.Title, message.Content, message.Tags.ToArray());
        document.CreatedAt = createdAt;
        await client.CreateDocument("questions", document);
    }
}
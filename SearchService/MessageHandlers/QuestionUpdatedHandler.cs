using Contracts;
using SearchService.Models;
using Typesense;

namespace SearchService.MessageHandlers;

public class QuestionUpdatedHandler(ITypesenseClient client)
{
    public async Task HandleAsync(QuestionUpdated message)
    {
        await client.UpdateDocument("questions", message.QuestionId, new
        {
            Title=message.Title,
            Content = Helpers.Html.Sanitize(message.Content),
            Tags = message.Tags,
        });
    }
    
}
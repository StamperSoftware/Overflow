using Typesense;

namespace SearchService.Data;

public static class SearchInitializer
{
    public static async Task EnsureIndexExists(ITypesenseClient client)
    {
        const string schemeName = "questions";
        try
        {
            await client.RetrieveCollection(schemeName);
            Console.WriteLine($"Collection {schemeName} has been created already");
            return;
        }
        catch
        {
            Console.WriteLine($"Collection {schemeName} has not been created yet");
        }

        var schema = new Schema(schemeName, new List<Field>
        {
            new("id", FieldType.String),
            new("title", FieldType.String),
            new("content", FieldType.String),
            new("tags", FieldType.StringArray),
            new("createdAt", FieldType.Int64),
            new("hasAcceptedAnswer", FieldType.Bool),
            new("answerCount", FieldType.Int32),
        })
        {
            DefaultSortingField = "createdAt"
        };

        await client.CreateCollection(schema);
        Console.WriteLine($"Collection {schemeName} has been created");
        
    } 
}
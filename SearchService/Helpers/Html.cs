using System.Text.RegularExpressions;

namespace SearchService.Helpers;

public static class Html
{
    public static string Sanitize(string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty);
    }
    
}

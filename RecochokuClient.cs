using System.Text.RegularExpressions;
using System.Web;

internal class RecochokuClient
{
    private static readonly HttpClient HttpClient = new();

    public async Task<RecochokuImage?> GetProductImageAsync(RecochokuUrl url, int width, int height)
    {
        // Get page.
        var pageContent = await GetProductPageContentAsync(url.ToString());
        if (string.IsNullOrEmpty(pageContent))
        {
            Console.WriteLine($"Failed to get page. (Url='{url}')");
            return null;
        }

        // Get product cover image.
        var productImageUrl = GetProductImageUrl(pageContent);
        if (string.IsNullOrEmpty(productImageUrl))
        {
            Console.WriteLine($"Failed to get product image url.");
            return null;
        }

        var imageContent = await GetProductImageAsync(productImageUrl, width, height);
        if (imageContent == null)
        {
            Console.WriteLine($"Failed to get product image.");
            return null;
        }

        var productTitle = GetProductTitle(pageContent);

        return new(productTitle, imageContent);
    }

    private async Task<string?> GetProductPageContentAsync(string url)
    {
        Console.WriteLine($"Requesting page.");
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to get responce. (Code={response.StatusCode})");
            return null;
        }
        
        return await response.Content.ReadAsStringAsync();
    }

    private string GetProductTitle(string pageContent)
    {
        var lines = pageContent.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        foreach (var line in lines)
        {
            // Find target line.
            if (!line.StartsWith("<h1 "))
                continue;

            if (!line.Contains("class=\"c-product-main-detail__title\""))
                continue;

            // Get product title.
            var regexMatch = Regex.Match(line, @">(?<title>[^<]+)</h1>$");
            if (!regexMatch.Success)
                continue;

            return regexMatch.Groups["title"].Value;
        }

        return string.Empty;
    }

    private string GetProductImageUrl(string pageContent)
    {
        var lines = pageContent.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        foreach (var line in lines)
        {
            // Find target line.
            if (!line.StartsWith("<img "))
                continue;

            if (!line.Contains("class=\"c-product-main-info__photo-image\""))
                continue;

            // Get product image url.
            var regexMatch = Regex.Match(line, @"background-image: url\((?<imageUrl>[^\)]+)");
            if (!regexMatch.Success)
                continue;

            return regexMatch.Groups["imageUrl"].Value;
        }

        return string.Empty;
    }

    private async Task<byte[]?> GetProductImageAsync(string url, int width, int height)
    {
        Console.WriteLine($"Requesting image.");

        var urlParts = url.Split("?");
        var baseUrl = urlParts[0];
        var parameterString = urlParts[1];

        // Override parameter string.
        var parameters = HttpUtility.ParseQueryString(parameterString);
        parameters["FFw"] = width.ToString();
        parameters["FFh"] = height.ToString();

        var response = await HttpClient.GetAsync($"{baseUrl}?{parameters}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to get responce. (Code={response.StatusCode})");
            return null;
        }
        
        return await response.Content.ReadAsByteArrayAsync();
    }
}
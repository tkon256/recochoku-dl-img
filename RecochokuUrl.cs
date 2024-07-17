internal record RecochokuUrl(string ProductType, string ProductId)
{
    private const string BaseUrl = "https://recochoku.jp";

    public static RecochokuUrl Parse(string url)
    {
        return RecochokuUrlParser.Parse(url);
    }

    public override string ToString()
    {
        return string.Join("/", BaseUrl, ProductType, ProductId);
    }
}

internal class RecochokuUrlParser
{
    public static RecochokuUrl Parse(string url)
    {
        var urlParts = url.Split("/");
        var productId = urlParts[urlParts.Length - 1];
        var productType = productId.Substring(0, 1).ToLower() switch {
            "a" => "album",
            "s" => "song",
            _ => string.Empty,
        };

        if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(productType))
            throw new ArgumentException($"'{url}' is invalid url.");

        return new(productType, productId);
    }
}
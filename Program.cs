var inputUrlOrProductId = args.FirstOrDefault();
while (string.IsNullOrEmpty(inputUrlOrProductId))
{
    Console.Write("Put recochoku product url or id: ");
    inputUrlOrProductId = Console.ReadLine();
}

var imageWidth = ArgsHelper.GetIntValue(args, "-w", 1000);
var imageHeight = ArgsHelper.GetIntValue(args, "-h", 1000);
var outPath = ArgsHelper.GetStringValue(args, "-o", "./");

// Parse recochoku url.
var url = RecochokuUrl.Parse(inputUrlOrProductId);

var client = new RecochokuClient();
var recochokuImage = await client.GetProductImageAsync(url, imageWidth, imageHeight);
if (recochokuImage == null)
{
    Console.WriteLine("Failed to download iamge.");
    return;
}

var outFilePath = GetOutFilePath(outPath, recochokuImage);

Console.WriteLine($"Saving '{outFilePath}'.");
await File.WriteAllBytesAsync(outFilePath, recochokuImage.Content);

string GetOutFilePath(string path, RecochokuImage image)
{
    // Combine path.
    if (Directory.Exists(path) || path.EndsWith("/"))
    {
        var outFileName = image.Title;
        if (string.IsNullOrEmpty(outFileName))
        {
            outFileName = "front";
        }
        outFileName = $"{outFileName}.{image.FileFormat.ToLower()}";

        return Path.Combine(path, outFileName);
    }

    return path;
}
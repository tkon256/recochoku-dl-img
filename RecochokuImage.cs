internal class RecochokuImage
{
    public RecochokuImage(string title, byte[] content)
    {
        Title = title;
        Content = content;

        FileFormat = GetFileFormat();
    }

    public string Title { get; }
    public byte[] Content { get; }
    public string FileFormat { get; }

    private string GetFileFormat()
    {
        var bit = Content[0].ToString("X2");
        return bit switch
        {
            "FF" => "JPG",
            "89" => "PNG",
            _ => string.Empty,
        };
    }
}
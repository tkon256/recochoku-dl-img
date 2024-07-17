internal static class ArgsHelper
{
    public static int GetIntValue(string[] args, string name, int defaultValue = default)
    {
        var value = defaultValue;

        var i = Array.IndexOf(args, name);
        if (0 <= i && i + 2 <= args.Length)
        {
            var specified = args[i + 1];
            if (!int.TryParse(specified, out value))
                throw new ArgumentException($"'{specified}' is invalid value for '{name}' option.");
        }

        return value;
    }

    public static string GetStringValue(string[] args, string name, string defaultValue)
    {
        var value = defaultValue;

        var i = Array.IndexOf(args, name);
        if (0 <= i && i + 2 <= args.Length)
        {
            value = args[i + 1];
        }

        return value;
    }
}
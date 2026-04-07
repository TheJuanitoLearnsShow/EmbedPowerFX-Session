using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace MinimalSamples.FxExtentions;

public class AsyncProductTagLineSearch : ReflectionFunction
{
    public AsyncProductTagLineSearch()
        : base("GetProductTagLine", FormulaType.String, [ FormulaType.String ])
    {
    }

    /// <summary>
    /// cancellationToken is required because the PowerFX interpreter needs to pass it.
    /// </summary>
    /// <param name="productToSearch"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<FormulaValue> Execute(StringValue productToSearch, CancellationToken cancellationToken)
    {
        var productToSearchStr = productToSearch.Value;
        await foreach (var line in FileSearch.FindLinesContainingAsync("data/data.txt", productToSearchStr)
                           .WithCancellation(cancellationToken))
        {
            return FormulaValue.New(line);
        }
        return FormulaValue.New(string.Empty);
    }
}

public static class FileSearch
{
    public static async IAsyncEnumerable<string> FindLinesContainingAsync(
        string filePath,
        string search,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        using var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (line != null && line.Contains(search, comparison))
                yield return line;
        }
    }
}
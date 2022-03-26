using System.Net;
using System.Net.Sockets;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var max = 50;
var tasks = Enumerable.Range(1, max).Select(CreateTask);
var numbers = await Task.WhenAll(tasks);
var sortedNumbers = numbers.OrderBy(x => x).ToList();

if (sortedNumbers.Count != max)
{
    throw new Exception($"Wrong number of items: {sortedNumbers.Count}");
}

var a = sortedNumbers[max / 2];
var b = sortedNumbers[max / 2 + 1];
var median = (a + b) / 2;

Console.WriteLine($"Median: {median}");

async Task<int> CreateTask(int number)
{
    var response = await GetValidResponseAsync(number);
    var numbers = new string(response.Where(char.IsDigit).ToArray());
    var result = int.Parse(numbers);

    System.Console.WriteLine($"{number}: {result}");
    return result;
}

async Task<string> GetValidResponseAsync(int number)
{
    var result = string.Empty;
    var isValid = false;

    do
    {
        result = await SendNumberAsync(number);
        isValid = result != null && result.Contains("\n");
    } while (!isValid);

    return result;
}

async Task<string?> SendNumberAsync(int number)
{
    try
    {
        using var client = new TcpClient("88.212.241.115", 2013);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync($"{number}\n");
        await writer.FlushAsync();

        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();

        return result;
    }
    catch
    {
        return null;
    }
}

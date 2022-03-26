using System.Net.Sockets;

var tasks = Enumerable.Range(1, 2018).Select(GetNumberAsync).ToArray();
var numbers = await Task.WhenAll(tasks);
var median = CalculateMedian(numbers);

Console.WriteLine($"Median: {median}"); // 4925680.5

async Task<int> GetNumberAsync(int number)
{
    var response = await GetValidResponseAsync(number);
    var numberString = new string(response.Where(char.IsDigit).ToArray());
    return int.Parse(numberString);
}

async Task<string> GetValidResponseAsync(int number)
{
    while (true)
    {
        var result = await SendMessageAsync($"{number}\n");
        var ok = result != null && result.Contains("\n");
        if (ok)
        {
            return result;
        }
    }
}

async Task<string?> SendMessageAsync(string message)
{
    try
    {
        using var client = new TcpClient("88.212.241.115", 2013);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(message);
        await writer.FlushAsync();

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
    catch
    {
        return null;
    }
}

static double CalculateMedian(IEnumerable<int> list)
{
    var sorted = list.OrderBy(x => x).ToArray();
    var a = sorted[sorted.Length / 2 - 1];
    var b = sorted[sorted.Length / 2];
    
    return (a + b) / 2.0;
}
using System.Net.Sockets;

var tasks = Enumerable.Range(1, 2013).Select(CreateTask).ToArray();
var numbers = await Task.WhenAll(tasks);

if (numbers.Length != 2013)
{
    throw new Exception($"Wrong number of items: {numbers.Length}");
}

var median = CalculateMedian(numbers);

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
    while (true)
    {
        var response = await SendNumberAsync(number);
        var ok = response != null && response.Contains("\n");
        if (ok)
        {
            return response;
        }
    }
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

static int CalculateMedian(IEnumerable<int> list)
{
    var sorted = list.OrderBy(x => x).ToArray();
    var a = sorted[sorted.Length / 2];
    var b = sorted[sorted.Length / 2 + 1];
    var result = (a + b) / 2;

    return result;
}
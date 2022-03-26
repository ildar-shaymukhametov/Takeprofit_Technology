using System.Net;
using System.Net.Sockets;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

try
{
    var tasks = Enumerable.Range(1, 2018).Select(CreateTask);
    var result = await Task.WhenAll(tasks);
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
catch (Exception e)
{
    Console.WriteLine("Exception: {0}", e);
}

Console.WriteLine("Запрос завершен...");

async Task<int> CreateTask(int number)
{
    try
    {
        using var client = new TcpClient("88.212.241.115", 2013);
        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync($"{number}\n");
        await writer.FlushAsync();

        using var reader = new StreamReader(stream);
        var line = await reader.ReadToEndAsync();
        var numberString = new string(line?.Where(char.IsDigit).ToArray());

        int.TryParse(numberString, out int result);

        System.Console.WriteLine($"{number}: {result} ({line}) ({numberString})");
        return result;
    }
    catch (System.Exception ex)
    {
        System.Console.WriteLine($"{number}: {ex}");
        return 0;
    }
}

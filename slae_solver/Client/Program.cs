using Client;

using (Client.Client client = new Client.Client())
{
    var reader = new SlaeDataReader();
    try
    {

        var slaeData = reader.ReadSlaeData();
        Console.WriteLine();
        var x = client.SendRequestToSolve(slaeData);

        Console.WriteLine("Ответ от сервера: ");
        AnswerOutput(x);

        Console.WriteLine();

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}
Console.WriteLine("\nPress Enter to continue...");
Console.Read();

void AnswerOutput(float[] x)
{
    for (int i = 0; i < x.Length; i++)
    {
        Console.WriteLine(x[i]);
    }
}
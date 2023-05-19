using System.Net.Mime;

public static class Program
{
    static void Main()
    {
        List<int> testL = new List<int>()
        {
            1,2,3,4,5,6,7,8,9,10
        };

        testL.ForEach(i => Console.WriteLine(i));
        Console.WriteLine("Hello World!");
    }
}
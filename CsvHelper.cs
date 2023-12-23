using AirportTicketBookingSystem;
using static AirportTicketBookingSystem.Enums;
using System.Globalization;

public static class CsvHelper
{
    public static List<T> ReadFromCsv<T>(string filePath, Func<string[], T> createObject)
    {
        var lines = File.ReadAllLines(filePath);
        var objects = new List<T>();

        foreach (var line in lines)
        {
            var values = line.Split(',');

            if (values.Length > 0)
            {
                var obj = createObject(values);
                objects.Add(obj);
            }
        }

        return objects;
    }

    public static void WriteToCsv<T>(string filePath, IEnumerable<T> objects, Func<T, string> convertObjectToString)
    {
        var lines = new List<string> { "HeaderLine" }; // Replace "HeaderLine" with your actual header

        foreach (var obj in objects)
        {
            var line = convertObjectToString(obj);
            lines.Add(line);
        }

        File.WriteAllLines(filePath, lines);
    }
}

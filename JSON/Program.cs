using System;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

class JsonToXmlConverter
{
    public static XElement ConvertJsonToXml(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        return CreateXmlElement(document.RootElement);
    }

    private static XElement CreateXmlElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var composite = new XElement("Object");
                foreach (var property in element.EnumerateObject())
                {
                    composite.Add(new XElement(property.Name, CreateXmlElement(property.Value)));
                }
                return composite;

            case JsonValueKind.Array:
                var array = new XElement("Array");
                foreach (var item in element.EnumerateArray())
                {
                    array.Add(CreateXmlElement(item));
                }
                return array;

            case JsonValueKind.String:
                return new XElement("String", element.GetString());

            case JsonValueKind.Number:
                return new XElement("Number", element.GetDecimal());

            case JsonValueKind.True:
            case JsonValueKind.False:
                return new XElement("Boolean", element.GetBoolean());

            case JsonValueKind.Null:
                return new XElement("Null");

            default:
                throw new ArgumentException($"Unsupported JSON value kind: {element.ValueKind}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к JSON файлу:");
        string filePath = Console.ReadLine();

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            XElement xml = JsonToXmlConverter.ConvertJsonToXml(jsonContent);
            Console.WriteLine("XML представление JSON:");
            Console.WriteLine(xml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}

using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;

var enumerateFiles = Directory.EnumerateFiles(@"C:\WorkSpace\xxx\xxx\Strings\zh-Hans", "*.resw", SearchOption.AllDirectories);
var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

foreach (var file in enumerateFiles)
{
    var resw = XDocument.Load(file);
    var json = resw.Descendants("data").ToDictionary(x =>
    {
        var value = x.Attribute("name")!.Value;
        if (value.IndexOf('[') is var index and not -1)
        {
            var endIndex = value.IndexOf(']');
            value = value.Remove(index, endIndex - index + 1);
        }

        return value.Replace('.', '/');
    }, x => x.Element("value")!.Value);
    File.Delete(file);
    var newPath = Path.ChangeExtension(file, "resjson");
    var jsonText = JsonSerializer.Serialize(json, jsonSerializerOptions);
    File.WriteAllText(newPath, jsonText);
}
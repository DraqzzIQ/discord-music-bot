using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DMusicBot;
internal class Config
{
    internal class ConfigDto
    {
        public string BotToken;
        public string LL_Hostname;
        public int LL_Port;
        public string LL_Password;
        public ulong DebugGuildId;
    }

    public static ConfigDto Data = new();

    public static string FilePath = "config.json";

    public static void ReadConfiguration()
    {
        if (!File.Exists(FilePath)) File.WriteAllText(FilePath, JsonConvert.SerializeObject(Data));

        var fileData = File.ReadAllText(FilePath);

        try
        {
            JsonConvert.PopulateObject(fileData, Data);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading " + FilePath + "\n" + e.Message);
        }
    }

    public static void SaveConfig()
    {
        string output = JsonConvert.SerializeObject(Data);
        File.WriteAllText(FilePath, output);
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DMusicBot;
internal class Config
{
    private static string FilePath = "config.json";
    internal class ConfigDto
    {
        public string BotToken;
        public string LL_Hostname;
        public int LL_Port;
        public string LL_Password;
        public ulong DebugGuildId;
    }

    private static ConfigDto? _data = null;

    public static ConfigDto Data
    {
        get
        {
            if (_data is null)
                ReadConfiguration();

            return _data!;
        }
    }

    private static void ReadConfiguration()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(new ConfigDto()));
            Console.WriteLine($"Config file created at {FilePath}. Please fill in the necessary information and restart the bot.");
            Environment.Exit(1);
        }

        var fileData = File.ReadAllText(FilePath);

        try
        {
            _data = JsonConvert.DeserializeObject<ConfigDto>(fileData);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading " + FilePath + "\n" + e.Message);
            Environment.Exit(1);
        }
    }

    public static void SaveConfig()
    {
        string output = JsonConvert.SerializeObject(Data);
        File.WriteAllText(FilePath, output);
    }
}

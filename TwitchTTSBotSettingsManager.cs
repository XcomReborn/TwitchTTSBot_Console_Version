using Newtonsoft.Json;

[System.Serializable]

class TwitchTTSBotSettingsManager
{


    public TwitchTTSBotSettings settings = new TwitchTTSBotSettings();

    public string userSettingsPath =  AppDomain.CurrentDomain.BaseDirectory + "/data/userSettings.json";

    public TwitchTTSBotSettingsManager()
    {

        // attempt to load settings if fails use defaults

        if (!Load())
        {

            Save();

        }
    }

    public bool Load()
    {

        if (File.Exists(userSettingsPath))
        {

            try
            {
                FileStream fs = new FileStream(userSettingsPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadToEnd();
                if (str != null)
                {
                    TwitchTTSBotSettings? settings = JsonConvert.DeserializeObject<TwitchTTSBotSettings>(str);
                    this.settings = settings;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load substitutionWords.json");
                return false;
            }
        }
        else
        {
            return false;
        }


        return true;


    }

    public bool Save()
    {

        try{
            if (!File.Exists(userSettingsPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(userSettingsPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the userSettingsPath Directory.");
        }


            try
            {
                FileStream fs = new FileStream(userSettingsPath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string userSettingsJson = JsonConvert.SerializeObject(settings);
                sw.WriteLine(userSettingsJson);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.ToString());
                System.Console.WriteLine("A problem occurred while trying to save userSettings.");
                return false;
            }
            return true;

    }


}

[System.Serializable]

class TwitchTTSBotSettings
{

    public string botName = "COHopponentBot"; // the bots chat userName
    public string botOAuthKey = "oauth:6lwp9xs2oye948hx2hpv5hilldl68g"; // the key you want to use for connection to the twitch server
    public string defaultJoinChannel = "xcomreborn"; // typically the broadcasters twitch channel
    public string botAdminUserName = "xcomreborn"; // incase you want to use the bot on someone elses channel only you will hear the tts.

    public override string ToString()
    {
        return "Object Contains TTS Bot Settings";
    }

}
using Newtonsoft.Json;


namespace TTSBot;

class TTSBotCommands{

    public Dictionary<string, Commands> commands = new Dictionary<string, Commands>();

    public string commandFilePath = AppDomain.CurrentDomain.BaseDirectory + "/data/commandFile.json";


    public TTSBotCommands(){

        if (!Load()){

            // Default Dictionary
            commands = new Dictionary<string, Commands>
        {
            { "!closetts", new Commands(Commands.UserLevel.STREAMER, "!closetts", true , "@","Closes the TTS bot.") },
            { "!ignoreword", new Commands(Commands.UserLevel.STREAMER, "!ignoreword", true , "@ [word]","Adds word to be ignored, will not speak the entire user message.") },
            { "!unignoreword", new Commands(Commands.UserLevel.STREAMER, "!unignoreword", true , "@ [word]" ,"Removes word if it has previously been added to the ignore word list.") },
            { "!alias", new Commands(Commands.UserLevel.MOD, "!alias", true , "@ #","Gives the chat user name an alternative alias in text to speech.") },
            { "!useralias", new Commands(Commands.UserLevel.MOD, "!useralias", true , "@ [userName] #","Gives another chat user name an alternative alias in text to speech.") },                
            { "!ignorelist", new Commands(Commands.UserLevel.MOD, "!ignorelist", true , "@","Displays the userNames currently being ignored in chat.") }, 
            { "!ignore", new Commands(Commands.UserLevel.MOD, "!ignore", true , "@ [userName]","Mutes the specified user.") }, 
            { "!unignore", new Commands(Commands.UserLevel.MOD, "!unignore", true , "@ [userName]","Unmutes the specified user.") },  
            { "!voices", new Commands(Commands.UserLevel.MOD, "!voices", true , "@","Displays the available voices and the number indicies.") },  
            { "!voice", new Commands(Commands.UserLevel.MOD, "!voice", true , "@ #","Chooses an available voice using the number index.") },
            { "!uservoice", new Commands(Commands.UserLevel.MOD, "!uservoice", true , "@ [userName] #","Chooses an available voice for a specified user using the number index.") }, 
            { "!substitute", new Commands(Commands.UserLevel.MOD, "!substitute", true , "@ [word] [substitute words]","Substitutes a word for another word or phrase.") },    
            { "!removesubstitute", new Commands(Commands.UserLevel.MOD, "!removesubstitute", true , "@ [word]","Removes a word that was previously being substituted.") },
            { "!regex", new Commands(Commands.UserLevel.MOD, "!regex", true , "@ [regex] [substitute words]","Substitutes a regular expression for another word or phrase.") }, 
            { "!removeregex", new Commands(Commands.UserLevel.MOD, "!removeregex", true , "@ [regex]","Removes a regular expression that was previously being substituted.") },

        };

            Save();

        }

    }

    public bool AddCommand(string command ="", Commands.UserLevel privilegeLevel = Commands.UserLevel.STREAMER, string ttsComparisonCommand = "", bool enabled = true, string usage = "@", string description = ""){

        try{
        commands.Add(command, new Commands(privilegeLevel, ttsComparisonCommand, enabled, usage, description));
        return true;
        }catch{
        return false;
        }
    }

    public bool ToggleCommand(string command, bool enabled){

        try{
            if (command != null){
                
                Commands userCommand = commands[command];
                userCommand.enabled = enabled;
                commands[command] = userCommand;
                Save();
                return true;

            }


        }catch{
            return false;
        }

        return false;
    }

    public bool ModifyCommand(string command, string ttsComparisonCommand)
    {

        try
        {
            if (ttsComparisonCommand != null)
            {

                // make list of all strings contained in the commands dictionary
                List<string> theList = new List<string>();
                foreach (var item in commands)
                {
                    theList.Add(item.Value.ttsComparisonCommand);
                }
                if (!theList.Contains(ttsComparisonCommand))
                {
                    Commands userCommand = commands[command];
                    userCommand.ttsComparisonCommand = ttsComparisonCommand;
                }
                else
                {
                    // ttsComparisonCommand is already in the command list.
                    System.Console.WriteLine("ttsComparisonCommand is already in the command list.");
                    return false;
                }
            }
        }
        catch
        {
            // command key may already exist in dictionary or command is null
            System.Console.WriteLine("Command key may already exist in dictionary or command is null");
            return false;
        }
        return true;
    }


    public bool Load()
        {

       if (File.Exists(commandFilePath))
        {

            try
            {
                FileStream fs = new FileStream(commandFilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadToEnd();
                if (str != null)
                {

                    Dictionary<string,Commands>? wordDictionary = JsonConvert.DeserializeObject<Dictionary<string,Commands>>(str);
                    this.commands = wordDictionary;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load commandFile.json");
                return false;
            }
        }
        else{
            return false;
        }

    
        return true;

    }

    public bool Save()

        {

        try{
            if (!File.Exists(commandFilePath)){
            Directory.CreateDirectory(Path.GetDirectoryName(commandFilePath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the commandFilePath Directory.");
        }


        try
        {
            FileStream fs = new FileStream(commandFilePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string commandJson = JsonConvert.SerializeObject(commands);
            sw.WriteLine(commandJson);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch
        {
            System.Console.WriteLine("A problem occurred while trying to save commandFile.json");
            return false;
        }

        return true;
    }


}


[System.Serializable]
class Commands{

    public enum UserLevel { USER, VIP,MOD, STREAMER};

    public UserLevel privilageLevel = UserLevel.STREAMER;

    public string ttsComparisonCommand = "";

    public bool enabled = true;

    public string usage = "";

    public string description = "";

    public Commands (UserLevel privilageLevel = UserLevel.STREAMER, string ttsComparisonCommand = "", bool enabled = true, string usage = "",string description = ""){

        this.privilageLevel = privilageLevel;
        this.ttsComparisonCommand = ttsComparisonCommand;
        this.enabled = enabled;
        this.usage = usage;
        this.description = description;

    }


}


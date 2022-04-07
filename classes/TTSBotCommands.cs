using Newtonsoft.Json;
using TwitchLib.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;


namespace TTSBot;

class TTSBotCommands{

    private TextToSpeech tts;

    public Dictionary<Delegate, Commands> commands = new Dictionary<Delegate, Commands>();

    public string commandFilePath = AppDomain.CurrentDomain.BaseDirectory + "/data/commandFile.json";


    public TTSBotCommands(TextToSpeech tts){

        this.tts = tts;

                    // Default Dictionary
            commands = new Dictionary<Delegate, Commands>
        {
            { tts.CloseTTS, new Commands("!closetts",Commands.UserLevel.STREAMER, "!closetts", true , "{0}","Closes the TTS bot.") },
            { tts.SetIgnoreWord, new Commands("!ignoreword",Commands.UserLevel.STREAMER, "!ignoreword", true , "{0} [word]","Adds word to be ignored, will not speak the entire user message.") },
            { tts.SetUnignoreWord, new Commands("!unignoreword",Commands.UserLevel.STREAMER, "!unignoreword", true , "{0} [word]" ,"Removes word if it has previously been added to the ignore word list.") },
            { tts.SetAlias, new Commands("!alias",Commands.UserLevel.MOD, "!alias", true , "{0} #","Gives the chat user name an alternative alias in text to speech.") },
            { tts.SetUserAlias, new Commands("!useralias",Commands.UserLevel.MOD, "!useralias", true , "{0} [userName] #","Gives another chat user name an alternative alias in text to speech.") },                
            { tts.DisplayBlackList, new Commands("!ignorelist",Commands.UserLevel.MOD, "!ignorelist", true , "{0}","Displays the userNames currently being ignored in chat.") }, 
            { tts.SetIgnore, new Commands("!ignore",Commands.UserLevel.MOD, "!ignore", true , "{0} [userName]","Mutes the specified user.") }, 
            { tts.SetUnignore, new Commands("!unignore",Commands.UserLevel.MOD, "!unignore", true , "{0} [userName]","Unmutes the specified user.") },  
            { tts.DisplayAvailableVoices, new Commands("!voices",Commands.UserLevel.MOD, "!voices", true , "{0}","Displays the available voices and the number indicies.") },  
            { tts.SetVoice, new Commands("!voice",Commands.UserLevel.MOD, "!voice", true , "{0} #","Chooses an available voice using the number index.") },
            { tts.SetUserVoice, new Commands("!uservoice",Commands.UserLevel.MOD, "!uservoice", true , "{0} [userName] #","Chooses an available voice for a specified user using the number index.") }, 
            { tts.SetSubstituteWord, new Commands("!substitute",Commands.UserLevel.MOD, "!substitute", true , "{0} [word] [substitute words]","Substitutes a word for another word or phrase.") },    
            { tts.RemoveSubstitute, new Commands("!removesubstitute",Commands.UserLevel.MOD, "!removesubstitute", true , "{0} [word]","Removes a word that was previously being substituted.") },
            { tts.SetRegex, new Commands("!regex",Commands.UserLevel.MOD, "!regex", true , "{0} [regex] [substitute words]","Substitutes a regular expression for another word or phrase.") }, 
            { tts.RemoveRegex, new Commands("!removeregex",Commands.UserLevel.MOD, "!removeregex", true , "{0} [regex]","Removes a regular expression that was previously being substituted.") },

        };

        if (!Load()){

            Save();

        }

    }

    public bool AddCommand(Delegate command = null, string name = "", Commands.UserLevel privilegeLevel = Commands.UserLevel.STREAMER, string ttsComparisonCommand = "", bool enabled = true, string usage = "@", string description = ""){

        try{
        commands.Add(command, new Commands(name, privilegeLevel, ttsComparisonCommand, enabled, usage, description));
        return true;
        }catch{
        return false;
        }
    }

    public bool ToggleCommand(Delegate command, bool enabled){

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

    public bool ModifyCommand(Delegate command, string ttsComparisonCommand)
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

                    List<Commands>? commandList = JsonConvert.DeserializeObject<List<Commands>>(str);
                    
                    foreach (var item in commandList){
                        foreach (var kvitem in commands){
                            if (kvitem.Value.name == item.name){
                                commands[kvitem.Key] = item;
                            }
                        }

                    }
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
            List<Commands> commandList = new List<Commands>();
            foreach (var item in commands){
                commandList.Add(item.Value);
            }
            string commandJson = JsonConvert.SerializeObject(commandList);
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

    public readonly string name;

    public UserLevel privilageLevel = UserLevel.STREAMER;

    public string ttsComparisonCommand = "";

    public bool enabled = true;

    public string usage = "";

    public string description = "";

    public Commands (string name = "", UserLevel privilageLevel = UserLevel.STREAMER, string ttsComparisonCommand = "", bool enabled = true, string usage = "",string description = ""){

        this.name = name;
        this.privilageLevel = privilageLevel;
        this.ttsComparisonCommand = ttsComparisonCommand;
        this.enabled = enabled;
        this.usage = usage;
        this.description = description;

    }


}


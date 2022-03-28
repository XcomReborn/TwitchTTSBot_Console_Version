using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;


namespace TestConsole
{


    class Program
    {
        static void Main(string[] args)
        {

            Bot bot = new Bot();
            Console.ReadLine();

        }
    }



    class Bot
    {
        public TwitchClient client;

        private TwitchTTSBotSettingsManager botSettingManager = new TwitchTTSBotSettingsManager();

        public TwitchUsers users = new TwitchUsers();

        private IgnoredWords ignoredWords = new IgnoredWords();

        private SubstitutionWords substitutionWords = new SubstitutionWords();

        

        private string previousUserName = "";

        public Bot()
        {


            ConnectionCredentials credentials = new ConnectionCredentials(botSettingManager.settings.botName, botSettingManager.settings.botOAuthKey);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, botSettingManager.settings.defaultJoinChannel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Text To Speech Bot Has Connected to This Channel!");
            client.SendMessage(e.Channel, String.Format("{0}'s TTSBot Has Connected to This Channel!", (char.ToUpper(botSettingManager.settings.botAdminUserName[0]) + botSettingManager.settings.botAdminUserName.Substring(1))));
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            // check for any text to speech chat commands 
            this.CheckForChatCommands(e);

            // Send to speech
            this.Speak(e);

        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        private void Speak(OnMessageReceivedArgs e)
        {

            string userName = e.ChatMessage.Username;

            //check for alias
            TwitchUser user = new TwitchUser(userName);
            if (users.IsUserInList(user))
            {
                user = users.GetUser(user);
                userName = user.alias;
            }

            // ensure message is not null
            if (e.ChatMessage.Message != null){

            // if the user exists they might be set to ignore or message starts with ! char
            if ((!user.ignored) && (!(e.ChatMessage.Message[0] == "!"[0])) && (!(ignoredWords.ContainsIgnoredWord(e.ChatMessage.Message))))
            {

                //only use username said something, if not saying for first time in a row.
                string spokenString = "";

                //substitute any words in the user message for the ones in the substitution dictionary.
                string messageToTextToSpeech = SubstituteWords(e);

                if (previousUserName == userName)
                {

                    spokenString = messageToTextToSpeech;

                }
                else
                {
                    spokenString = userName + " said " + messageToTextToSpeech;
                }
                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();

                // Configure the audio output.   
                synth.SetOutputToDefaultAudioDevice();

                // Set the voice based on Name
                if (user.voiceName != ""){
                synth.SelectVoice(user.voiceName);
                }
                // This requires testing to see if the voiceNumber index is correct.
                //synth.SelectVoiceByHints(VoiceGender.NotSet,VoiceAge.NotSet,user.voiceNumber);

                // Speak a string.  
                synth.Speak(spokenString);

                previousUserName = userName;
            }

            }

        }


        private string SubstituteWords(OnMessageReceivedArgs e){

            //check if there are any keys to substitute
            if (substitutionWords.subwords.words.Count > 0){

            string output = "";
            try{

                
                var words = string.Join("|", substitutionWords.subwords.words.Keys);
                System.Console.WriteLine("Word Sub Pattern Matches : " + $@"\b({words})\b");
                // This next line replaces all the dictionary key matches with their value pairs, exclusive bound words. How does it work? No idea!

                output = Regex.Replace(e.ChatMessage.Message, $@"\b({words})\b", delegate (Match m) 
                { 

                    return substitutionWords.subwords.words[Regex.Escape(m.Value)].PickRandom();  }  
                
                );

                if (substitutionWords.subwords.regularexpressions.Count > 0){

                     // this will iterate over regular expressions in the regular expression dictionary so care should be taken with the entered patterns.
                    foreach (KeyValuePair<string, List<string>> item in substitutionWords.subwords.regularexpressions){
                        output = Regex.Replace(output, item.Key, item.Value.PickRandom());
                        System.Console.WriteLine("Regex Sub Pattern Matches : " + item.Key + " : Sub : " + item.Value);
                    }


                }



            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
                System.Console.WriteLine("Problems in SubstituteWords.");
                return "";

            }
            return output;
            
            }else{
            return e.ChatMessage.Message;
            }
        }



        private void CheckForChatCommands(OnMessageReceivedArgs e)
        {


            string[] words = e.ChatMessage.Message.Split(' ');

            if (words.Length > 0)
            {


                // Broadcaster Commands
                if ((e.ChatMessage.IsBroadcaster) || (this.botSettingManager.settings.botAdminUserName.ToLower() == e.ChatMessage.Username))
                {
                    switch (words[0])
                    {

                        case "!closetts":
                            CloseTTS(e);
                            break;
                        case "!ignoreword":
                            SetIgnoreWord(e);
                            break;
                        case "!unignoreword":
                            SetUnignoreWord(e);
                            break;

                        default:
                            // code block
                            break;
                    }

                }


                // Moderator commands 
                if ((e.ChatMessage.IsModerator) || (e.ChatMessage.IsBroadcaster) || (this.botSettingManager.settings.botAdminUserName.ToLower() == e.ChatMessage.Username))
                {

                    switch (words[0])
                    {
                        case "!alias":
                            SetAlias(e);
                            break;
                        case "!useralias":
                            SetUserAlias(e);
                            break;

                        case "!blacklist":
                        case "!ignorelist":
                            DisplayBlackList(e);
                            break;
                        case "!ignore":
                            SetIgnore(e);
                            break;
                        case "!unignore":
                            SetUnignore(e);
                            break;
                        case "!voices":
                            DisplayAvailableVoices(e);
                            break;
                        case "!voice":
                            SetVoice(e);
                            break;
                        case "!uservoice":
                            SetUserVoice(e);
                            break;
                        case "!substitute":
                            SetSubstituteWord(e);
                            break;
                        case "!removesubstitute":
                            RemoveSubstitute(e);
                            break;
                        case "!regex":
                            SetRegex(e);
                            break;
                        case "!removeregex":
                            RemoveRegex(e);
                            break;
                        case "!speed":
                            // currently not bothering implement this
                            break;
                        case "!volume":
                            // currently not bothering implement this
                            break;


                        default:
                            // code block
                            break;
                    }


                }

                // extra commands for non mods
                /*
                switch (words[0])
                {
                    case "!voices":
                        // code block
                        break;
                    default:
                        // code block
                        break;
                }

                */

            }




        }

        private bool SetRegex(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 2){
                // typical input in the form of !regex \b8=*D\b|\b8-*D\b wub
                string pattern = wordList[1];
                string wordToSubstitute = String.Join(" ", wordList.Skip(2));

                substitutionWords.AddRegularExpressionSubPair(pattern, wordToSubstitute);
                substitutionWords.Save();

                client.SendMessage(e.ChatMessage.Channel, String.Format("{0} regex pattern will be substituted with {1} in text to speech.", pattern, wordToSubstitute));

                return true;

            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Please enter the command in the form : !regex pattern substitute"));
            return false;

        }

        private bool RemoveRegex(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1){
                string pattern = wordList[1];

                bool success = substitutionWords.RemoveRegularExpressionSubPair(pattern);

                if (success){

                client.SendMessage(e.ChatMessage.Channel, String.Format("{0} pattern has been removed from the word regex substitution list.", pattern));
                substitutionWords.Save();
                return true;
                }
                else{
                    client.SendMessage(e.ChatMessage.Channel, String.Format("Could not remove {0} pattern from the regex substitution list.", pattern));
                    return false;
                }

            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Please enter word to remove in the form of !removeregex word."));
            return false;


        }
        
        private bool RemoveSubstitute(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1){
                string wordToRemove = wordList[1];


                bool success = substitutionWords.RemoveWord(wordToRemove);

                if (success){

                client.SendMessage(e.ChatMessage.Channel, String.Format("{0} has been removed from the word substitution list.", wordToRemove));
                substitutionWords.Save();
                return true;
                }
                else{
                    client.SendMessage(e.ChatMessage.Channel, String.Format("Could not remove {0} from the word substitution list.", wordToRemove));
                    return false;
                }

            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Please enter word to remove in the form of !removesubstitute word."));
            return false;

        }

        private bool SetSubstituteWord(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 2){
                // typical input in the form of !substitute <3 wub
                string word = wordList[1];
                System.Console.WriteLine(String.Format("word : {0}", word));
                string wordToSubstitute = String.Join(" ", wordList.Skip(2));

                substitutionWords.AddWordPair(word, wordToSubstitute);
                substitutionWords.Save();

                client.SendMessage(e.ChatMessage.Channel, String.Format("{0} will be substituted with {1} in text to speech.", word, wordToSubstitute));

                return true;

            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Please enter the command in the form : !substitute word substitute"));
            return false;

        }


        private bool DisplayBlackList(OnMessageReceivedArgs e){


            

            List<TwitchUser> ignoredList = new List<TwitchUser>();

            // get only users where ignore is true and put into new list

            foreach (TwitchUser user in users.users){

                if (user.ignored == true){

                    ignoredList.Add(user);
                }

            }

            List<string> names = new List<string>();

            foreach (TwitchUser user in ignoredList){

                names.Add(user.name);

            }

            string output = "";
            output = String.Join(",", names);

            client.SendMessage(e.ChatMessage.Channel, String.Format("Ignored Users : {0}.", output));

            return true;


        }

        private bool SetVoice(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {   

                int voiceNumber = -1;
                string voiceNumberString = wordList[1]; 
                try{
                    voiceNumber = int.Parse(voiceNumberString);
                }
                catch{
                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} is not a vaild voice number.", voiceNumberString));
                    System.Console.WriteLine("Problem parsing string to int in SetVoice.");
                    return false;
                }

                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();
                ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();
                if ( 0 <= voiceNumber && voiceNumber <= (installedVoices.Count - 1)){

                    TwitchUser user = new TwitchUser(e.ChatMessage.Username);
                    if (users.IsUserInList(user)){

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);

                    }
                    else{

                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);
                        
                    }

                     client.SendMessage(e.ChatMessage.Channel, String.Format("{0} has selected voice : {1}", user.name, user.voiceName));

                    users.Save();

                }

            }

            return true;


        }

        private bool SetUserVoice(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 2)
            {   

                int voiceNumber = -1;
                string userName = wordList[1];
                string voiceNumberString = wordList[2]; 
                try{
                    voiceNumber = int.Parse(voiceNumberString);
                }
                catch{
                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} is not a vaild voice number.", voiceNumberString));
                    System.Console.WriteLine("Problem parsing string to int in SetVoice.");
                    return false;
                }

                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();
                ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();
                if ( 0 <= voiceNumber && voiceNumber <= (installedVoices.Count - 1)){

                    TwitchUser user = new TwitchUser(userName);
                    if (users.IsUserInList(user)){

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);

                    }
                    else{

                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);
                        
                    }

                     client.SendMessage(e.ChatMessage.Channel, String.Format("{0} voice has been set to voice : {1}", user.name, user.voiceName));

                    users.Save();

                }

            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Correct useage in the form of !uservoice [UserName] [VoiceNumber]"));

            return true;


        }

        private void DisplayAvailableVoices(OnMessageReceivedArgs e){

            string voices = "";

            // Initialize a new instance of the SpeechSynthesizer.  
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();

            ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();

            int index = 0;
            try{
            foreach (InstalledVoice voice in installedVoices){

                string voiceName = voice.VoiceInfo.Name;
                voiceName = voiceName.Replace("Microsoft ", ""); //# remove the word microsoft
                //voiceName = voiceName.Replace(" English " , ""); //# remove the word English 
                voiceName = voiceName.Replace("Desktop", "" ); //# remove the word Desktop
                voiceName = voiceName.Replace("Mobile", ""); //# remove the word Mobile
                voiceName = voiceName.Replace("-" , ""); //# remove the character "-"
                voiceName = voiceName.Replace("(Canada)" , ""); // just removes canada for Eva if present
                string culture = "  ";
                try{
                    culture = voice.VoiceInfo.Culture.Name.Substring(3, 2);

                }catch{

                    System.Console.WriteLine("Problem getting last two characters of : " + voice.VoiceInfo.Culture.Name);

                }

                voices += " " + index.ToString() + ". " + voiceName + " (" + culture + ") ";
                //System.Console.WriteLine(voice.VoiceInfo.Name);
                index++;

            }
            }catch{
                
                System.Console.WriteLine("Problem gettings installed voices.");
            }


            client.SendMessage(e.ChatMessage.Channel, String.Format("Available Voices : {0}", voices.Substring(0, Math.Min(voices.Length, 500)))); 

        }

        private void SetIgnoreWord(OnMessageReceivedArgs e){


            
            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.AddWord(wordList[1]);
                ignoredWords.Save();

                client.SendMessage(e.ChatMessage.Channel, String.Format("messages containing {0} will be ignored.", wordList[1]));  

            }


        }

        private void SetUnignoreWord(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.RemoveWord(wordList[1]);
                ignoredWords.Save();

                client.SendMessage(e.ChatMessage.Channel, String.Format("messages containing {0} will not be ignored.", wordList[1]));  

            }            


        }

        private void CloseTTS(OnMessageReceivedArgs e)
        {

            System.Console.WriteLine("Closing TTS.");
            client.SendMessage(e.ChatMessage.Channel, "Closing TTS Bot.");

            // BruteForce the Exit
            Environment.Exit(0);

        }

        private void SetAlias(OnMessageReceivedArgs e)
        {

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                string alias = Sanitize(String.Join(" ", wordList.Skip(1)));
                TwitchUser user = new TwitchUser(e.ChatMessage.Username, alias);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.alias = alias;
                        users.AddUser(user);
                    }
                    else
                    {

                        users.AddUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0}'s alias has been set to {1}", e.ChatMessage.Username, alias));

                    users.Save();

                    return;

                }


            }


        }

        private void SetUserAlias(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 2)
            {
                string userName = wordList[1];
                string alias = Sanitize(String.Join(" ", wordList.Skip(2)));
                TwitchUser user = new TwitchUser(userName, alias);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.alias = alias;
                        users.AddUser(user);
                    }
                    else
                    {

                        users.AddUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0}'s alias has been set to {1}", userName, alias));

                    users.Save();

                    return;

                }


            }

            client.SendMessage(e.ChatMessage.Channel, String.Format("Correct usage in the form !useralias userName alias"));

        }

        private void SetIgnore(OnMessageReceivedArgs e)
        {
            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {
                TwitchUser user = new TwitchUser(wordList[1]);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.ignored = true;
                        users.AddUser(user);
                    }
                    else
                    {

                        user.ignored = true;
                        users.AddUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} will be ignored.", user.name));
                    users.Save();

                }

            }

        }

        private void SetUnignore(OnMessageReceivedArgs e)
        {
            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                TwitchUser user = new TwitchUser(wordList[1]);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.ignored = false;
                        users.AddUser(user);
                    }
                    else
                    {

                        user.ignored = false;
                        users.AddUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} will not be ignored.", user.name));
                    users.Save();

                }


            }


        }

        private string Sanitize(string str)
        {

            // Allowed Characters only allowed in alias
            string pattern = @"[^a-zA-Z0-9 -.&,%£+=?*@!#]";
            // Create a Regex  
            Regex rg = new Regex(pattern);

            string match = rg.Replace(str, "");

            return match;

        }




    }
}

public static class EnumerableExtension
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}


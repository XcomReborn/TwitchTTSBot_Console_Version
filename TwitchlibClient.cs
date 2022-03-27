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

        public TwitchUsers users = new TwitchUsers();

        public IgnoredWords ignoredWords = new IgnoredWords();

        private string previousUserName = "";

        public Bot()
        {

            // load all tts users
            users.load();
            // load ignored words
            ignoredWords.load();

            ConnectionCredentials credentials = new ConnectionCredentials("COHopponentBot", "oauth:6lwp9xs2oye948hx2hpv5hilldl68g");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "xcomreborn");

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
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //if (e.ChatMessage.Message.Contains("badword"))
            //    client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(1), "Bad word! 30 minute timeout!");

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
            if (users.isUserInList(user))
            {
                user = users.getUser(user);
                userName = user.alias;
            }

            // ensure message is not null
            if (e.ChatMessage.Message != null){

            // if the user exists they might be set to ignore or message starts with ! char
            if ((!user.ignored) && (!(e.ChatMessage.Message[0] == "!"[0])) && (!(ignoredWords.containsIgnoredWord(e.ChatMessage.Message))))
            {

                //only use username said something, if not saying for first time in a row.
                string spokenString = "";
                if (previousUserName == userName)
                {

                    spokenString = e.ChatMessage.Message;

                }
                else
                {
                    spokenString = userName + " said " + e.ChatMessage.Message;
                }
                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();

                // Configure the audio output.   
                synth.SetOutputToDefaultAudioDevice();

                // Set the voice based on Name
                if (user.voiceName != ""){
                synth.SelectVoice(user.voiceName);
                }

                // Speak a string.  
                synth.Speak(spokenString);

                previousUserName = userName;
            }

            }

        }

        private void CheckForChatCommands(OnMessageReceivedArgs e)
        {


            string[] words = e.ChatMessage.Message.Split(' ');

            if (words.Length > 0)
            {



                // Broadcaster Commands
                if (e.ChatMessage.IsBroadcaster)
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

                if ((e.ChatMessage.IsModerator) || (e.ChatMessage.IsBroadcaster))
                {

                    switch (words[0])
                    {
                        case "!alias":
                            SetAlias(e);
                            break;
                        case "!blacklist":
                        case "!ignorelist":
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
                        case "!speed":
                            break;
                        case "!volume":
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
                    System.Console.WriteLine("Problem parsing string to int in SetVoice.");
                    return false;
                }

                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();
                ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();
                if ( 0 <= voiceNumber && voiceNumber <= (installedVoices.Count - 1)){

                    TwitchUser user = new TwitchUser(e.ChatMessage.Username);
                    if (users.isUserInList(user)){

                        user = users.getUser(user);
                        users.removeUser(user);
                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.addUser(user);

                    }
                    else{

                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.addUser(user);
                        
                    }

                    users.save();

                }

            }

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

                    System.Console.WriteLine(voice.VoiceInfo.Culture.Name);

                }

                voices += " " + index.ToString() + ". " + voiceName + " (" + culture + ") ";
                //System.Console.WriteLine(voice.VoiceInfo.Name);
                index++;

            }
            }catch{

                System.Console.WriteLine("Problem gettings installed voices.");
            }


            client.SendMessage(e.ChatMessage.Channel, String.Format("Available Voices : {0}", voices.Substring(0,voices.Length))); 

        }

        private void SetIgnoreWord(OnMessageReceivedArgs e){


            
            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.addWord(wordList[1]);
                ignoredWords.save();

                client.SendMessage(e.ChatMessage.Channel, String.Format("messages containing {0} will be ignored.", wordList[1]));  

            }


        }

        private void SetUnignoreWord(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.removeWord(wordList[1]);
                ignoredWords.save();

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

                    if (users.isUserInList(user))
                    {

                        user = users.getUser(user);
                        users.removeUser(user);
                        user.alias = alias;
                        users.addUser(user);
                    }
                    else
                    {

                        users.addUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0}'s alias has been set to {1}", e.ChatMessage.Username, alias));

                    users.save();

                }


            }


        }

        private void SetIgnore(OnMessageReceivedArgs e)
        {
            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1)
            {
                TwitchUser user = new TwitchUser(wordList[1]);
                if (user != null)
                {

                    if (users.isUserInList(user))
                    {

                        user = users.getUser(user);
                        users.removeUser(user);
                        user.ignored = true;
                        users.addUser(user);
                    }
                    else
                    {

                        user.ignored = true;
                        users.addUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} will be ignored.", user.name));
                    users.save();

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

                    if (users.isUserInList(user))
                    {

                        user = users.getUser(user);
                        users.removeUser(user);
                        user.ignored = false;
                        users.addUser(user);
                    }
                    else
                    {

                        user.ignored = false;
                        users.addUser(user);

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0} will not be ignored.", user.name));
                    users.save();

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
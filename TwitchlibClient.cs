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


namespace TestConsole
{

    
    class Program
    {
        static void Main(string[] args)
        {


        Bot bot = new Bot();

        System.Console.ReadLine() ;


        }
    }



    class Bot
    {
        public TwitchClient client;

        public twitchUsers users = new twitchUsers();

        private string previousUserName = "";
	
        public Bot()
        {

            // load all tts users
            users.load();

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

    private void Speak(OnMessageReceivedArgs e){

            // Initialize a new instance of the SpeechSynthesizer.  
            SpeechSynthesizer synth = new SpeechSynthesizer();  

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();  

            string userName = e.ChatMessage.Username;

            //check for alias
            twitchUser user = new twitchUser(userName, "");
            if (users.isUserInList(user)){  
                user = users.getUser(user);
                userName = user.alias;
            }

            //only use username said something, if not saying for first time in a row.
            string spokenString = "";
            if (previousUserName == userName){

                spokenString = e.ChatMessage.Message;

            }
            else{
            spokenString = userName + " said " + e.ChatMessage.Message;
            }
            // Speak a string.  
            synth.Speak(spokenString);  

            previousUserName = userName;

    }

        private void CheckForChatCommands(OnMessageReceivedArgs e){


            string[] words = e.ChatMessage.Message.Split(' ');

            if (words.Length > 0){



            // Broadcaster Commands
            if (e.ChatMessage.IsBroadcaster){
            switch(words[0]) 
            {
            
            case "!closetts":
                CloseTTS();
                break;
            default:
                // code block
                break;
            }

            }

            if ((e.ChatMessage.IsModerator) || (e.ChatMessage.IsBroadcaster)){

            switch(words[0]) 
            {
            case "!alias":
                SetAlias(e);
                break;
            case "!voice":
                break;
            case "!blacklist":
            case "!ignorelist":
                break;
            case "!ignore":
                break;
            case "!unignore":
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


            switch(words[0]) 
            {
            case "!voices":
                // code block
                break;
            default:
                // code block
                break;
            }

            }



 
        }

        private void CloseTTS(){

            System.Console.WriteLine("Closing TTS.");

            client.Disconnect();

            //wait until disconnected
            while (client.IsConnected){};

            Environment.Exit(0);
            

        }

        private void SetAlias(OnMessageReceivedArgs e){

            string[] wordList = e.ChatMessage.Message.Split(' ');
            if (wordList.Length > 1){
                string alias = Sanitize (wordList[1]);
                twitchUser user = new twitchUser(e.ChatMessage.Username, alias);
                if (user != null){

                    //System.Console.WriteLine("Got this far");
                    //System.Console.WriteLine(e.ChatMessage.Username);
                    //System.Console.WriteLine(alias);
                    // check if user already exists
                    //System.Console.WriteLine(users.isUserInList(user).ToString());
                    if (users.isUserInList(user)){

                        user = users.getUser(user);
                        users.removeUser(user);
                        user.alias = alias;
                        users.addUser(user);
                    }
                    else{

                    //System.Console.WriteLine(users.ToString());
                    //System.Console.WriteLine(users.users.Count.ToString());

                    users.addUser(user);

                    //System.Console.WriteLine(user.ToString());
                    //System.Console.WriteLine(users.ToString());
                    //System.Console.WriteLine(users.users.Count.ToString());

                    }

                    client.SendMessage(e.ChatMessage.Channel, String.Format("{0}'s alias has been set to {1}", e.ChatMessage.Username ,alias));

                    users.save();

                }


            }


        }

        private string Sanitize(string str){

            // Allowed Characters only allowed in alias
            string pattern = @"[^a-zA-Z0-9 -.&,%£+=?*@!#]";  
            // Create a Regex  
            Regex rg = new Regex(pattern);

            string match = rg.Replace(str, "");

            return match;

        } 




    }
}
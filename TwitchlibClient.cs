using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System.Speech.Synthesis;

namespace TestConsole
{

    
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            while (bot.client.IsConnected){

                // runs while connected

            }
        }
    }



    class Bot
    {
        public TwitchClient client;
	
        public Bot()
        {
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
            
            // Initialize a new instance of the SpeechSynthesizer.  
            SpeechSynthesizer synth = new SpeechSynthesizer();  

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();  

            // Speak a string.  
            synth.Speak(e.ChatMessage.Message);  


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


        private void CheckForChatCommands(OnMessageReceivedArgs e){

            switch(e.ChatMessage.Message) 
            {
            case "!alias":
                // code block
                break;
            case "!voice":
                // code block
                break;
            case "!closetts":
                closeTTS();
                break;
            default:
                // code block
                break;
            }
 

        }

        private void closeTTS(){

            System.Console.WriteLine("Closing TTS.");

            client.Disconnect();

        }




    }
}

using TwitchBotClient;


namespace MainProgram
{

    class Program
    {
        static void Main(string[] args)
        {

            Bot bot = new Bot();

            TextToSpeech.TextToSpeech tts = new TextToSpeech.TextToSpeech(bot);

            tts.run();

            //Console.ReadLine();

        }

}

}
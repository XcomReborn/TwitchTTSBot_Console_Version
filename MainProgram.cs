
namespace TTSBot
{

    class Program
    {
        static void Main(string[] args)
        {

            Bot bot = new Bot();

            TextToSpeech tts = new TextToSpeech(bot);

            tts.run();

            //Console.ReadLine();

        }

}

}
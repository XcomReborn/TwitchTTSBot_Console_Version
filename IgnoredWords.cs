using System.Text.Json;
using System.Linq;

class IgnoredWords
{


    private HashSet<string> words;


    public IgnoredWords()
    {

        words = new HashSet<string>();

    }

    public bool addWord(string word)
    {
        return words.Add(word);
    }

    public bool removeWord(string word)
    {
        return words.Remove(word);
    }

    public bool containsIgnoredWord(string message){

        return ContainsAny(message, words);

    }

    
    private bool ContainsAny(string s, HashSet<string> substrings)
    {
        if (string.IsNullOrEmpty(s) || substrings == null)
            return false;
        return substrings.Any(substring => s.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool load()
    {

        if (File.Exists("ignoredWords.json"))
        {

            try
            {
                FileStream fs = new FileStream("ignoredWords.json", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                if (str != null)
                {
                    Console.WriteLine(str);
                    HashSet<string>? wordList = JsonSerializer.Deserialize<HashSet<string>>(str);
                    this.words = wordList;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load ignoredWords");
                return false;
            }
        }

        return true;

    }

    public bool save()

    {
        try
        {
            FileStream fs = new FileStream("ignoredWords.json", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string userJson = JsonSerializer.Serialize(words);
            sw.WriteLine(userJson);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch
        {
            System.Console.WriteLine("A problem occurred while trying to save ignoredWords");
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string output = "";
        foreach (string word in words)
        {

            output += word + "\n";
        }

        return output;
    }


}
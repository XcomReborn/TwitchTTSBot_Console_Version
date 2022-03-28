using Newtonsoft.Json;
using System.Linq;

class IgnoredWords
{
    
    private HashSet<string> words = new HashSet<string>();

    private string ignoredWordsPath = AppDomain.CurrentDomain.BaseDirectory + "/data/ignoredWords.json";


    public IgnoredWords()
    {

        // attempt to load words from file if fails use defaults

        if (!Load())
        {

            Save();

        }       

    }

    public bool AddWord(string word)
    {
        return words.Add(word);
    }

    public bool RemoveWord(string word)
    {
        return words.Remove(word);
    }

    public bool ContainsIgnoredWord(string message){

        List<string> matches = words.Where(i => message.Contains(i)).ToList();

        if (matches.Count > 0){
            return true;
        }
        return false;

    }


    // currently not using this function
    private bool ContainsAny(string s, HashSet<string> substrings)
    {
        if (string.IsNullOrEmpty(s) || substrings == null)
            return false;
        return substrings.Any(substring => s.Contains(substring, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool Load()
    {

        if (File.Exists(ignoredWordsPath))
        {

            try
            {
                FileStream fs = new FileStream(ignoredWordsPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                if (str != null)
                {
                    Console.WriteLine(str);
                    HashSet<string>? wordList = JsonConvert.DeserializeObject<HashSet<string>>(str);
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

    public bool Save()

    {


        try{
            if (!File.Exists(ignoredWordsPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(ignoredWordsPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the ignoredWordsPath Directory.");
        }

        try
        {
            FileStream fs = new FileStream(ignoredWordsPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string userJson = JsonConvert.SerializeObject(words);
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
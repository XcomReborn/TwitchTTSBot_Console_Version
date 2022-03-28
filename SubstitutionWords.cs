using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


class SubstitutionWords{

    public SubWords subwords = new SubWords();

    public string substitutionWordsPath = "data/substitutionWords.json";


    public SubstitutionWords(){

        // attempt to load settings if fails use defaults

        if (!Load())
        {

            subwords = new SubWords();
            Save();

        }
        

    }

    public bool AddWordPair(string word, string wordToSubstitute){
        
        subwords.words.Add(Regex.Escape(word), wordToSubstitute);
        return true;
    }

    public bool RemoveWord(string word){

        subwords.words.Remove(word);
        return true;
    }

    public bool AddRegularExpressionSubPair(string pattern, string wordToSubstitute){

        subwords.regularexpressions.Add(pattern, wordToSubstitute);
        return true;
    }

    public bool RemoveRegularExpressionSubPair(string pattern){

        subwords.regularexpressions.Remove(pattern);
        return true;

    }


    public bool Save(){
        try
        {
            FileStream fs = new FileStream(substitutionWordsPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            //string userJson = JsonSerializer.Serialize(subwords);
            string userJson = JsonConvert.SerializeObject(subwords);
            sw.WriteLine(userJson);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch
        {
            System.Console.WriteLine("A problem occurred while trying to save substitutionWords.json");
            return false;
        }

        return true;
    }

    public bool Load(){

       if (File.Exists(substitutionWordsPath))
        {

            try
            {
                FileStream fs = new FileStream(substitutionWordsPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadToEnd();
                if (str != null)
                {
                    //Console.WriteLine(str);
                    //SubWords? wordDictionary = JsonSerializer.Deserialize<SubWords>(str);
                    SubWords? wordDictionary = JsonConvert.DeserializeObject<SubWords>(str);
                    this.subwords = wordDictionary;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load substitutionWords.json");
                return false;
            }
        }

    
        return true;


    }

    public override string ToString()
    {
        return subwords.ToString();
    }


}


[System.Serializable]
class SubWords{

    public Dictionary<string, string> words;
    public Dictionary<string, string> regularexpressions; 

    public SubWords(){

        words = new Dictionary<string, string>();
        regularexpressions = new Dictionary<string, string>();

    }


    public override string ToString()
    {
        string output = "";
        if (words != null){
        var lines = words.Select(kvp => kvp.Key + ": " + kvp.Value);
        output = "words:\n" + string.Join(Environment.NewLine, lines);
        }
        if (regularexpressions != null){
        var lines = regularexpressions.Select(kvp => kvp.Key + ": " + kvp.Value);
        output = "regularexpressions:\n" + string.Join(Environment.NewLine, lines);
        return output;
        }        

        return "Dictionaries Empty.";
    }    


}
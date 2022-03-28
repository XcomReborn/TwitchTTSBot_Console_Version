
using System.Collections;
using System.Text.Json;

class TwitchUsers
{

    public List<TwitchUser> users { get; set; } = new List<TwitchUser>();

    public string dataPath = AppDomain.CurrentDomain.BaseDirectory + "/data/twitchUserData.json";

    public TwitchUsers()
    {

        // attempt to load settings if fails use defaults and create file
        if (!Load())
        {

            Save();

        }
        
    }

    public bool AddUser(string username, string alias)
    {

        try
        {
            users.Add(new TwitchUser(username.ToLower(), alias));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public bool AddUser(TwitchUser user)
    {
        try
        {
            user.name.ToLower();
            users.Add(user);
        }
        catch { return true; }
        return true;
    }

    public bool RemoveUser(TwitchUser user)
    {

        return users.Remove(user);

    }

    public 
    TwitchUser GetUser(TwitchUser user)
    {

        return users.Find(x => x.name.ToLower() == user.name.ToLower());

    }

    TwitchUser GetUser(string userName){

        return users.Find(x => x.name.ToLower() == userName.ToLower());

    }

    public bool IsUserInList(TwitchUser user)
    {

        if (users.Find(x => x.name.ToLower() == user.name.ToLower()) != null)
        {
            return true;
        }
        return false;
    }

      public bool IsUserInList(string userName)
    {

        if (users.Find(x => x.name.ToLower() == userName.ToLower()) != null)
        {
            return true;
        }
        return false;
    }

      


    public bool Load()
    {
        if (File.Exists(dataPath))
        {

            try
            {
                FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                if (str != null)
                {
                    Console.WriteLine(str);
                    List<TwitchUser>? twitchUsers = JsonSerializer.Deserialize<List<TwitchUser>>(str);
                    this.users = twitchUsers;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load twitchUsers");
                return false;
            }
        }

        return true;

    }

    public bool Save()
    {

        try{
            if (!File.Exists(dataPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the dataPath Directory.");
        }
        try{
        FileStream fs = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        string userJson = JsonSerializer.Serialize(users);
        sw.WriteLine(userJson);
        sw.Flush();
        sw.Close();
        fs.Close();
        }
        catch{
            System.Console.WriteLine("A problem occurred while trying to save twitchUsers.");
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string output = "";
        foreach (TwitchUser user in users)
        {

            output += user.ToString() + "\n";
        }

        return output;
    }


}
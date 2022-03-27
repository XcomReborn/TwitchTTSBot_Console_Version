
using System.Collections;
using System.Text.Json;

class TwitchUsers
{

    public List<TwitchUser> users { get; set; }

    public TwitchUsers()
    {
        users = new List<TwitchUser>();
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
        if (File.Exists("data.json"))
        {

            try
            {
                FileStream fs = new FileStream("data.json", FileMode.Open, FileAccess.Read);
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
        FileStream fs = new FileStream("data.json", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        string userJson = JsonSerializer.Serialize(users);
        sw.WriteLine(userJson);
        sw.Flush();
        sw.Close();
        fs.Close();
        }
        catch{
            System.Console.WriteLine("A problem occurred while trying to save twitchUsers");
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
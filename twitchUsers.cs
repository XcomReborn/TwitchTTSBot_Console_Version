
using System.Collections;
using System.Text.Json;

class twitchUsers
{

    public List<twitchUser> users { get; set; }

    public twitchUsers()
    {
        users = new List<twitchUser>();
    }

    public bool addUser(string username, string alias, int voiceNumber = 0, float voiceRate = 200)
    {

        try
        {
            users.Add(new twitchUser(username, alias, voiceNumber, voiceRate));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public bool addUser(twitchUser user)
    {
        try
        {
            users.Add(user);
        }
        catch { return true; }
        return true;
    }

    public bool removeUser(twitchUser user)
    {

        return users.Remove(user);

    }

    public twitchUser getUser(twitchUser user)
    {

        return users.Find(x => x.name == user.name);

    }

    public bool isUserInList(twitchUser user)
    {

        if (users.Find(x => x.name == user.name) != null)
        {
            return true;
        }
        return false;
    }


    public void load()
    {

        FileStream fs = new FileStream("test.txt", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs);
        sr.BaseStream.Seek(0, SeekOrigin.Begin);
        string str = sr.ReadLine();
        while (str != null)
        {
            Console.WriteLine(str);
            List<twitchUser>? twitchUsers = JsonSerializer.Deserialize<List<twitchUser>>(str);
            this.users = twitchUsers;
        }



        sr.Close();
        fs.Close();

    }

    public void save()
    {

        FileStream fs = new FileStream("test.txt", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        string userJson = JsonSerializer.Serialize(users);
        sw.WriteLine(userJson);
        sw.Flush();
        sw.Close();
        fs.Close();
    }

        public override string ToString()
    {
        string output = "";
        foreach (twitchUser user in users){

            output += user.ToString() + "\n";
        }

        return output;
    }


}
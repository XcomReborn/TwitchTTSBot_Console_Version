
using System.Collections;
using System.Text.Json;

class twitchUsers{

    public  List<twitchUser> users { get; set; }

    public bool addUser(){

        return false;
    }


    public bool removeUser (){

        return false;

    }

    public twitchUser getUser(){

        return null;
    }




    public bool isUserInList(){


        return false;
    }


    public void load(){

                FileStream fs = new FileStream("c:\\test.txt", FileMode.Open, FileAccess.Read);  
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

    public void save(){

                FileStream fs = new FileStream("c:\\test.txt", FileMode.Create, FileAccess.Write);  
                StreamWriter sw = new StreamWriter(fs); 
                string userJson = JsonSerializer.Serialize(users);
                sw.WriteLine(userJson);  
                sw.Flush();  
                sw.Close();  
                fs.Close();  
    }

}
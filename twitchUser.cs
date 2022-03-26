

[System.Serializable]
class twitchUser{

    public string name { get; set; }
    public string 	alias { get; set; }
	public int voiceNumber { get; set; }
    public float voiceRate { get; set; }

    public twitchUser(string name, string alias, int voiceNumber, float voiceRate){

        this.name = name;
        this.alias = alias;
        this.voiceNumber = voiceNumber;
        this.voiceRate = voiceRate;

    }



}
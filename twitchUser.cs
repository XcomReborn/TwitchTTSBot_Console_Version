

[System.Serializable]
class twitchUser{

    public string name { get; set; }
    public string alias { get; set; }
	public int voiceNumber { get; set; }
    public float voiceRate { get; set; }
    public bool ignored {get; set;}

    public twitchUser(string name, string alias, int voiceNumber = 0, float voiceRate = (float)200.0, bool ignored = false){

        this.name = name;
        this.alias = alias;
        this.voiceNumber = voiceNumber;
        this.voiceRate = voiceRate;
        this.ignored = ignored;

    }

    public override string ToString()
    {
        return String.Format("userName {0}, alias {1}, voiceNumber {2}, voiceRate {3}, ignored {4}",name, alias, voiceNumber.ToString(), voiceRate.ToString(), ignored.ToString());
    }



}
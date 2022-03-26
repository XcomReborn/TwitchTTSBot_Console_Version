

[System.Serializable]
class twitchUser{

    public string name { get; set; }
    public string alias { get; set; }
	public int voiceNumber { get; set; }
    public float voiceRate { get; set; }

    public twitchUser(string name, string alias, int voiceNumber = 0, float voiceRate = (float)200.0){

        this.name = name;
        this.alias = alias;
        this.voiceNumber = voiceNumber;
        this.voiceRate = voiceRate;

    }

    public override string ToString()
    {
        return String.Format("userName {0}, alias {1}, voiceNumber {2}, voiceRate {3}",name, alias, voiceNumber, voiceRate);
    }



}
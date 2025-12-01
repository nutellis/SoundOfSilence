using UnityEngine;

[CreateAssetMenu(fileName = "InsultWord", menuName = "Insults/InsultWord")]
public class InsultWord : ScriptableObject
{


    // not sure that I should set them to public or ptivate
    // for now I just set ID to private and create a getter since no one should be able to change it

    private string id;
    public string displayText;
    // tags is mabey give words a flag that can be used to make combo or effective against certain type of boss
    public string[] tags;

    public bool isNSFW = false;

    public int baseDamage = 1;
    public int goldCost = 3;




    public string getID()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
        return id;
    }
    public string setId(string newId)
    {
        id = newId;
        return id;
    }
}

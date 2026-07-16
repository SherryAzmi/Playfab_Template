using UnityEngine;

public class PersistentData : MonoBehaviour
{

    public static PersistentData PD;
    public bool[] allSkins;
    public int selectedSkin;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
      PersistentData.PD = this;
    }
public void SkinStringToData(string skinsIn)
    {
       for(int i = 0; i < skinsIn.Length; i++)
        {
            if(int.Parse(skinsIn[i].ToString()) > 0)
            {
                allSkins[i] = true;
            }
            else
            {
                allSkins[i] = false;
            }
        }
        MenuController.MC.SetUpStore();
    }

    public string SkinDataToString()
    {
        string toString = "";
        for(int i = 0; i < allSkins.Length; i++)
        {
            if(allSkins[i]== true)
            {
                toString += "1";
            }
            else
            {
                toString += "0";
            }
        }
        return toString;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

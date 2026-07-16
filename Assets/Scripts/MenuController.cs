using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public static MenuController MC;
    public GameObject ShopPanel;
    public GameObject[] lockedItems;
    public Button[] unlockedItems;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
      MenuController.MC = this;  
    }
    void Start()
    {
       SetUpStore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpStore()
    {
        for(int i = 0; i < PersistentData.PD.allSkins.Length; i++)
        {
           lockedItems[i].SetActive(!PersistentData.PD.allSkins[i]);
           unlockedItems[i].interactable = PersistentData.PD.allSkins[i];
        }
    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }

    public void UnclockSkin(int skinIndex)
    {
        PersistentData.PD.allSkins[skinIndex] = true;
        PlayFabController.PFC.SetUserData( PersistentData.PD.SkinDataToString());
        SetUpStore();
    }

    public void SelectSkin(int skinIndex)
    {
        PersistentData.PD.selectedSkin = skinIndex;
    }
}

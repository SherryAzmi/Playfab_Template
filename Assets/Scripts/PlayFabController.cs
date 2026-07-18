using System;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.Json;
using System.Collections;
public class PlayFabController : MonoBehaviour
{

public static PlayFabController PFC;
    private string userEmail;
    private string userPassword;
    private string userName;   
    private string myID; 
    public GameObject loginPanel;
      public GameObject addLoginPanel;
        public GameObject recoveryButton;

void DisplayPlayFabError(PlayFabError error)
 {
    Debug.LogError(error.GenerateErrorReport());
 }

void DisplayError(string error)
{
    Debug.LogError(error);
}

        private void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if (PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "1995C5"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
       // PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

//PlayerPrefs.DeleteAll();
if(PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");

  var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
            
         #if UNITY_ANDROID
         var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID() , CreateAccount = true};
         PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
         #endif 

          #if UNITY_IOS
         var requestIOS = new LoginWithIOSDeviceIDRequest { IOSDeviceId = ReturnMobileID() , CreateAccount = true};
         PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
         #endif   
        }
     
    }
#region Login
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
            PlayerPrefs.SetString("EMAIL", userEmail);
    PlayerPrefs.SetString("PASSWORD", userPassword);
     GetStatistics();
    loginPanel.SetActive(false);
    recoveryButton.SetActive(false);

    myID = result.PlayFabId;
    GetPlayerData();
    }
    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
         GetStatistics();
    loginPanel.SetActive(false);
    myID = result.PlayFabId;
    GetPlayerData();
    }
    
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    PlayerPrefs.SetString("EMAIL", userEmail);
    PlayerPrefs.SetString("PASSWORD", userPassword);

PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = userName }, OnDisplayNameUpdate, OnLoginMobileFailure);

     GetStatistics();
    loginPanel.SetActive(false);
    myID = result.PlayFabId;
    GetPlayerData();
    }


void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display name updated successfully");
    }
    private void OnLoginFailure(PlayFabError error)
    {
     var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }
        private void OnLoginMobileFailure(PlayFabError error)
    {
             DisplayPlayFabError(error);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
      
        Debug.Log(error.GenerateErrorReport());
    }
    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void GetUserName(string nameIn)
    {
        userName = nameIn;
    }

    public void OnClickLoginButton()
    {
            Debug.Log($"Login attempt - Email='{userEmail}' Password='{userPassword}'");

        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public static string ReturnMobileID()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        return deviceId;
    }

     public void OpenAddLogin()
    {
        
        addLoginPanel.SetActive(true);
    }

    
     public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, DisplayPlayFabError);
    }

        private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    PlayerPrefs.SetString("EMAIL", userEmail);
    PlayerPrefs.SetString("PASSWORD", userPassword);
     GetStatistics();
    addLoginPanel.SetActive(false);
    }
    #endregion Login


public int playerLevel;
public int gameLevel;
public int playerHealth;
public int playerDamage;
public int playerHighScore;

#region PlayerStats
public void SetStats()
    {
        
PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
    // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
    Statistics = new List<StatisticUpdate> {
        new StatisticUpdate { StatisticName = "playerLevel", Value = playerLevel },
        new StatisticUpdate { StatisticName = "gameLevel", Value = gameLevel },
        new StatisticUpdate { StatisticName = "playerHealth", Value = playerHealth },
        new StatisticUpdate { StatisticName = "playerDamage", Value = playerDamage },
        new StatisticUpdate { StatisticName = "playerHighScore", Value = playerHighScore },
    }
},
result => { Debug.Log("User statistics updated"); },
error => { Debug.LogError(error.GenerateErrorReport()); });
    }

void GetStatistics()
{
    PlayFabClientAPI.GetPlayerStatistics(
        new GetPlayerStatisticsRequest(),
        OnGetStatistics,
        error => Debug.LogError(error.GenerateErrorReport())
    );
}

void OnGetStatistics(GetPlayerStatisticsResult result)
{
    Debug.Log("Received the following Statistics:");
    foreach (var eachStat in result.Statistics)
    {
        Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
        switch (eachStat.StatisticName)
        {
            case "playerLevel":
                playerLevel = eachStat.Value;
                break;
            case "gameLevel":
                gameLevel = eachStat.Value;
                break;
            case "playerHealth":
                playerHealth = eachStat.Value;
                break;
            case "playerDamage":
                playerDamage = eachStat.Value;
                break;
            case "playerHighScore":
                playerHighScore = eachStat.Value;
                break;
        }
    }
}
// Build the request object and access the API
public void StartCloudUpdatePlayerStats()
{
    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
    {
        FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
        FunctionParameter = new { level = playerLevel , PlayerHighScore = playerHighScore ,playerHealth = playerHealth }, // The parameter provided to your function
        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
    }, OnCloudUpdatePlayerStats, OnErrorShared);
}
// OnCloudUpdatePlayerStats defined in the next code block

private static void OnCloudUpdatePlayerStats(ExecuteCloudScriptResult result) {
    // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
    Debug.Log(PlayFabSimpleJson.SerializeObject(result.FunctionResult));
    JsonObject jsonResult = (JsonObject)result.FunctionResult;
    object messageValue;
    jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
    Debug.Log((string)messageValue);
}

private void OnErrorShared(PlayFabError error)
{
    Debug.Log(error.GenerateErrorReport());
}



#endregion PlayerStats
public GameObject leaderboardPanel;
public GameObject leaderboardListingPrefab;
public Transform leaderboardContainer;
#region playerLeaderboard
public void GetLeaderboarder()
{
    var requestLeaderboard = new GetLeaderboardRequest { StatisticName = "playerHighScore", StartPosition = 0, MaxResultsCount = 10 };
    PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
}

public void OnGetLeaderboard(GetLeaderboardResult result)
{
    leaderboardPanel.SetActive(true);
    Debug.Log("Leaderboard:");
    foreach (var item in result.Leaderboard)
    {
        GameObject listingObj = Instantiate(leaderboardListingPrefab, leaderboardContainer);
        LeaderboardListing listing = listingObj.GetComponent<LeaderboardListing>();
        listing.playerName.text = item.DisplayName;
        listing.playerScore.text = item.StatValue.ToString();
        Debug.Log(string.Format("Position: {0} | PlayFabId: {1} | DisplayName: {2} | StatValue: {3}", item.Position, item.PlayFabId, item.DisplayName, item.StatValue));
    }
}

public void OnErrorLeaderboard(PlayFabError error)
{
    Debug.Log(error.GenerateErrorReport());
}


public void CloseLeaderboard()
{
    leaderboardPanel.SetActive(false);
    for (int i = leaderboardContainer.childCount - 1; i >= 0; i--)
    {
        Destroy(leaderboardContainer.GetChild(i).gameObject);
    }
}



#endregion
#region PlayerData
public void GetPlayerData()
{
    PlayFabClientAPI.GetUserData(new GetUserDataRequest()
    {
        PlayFabId = myID,
        Keys = null 
    }, UserDataSuccess, OnErrorLeaderboard);
}

 void UserDataSuccess(GetUserDataResult result)
{
    if (result.Data == null || !result.Data.ContainsKey("Skins"))
    {
        Debug.Log("No user data available");
        
    }
    else
    {
        Debug.Log("User data retrieved successfully");
        PersistentData.PD.SkinStringToData(result.Data["Skins"].Value);
    }

    
}

public void SetUserData(string skinData)
{

PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()  
 {
        Data = new Dictionary<string, string> {
            {"Skins", skinData}
        }
    }, SetDataSuccess,OnErrorLeaderboard);}

    void SetDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log(result.DataVersion);
    }
#endregion
#region FriendList

[SerializeField] Transform friendScrollView;
List<FriendInfo> myFriends;

void DisplayFriends(List<FriendInfo> friendCache)
{
    foreach (FriendInfo f in friendCache)
    {
        bool isFound = false;
        if(myFriends != null)
            {
                foreach(FriendInfo g in myFriends)
            {
                if(f.FriendPlayFabId == g.FriendPlayFabId)
                
                    isFound = true;
                 
                } 
            }
       
                if(isFound == false)
                {
                        GameObject listing = Instantiate(leaderboardListingPrefab, friendScrollView);
        LeaderboardListing tempListing = listing.GetComponent<LeaderboardListing>();
        // Debug.Log(tempListing.playerName);
        // Debug.Log(f.TitleDisplayName);
        tempListing.playerName.text = f.TitleDisplayName;
                }
            
    
    }
    myFriends = friendCache;
}

IEnumerator waitForFriends()
{
    yield return new WaitForSeconds(2f);
    GetFriends();
}

public void RunWaitForFriends()
{
    StartCoroutine(waitForFriends());
}
public void InputDriendID(string friendId)
{
    friendSearch = friendId;
}

public void SubmitFriendRequest()
{
    AddFriend(FriendIdType.PlayFabId, friendSearch);
}

public void OpenCloseFriend()
{
    FriendPanel.SetActive(!FriendPanel.activeInHierarchy);
}
List<FriendInfo> _friends = null;

public void GetFriends()
{
    PlayFabClientAPI.GetFriendsList(
        new GetFriendsListRequest(),
        result =>
        {
            _friends = result.Friends;
            DisplayFriends(_friends);
        },
        DisplayPlayFabError
    );
}

string friendSearch;
[SerializeField] GameObject FriendPanel;

enum FriendIdType { PlayFabId, Username, Email, DisplayName };

void AddFriend(FriendIdType idType, string friendId) {
    var request = new AddFriendRequest();
    switch (idType) {
        case FriendIdType.PlayFabId:
            request.FriendPlayFabId = friendId;
            break;
        case FriendIdType.Username:
            request.FriendUsername = friendId;
            break;
        case FriendIdType.Email:
            request.FriendEmail = friendId;
            break;
        case FriendIdType.DisplayName:
            request.FriendTitleDisplayName = friendId;
            break;
    }
    // Execute request and update friends when we are done
    PlayFabClientAPI.AddFriend(request, result => {
        Debug.Log("Friend added successfully!");
    }, DisplayPlayFabError);
}
#endregion


}
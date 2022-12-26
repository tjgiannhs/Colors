using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;


public class AnalyticsManager : MonoBehaviour
{
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
        }
        catch (ConsentCheckException e)
        {
          // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
        }
    }   


    public void reportGamesLastSession(int n) 
    {
        if(n==0) return;
        // Send custom event
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "totalGames", n },
        };

        // The ‘myEvent’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("noOfGamesLastSession", parameters); 

        // Optional - You can call Events.Flush() to send the event immediately
        try{
            AnalyticsService.Instance.Flush();
        }catch{}
        
        //Legacy analytics
        /*
        Analytics.CustomEvent("Achievement earned", new Dictionary<string, object> {
            { "Name", achievementName},
            { "OS and Version", SystemInfo.operatingSystem },
            { "Playthrough time", PlayerPrefs.GetInt("Playthrough time", 0) + (int)Time.timeSinceLevelLoad / 60 }
        });       
        */
    }
    public void reportAdsLastSession(int n) 
    {
        if(n==0) return;
        // Send custom event
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "totalAds", n },
        };

        // The ‘myEvent’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("noOfAdsLastSession", parameters); 

        // Optional - You can call Events.Flush() to send the event immediately
        try{
            AnalyticsService.Instance.Flush();
        }catch{}
    }

}

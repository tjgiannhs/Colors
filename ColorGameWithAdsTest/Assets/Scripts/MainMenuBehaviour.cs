using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Lean.Transition;
using Lean.Gui;
using UnityEngine.UI;
using System.Collections;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] GameObject adsManager;
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] GameObject exitGamePopup;
    [SerializeField] GameObject boardObject;
    [SerializeField] GameObject colorblindButton;
    [SerializeField] GameObject musicButton;
    [SerializeField] GameObject soundsButton;
    [SerializeField] GameObject vibrationButton;
    [SerializeField] GameObject FTUEinstructions;
    [SerializeField] TextMeshProUGUI highscoreTextMainMenu;
    [SerializeField] Sprite checkImage;
    [SerializeField] Sprite crossImage;
    bool atMenuScene = true;
    bool bannerShown = false;

    // Start is called before the first frame update
    void Start()
    {
        colorblindButton.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("ColorblindOn",0)>0.5?"ON":"OFF";
        musicButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("MusicOn",1)>0.5f?checkImage:crossImage;
        musicButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("MusicOn",1)>0.5f?checkImage:crossImage;
        soundsButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundsOn",1)>0.5f?checkImage:crossImage;
        soundsButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundsOn",1)>0.5f?checkImage:crossImage;
        vibrationButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("VibrationOn",1)>0.5f?checkImage:crossImage;
        vibrationButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("VibrationOn",1)>0.5f?checkImage:crossImage;
        highscoreTextMainMenu.text = PlayerPrefs.GetInt("Highscore",0)+"";
        adsManager.GetComponent<BannerAd>().LoadBanner();
        Vibration.Init();
        //GetComponent<LeanManualAnimation>().BeginTransitions();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(atMenuScene){
                exitGamePopup.GetComponent<LeanWindow>().TurnOn();
            }
            else {
                LoadMenuScene();
            }
        }
    }

    public void PlayBigVibration()
    {
        #if UNITY_IPHONE
            Vibration.VibratePeek();    
        #endif
            
        #if UNITY_ANDROID
            Vibration.Vibrate (60);
        #endif
    }
    public void PlaySmallVibration()
    {
        #if UNITY_IPHONE
            Vibration.VibratePop();    
        #endif

        #if UNITY_ANDROID
            Vibration.Vibrate (5);
        #endif
    }
    public void LoadMenuScene()
    {
        try{
            adsManager.GetComponent<BannerAd>().HideBannerAd();
        }catch{}
        bannerShown = false;
        GetComponent<LeanManualAnimation>().BeginTransitions(); ChangeAtMenuScene(true);
    }
    public void DeleteHighscore()
    {
        PlayerPrefs.SetInt("Highscore",0);
    }
    public void ChangeAtMenuScene(bool b)
    {
        atMenuScene = b;
        if(b){
            highscoreTextMainMenu.text = PlayerPrefs.GetInt("Highscore",0)+"";
            mainMenuCanvas.sortingOrder = 5;
            StopCoroutine("PutMainMenuBehind");
        }
        else {
            StartCoroutine(ShowBannerAd());
            StartCoroutine("PutMainMenuBehind");
        }
        //print(atMenuScene);
    }

    IEnumerator ShowBannerAd()
    {
        if(!atMenuScene)
        {
            try
            {
                adsManager.GetComponent<BannerAd>().ShowBannerAd();
                bannerShown = true;
            }catch
            {
                bannerShown = false;
            }  
        }
        yield return new WaitForSeconds(0.5f);

        if(!bannerShown)
            StartCoroutine(ShowBannerAd());
    }

    IEnumerator PutMainMenuBehind()
    {
        if(PlayerPrefs.GetInt("Highscore",0) == 0 && boardObject.GetComponent<ColorMerging>().GetMovesPlayed()==0) FTUEinstructions.GetComponent<LeanWindow>().On = true;
        yield return new WaitForSeconds(0.2f);//the time for its disappearance animation to play
        mainMenuCanvas.sortingOrder = -10;
    }
    public void OnTwitterLinkClicked() { Application.OpenURL("https://twitter.com/gggamesdev"); }
    public void OnItchioLinkClicked() { Application.OpenURL("https://gg-undroid-games.itch.io/"); }
    public void OnGooglePlayLinkClicked() { Application.OpenURL("https://play.google.com/store/apps/dev?id=5698010272073934079"); }
    public void OnLinktreeLinkClicked() { Application.OpenURL("https://linktr.ee/ggundroidgames"); }

    public void OnApplicationClickQuit() { Application.Quit();}
    public void ClosePopupWindow() {} //transform.parent.parent.GetComponent<Lean.Gui.LeanWindow>().TurnOff();}

    public void OpenSettingsPopupWindow(GameObject settingsPopUpWindow){} //settingsPopUpWindow.GetComponent<Lean.Gui.LeanWindow>().TurnOn();}
    public void ChangeSoundSetting()
    {
        PlayerPrefs.SetInt("SoundsOn",1-PlayerPrefs.GetInt("SoundsOn",1)); 
        soundsButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundsOn",1)>0.5f?checkImage:crossImage;
        soundsButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("SoundsOn",1)>0.5f?checkImage:crossImage;
    }
    public void ChangeMusicSetting()
    {
        PlayerPrefs.SetInt("MusicOn",1-PlayerPrefs.GetInt("MusicOn",1)); 
        musicButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("MusicOn",1)>0.5f?checkImage:crossImage;
        musicButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("MusicOn",1)>0.5f?checkImage:crossImage;
    }
    public void ChangeVibrationSetting()
    {
        PlayerPrefs.SetInt("VibrationOn",1-PlayerPrefs.GetInt("VibrationOn",1));
        vibrationButton.transform.GetChild(1).GetComponent<Image>().sprite = PlayerPrefs.GetInt("VibrationOn",1)>0.5f?checkImage:crossImage;
        vibrationButton.transform.GetChild(2).GetComponent<Image>().sprite = PlayerPrefs.GetInt("VibrationOn",1)>0.5f?checkImage:crossImage;
    }

    public void ChangeColorBlindSetting()
    {
        PlayerPrefs.SetInt("ColorblindOn",1-PlayerPrefs.GetInt("ColorblindOn",0));

        colorblindButton.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("ColorblindOn",0)>0.5?"ON":"OFF";

        boardObject.GetComponent<ColorMerging>().OnColorblindSettingChange();
    }

    public void ToMainMenuFromPause()
    {
        transform.parent.parent.parent.parent.GetChild(0).GetChild(2).gameObject.SetActive(false);//making sure to prevent the bug that hid the main menu items for no reason
        transform.parent.parent.parent.parent.GetChild(0).GetChild(3).gameObject.SetActive(false);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(4).gameObject.SetActive(false);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(6).gameObject.SetActive(false);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(7).gameObject.SetActive(false);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(8).gameObject.SetActive(false);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(9).gameObject.SetActive(false);

        transform.parent.parent.parent.parent.GetChild(0).GetChild(2).gameObject.SetActive(true);//making sure to prevent the bug that hid the main menu items for no reason
        transform.parent.parent.parent.parent.GetChild(0).GetChild(3).gameObject.SetActive(true);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(4).gameObject.SetActive(true);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(6).gameObject.SetActive(true);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(7).gameObject.SetActive(true);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(8).gameObject.SetActive(true);
        transform.parent.parent.parent.parent.GetChild(0).GetChild(9).gameObject.SetActive(true);
        
        transform.parent.parent.parent.parent.GetComponent<Animator>().SetTrigger("LoadNewScene");//parent with animation
        transform.parent.gameObject.SetActive(false);
    }

    public void DeactivateMeOnClick()
    {
        transform.gameObject.SetActive(false);
    }

    public void OnClickRateUs()
    {
        Application.OpenURL("market://details?id=" + "GGUndoirdGames.Infinitoggle");
    }

}

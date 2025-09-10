using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Paytable Popup")]
    [SerializeField]
    private Button Info_button;
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private TMP_Text[] SymbolsText;
    [SerializeField]
    private TMP_Text[] KTRSymbolsText;
    [SerializeField]
    private TMP_Text KTR_Text;
    [SerializeField]
    private TMP_Text Scatter_Text;
    [SerializeField]
    private TMP_Text TFC_Text;
    [SerializeField]
    private TMP_Text Wild_Text;
    [SerializeField]
    private Button Right_Button;
    [SerializeField]
    private Button Left_Button;
    [SerializeField]
    private GameObject[] Info_Screens;
    int screenCounter = 0;
    [SerializeField]
    private Button lastQuit_Button;
    [SerializeField]
    private Button lastArrLeft_Button;
    [SerializeField]
    private Button lastArrRight_Button;

    [Header("Settings Popup")]
    [SerializeField]
    private GameObject SettingsPopup_Object;
    [SerializeField]
    private Button SettingsExit_Button;
    [SerializeField]
    private Button Sound_Button;
    [SerializeField]
    private Button Music_Button;
    [SerializeField]
    private Button Settings_Button;

    [SerializeField]
    private GameObject MusicOn_Object;
    [SerializeField]
    private GameObject MusicOff_Object;
    [SerializeField]
    private GameObject SoundOn_Object;
    [SerializeField]
    private GameObject SoundOff_Object;

    [Header("Win Popup")]
    [SerializeField]
    private Sprite BigWin_Sprite;
    [SerializeField]
    private Sprite HugeWin_Sprite;
    [SerializeField]
    private Sprite MegaWin_Sprite;
    [SerializeField]
    private Image Win_Image;
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;

    [Header("Free Spin Popup")]
    [SerializeField]
    private GameObject FreeSpinPopup_Object;
    [SerializeField]
    private TMP_Text FS_Text;
    [SerializeField]
    private Image FS_Image;
    [SerializeField]
    private Image FSTitle_Image;
    internal bool isFreeSpinPopupActive = false;
    [SerializeField] private Button SkipFreeSpinStart_Button;

    [Header("Free Spin Complete Popup")]
    [SerializeField]
    private GameObject FreeSpinCompletePopup_Object;
    [SerializeField]
    private TMP_Text FSComplete_Text;
    [SerializeField]
    private TMP_Text FSNum_Text;
    [SerializeField]
    private Image FSComplete_Image;
    [SerializeField] private Button SkipFreeSpinComplete_Button;

    [Header("Splash Screen")]
    [SerializeField]
    private GameObject Loading_Object;
    [SerializeField]
    private Image Loading_Image;
    [SerializeField]
    private TMP_Text Loading_Text;
    [SerializeField]
    private TMP_Text LoadPercent_Text;
    [SerializeField]
    private Button QuitSplash_button;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField]
    private Button CloseAD_Button;
    [SerializeField]
    private GameObject ADPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private TMP_Text reconnect_Text;
    [SerializeField]
    private GameObject ReconnectPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;
    [SerializeField]
    private Button CrossQuit_Button;

    [Header("Squirrel Animation")]
    [SerializeField]
    private GameObject SquirrelStanding_Object;
    [SerializeField]
    private GameObject SquirrelCoins_Object;

    [Header("KTR Elements")]
    [SerializeField]
    private GameObject SlotMainKTR_Object;
    [SerializeField]
    private GameObject FGSetupKTR_Object;
    [SerializeField]
    private GameObject ButtonSetupKTR_Object;
    [SerializeField]
    private TMP_Text TotalBetKTR_Text;
    [SerializeField]
    private TMP_Text BonusWinKTR_Text;
    [SerializeField]
    private TMP_Text FreeSpinKTR_Text;
    [SerializeField]
    private GameObject SlotBgSetupKTR_Object;

    [Header("Normal Elements")]
    [SerializeField]
    private GameObject SlotMain_Object;
    [SerializeField]
    private GameObject FGSetup_Object;
    [SerializeField]
    private GameObject ButtonSetup_Object;
    [SerializeField]
    private GameObject SlotBGSetup_Object;

    [Header("Locker Setup")]
    [SerializeField]
    private GameObject LockerMain_Object;
    [SerializeField]
    private GameObject LockerOpen_Object;
    [SerializeField]
    private GameObject LockerClose_Object;

    [Header("Bonus Routine Text")]
    [SerializeField]
    private TMP_Text BonusRoutine_Text;
    [SerializeField]
    private GameObject BonusRoutine_GameObject;
    private Tween BonusTextTween = null;

    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private Button GameExit_Button;

    [SerializeField]
    private SlotBehaviour slotManager;

    [SerializeField]
    private SocketIOManager socketManager;

    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    internal double BonusWin = 0;

    internal int FreeSpins = 0;

    internal bool FSPopUpActive;
    private Tween WinPopupTextTween;
    private Tween ClosePopupTween;

    [SerializeField] private Button SkipWinAnimation;

    private void Awake()
    {
        //if (Loading_Object) Loading_Object.SetActive(true);
        //StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        StartCoroutine(LoadingTextAnimate());
        float imageFill = 0f;
        DOTween.To(() => imageFill, (val) => imageFill = val, 0.7f, 2f).OnUpdate(() =>
        {
            if (Loading_Image) Loading_Image.fillAmount = imageFill;
            if (LoadPercent_Text) LoadPercent_Text.text = (100 * imageFill).ToString("f0") + "%";
        });
        yield return new WaitForSecondsRealtime(2);
        yield return new WaitUntil(() => socketManager.isLoaded);
        DOTween.To(() => imageFill, (val) => imageFill = val, 1, 1f).OnUpdate(() =>
        {
            if (Loading_Image) Loading_Image.fillAmount = imageFill;
            if (LoadPercent_Text) LoadPercent_Text.text = (100 * imageFill).ToString("f0") + "%";
        });
        yield return new WaitForSecondsRealtime(1f);
        if (Loading_Object) Loading_Object.SetActive(false);
        StopCoroutine(LoadingTextAnimate());
    }

    private IEnumerator LoadingTextAnimate()
    {
        while (true)
        {
            if (Loading_Text) Loading_Text.text = "Loading.";
            yield return new WaitForSeconds(1f);
            if (Loading_Text) Loading_Text.text = "Loading..";
            yield return new WaitForSeconds(1f);
            if (Loading_Text) Loading_Text.text = "Loading...";
            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        if (Info_button) Info_button.onClick.RemoveAllListeners();
        if (Info_button) Info_button.onClick.AddListener(delegate { screenCounter = 1; ChangePage(false); OpenPopup(PaytablePopup_Object); });

        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (Right_Button) Right_Button.onClick.RemoveAllListeners();
        if (Right_Button) Right_Button.onClick.AddListener(delegate { ChangePage(true); });

        if (Left_Button) Left_Button.onClick.RemoveAllListeners();
        if (Left_Button) Left_Button.onClick.AddListener(delegate { ChangePage(false); });

        if (lastQuit_Button) lastQuit_Button.onClick.RemoveAllListeners();
        if (lastQuit_Button) lastQuit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (lastArrRight_Button) lastArrRight_Button.onClick.RemoveAllListeners();
        if (lastArrRight_Button) lastArrRight_Button.onClick.AddListener(delegate { ChangePage(true); });

        if (lastArrLeft_Button) lastArrLeft_Button.onClick.RemoveAllListeners();
        if (lastArrLeft_Button) lastArrLeft_Button.onClick.AddListener(delegate { ChangePage(false); });

        if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
        if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); });

        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

        if (MusicOn_Object) MusicOn_Object.SetActive(true);
        if (MusicOff_Object) MusicOff_Object.SetActive(false);

        if (SoundOn_Object) SoundOn_Object.SetActive(true);
        if (SoundOff_Object) SoundOff_Object.SetActive(false);

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        if (QuitSplash_button) QuitSplash_button.onClick.RemoveAllListeners();
        if (QuitSplash_button) QuitSplash_button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (audioController) audioController.ToggleMute(false);
        SkipFreeSpinComplete_Button.onClick.RemoveAllListeners();
        if (SkipFreeSpinComplete_Button) SkipFreeSpinComplete_Button.onClick.AddListener(skipFreeSpinPopup);

        SkipFreeSpinStart_Button.onClick.RemoveAllListeners();
        if (SkipFreeSpinStart_Button) SkipFreeSpinStart_Button.onClick.AddListener(skipFreeSpinPopup);

        isMusic = true;
        isSound = true;

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

        if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);


    }

    private void ChangePage(bool Increment)
    {
        foreach (GameObject t in Info_Screens)
        {
            t.SetActive(false);
        }

        if (Increment)
        {
            if (screenCounter == Info_Screens.Length - 1)
            {
                screenCounter = 0;
            }
            else
            {
                screenCounter++;
            }
        }
        else
        {
            if (screenCounter == 0)
            {
                screenCounter = Info_Screens.Length - 1;
            }
            else
            {
                screenCounter--;
            }
        }
        Info_Screens[screenCounter].SetActive(true);
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }

    internal void PopulateWin(int value, double amount)
    {
        if (audioController) audioController.PlayWLAudio("megaWin");
        switch (value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;
        }

        StartPopupAnim(amount);
    }


    internal void FreeSpinProcessStart(int spins, double totalbet, bool isUpdate = false)
    {
        FSPopUpActive = true;
        int ExtraSpins = spins - FreeSpins;
        FreeSpins = spins;
        if (isUpdate)
        {
            ToggleKTR(true);
            if (TotalBetKTR_Text) TotalBetKTR_Text.text = totalbet.ToString();
            if (BonusWinKTR_Text) BonusWinKTR_Text.text = "0.00";
        }
        if (FreeSpinKTR_Text) FreeSpinKTR_Text.text = spins.ToString();
        StartCoroutine(ShowPopupProcess(ExtraSpins, isUpdate));


    }

    private void skipFreeSpinPopup()
    {
        isFreeSpinPopupActive = false;
    }

    internal void UpdateUI(int freeSpins, double CurrentWin)
    {
        BonusWin += CurrentWin;
        if (FreeSpinKTR_Text) FreeSpinKTR_Text.text = freeSpins.ToString();
        if (BonusWinKTR_Text) BonusWinKTR_Text.text = CurrentWin.ToString("f3");
    }

    void SkipWin()
    {
        Debug.Log("Skip win called");
        if (ClosePopupTween != null)
        {
            ClosePopupTween.Kill();
            ClosePopupTween = null;
        }
        if (WinPopupTextTween != null)
        {
            WinPopupTextTween.Kill();
            WinPopupTextTween = null;
        }
        ClosePopup(WinPopup_Object);
        slotManager.CheckPopups = false;
    }

    internal IEnumerator FreeSpinProcessStop()
    {
        Debug.Log($"#$############# free sppin process stop called");
        isFreeSpinPopupActive = true;
        FSPopUpActive = true;
        if (FSComplete_Image) FSComplete_Image.color = new Color(FSComplete_Image.color.r, FSComplete_Image.color.g, FSComplete_Image.color.b, 0f);
        if (FSComplete_Text) FSComplete_Text.color = new Color(FSComplete_Text.color.r, FSComplete_Text.color.g, FSComplete_Text.color.b, 0f);
        if (FSNum_Text) FSNum_Text.color = new Color(FSNum_Text.color.r, FSNum_Text.color.g, FSNum_Text.color.b, 0f);
        if (FSNum_Text) FSNum_Text.text = BonusWin.ToString("f3");  //BonusWinKTR_Text.text;
        if (FreeSpinCompletePopup_Object) FreeSpinCompletePopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (FSComplete_Image) FSComplete_Image.DOFade(1f, 1f);
        if (FSComplete_Text) FSComplete_Text.DOFade(1f, 1f);
        if (FSNum_Text) FSNum_Text.DOFade(1f, 1f);
        DOVirtual.DelayedCall(2.5f, () =>
        {
        isFreeSpinPopupActive = false;
        });


        // DOVirtual.DelayedCall(2.5f, () =>
        // {
        yield return new WaitUntil(() => !isFreeSpinPopupActive);
        if (FreeSpinCompletePopup_Object) FreeSpinCompletePopup_Object.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        ToggleKTR(false);

        if (audioController) audioController.SwitchBGSound(false);
        FSPopUpActive = false;
        BonusWin = 0;

        // });
    }

    internal void ToggleBonusRText(bool isActive)
    {
        if (isActive)
        {
            if (BonusRoutine_GameObject) BonusRoutine_GameObject.SetActive(true);
            if (BonusRoutine_Text) BonusTextTween = BonusRoutine_Text.DOFade(0f, 2f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            BonusTextTween.Kill();
            if (BonusRoutine_GameObject) BonusRoutine_GameObject.SetActive(false);
        }
    }

    private IEnumerator ShowPopupProcess(int freeSpins, bool isBegin)
    {
        float time = 3f;
        isFreeSpinPopupActive = true;
        if (isBegin)
        {
            time = 5f;
        }
        if (!isBegin)
            FS_Text.text = $"You have been awarded with extra <size=100><color=green>{freeSpins}</color></size> free spins.";
        else
            FS_Text.text = $"You have been awarded with <size=100><color=green>{freeSpins}</color></size> free spins.";

        if (FS_Image) FS_Image.color = FS_Image.color = new Color(FS_Image.color.r, FS_Image.color.g, FS_Image.color.b, 1f);
        if (FSTitle_Image) FSTitle_Image.color = FSTitle_Image.color = new Color(FSTitle_Image.color.r, FSTitle_Image.color.g, FSTitle_Image.color.b, 1f);
        if (FS_Text) FS_Text.color = FS_Text.color = new Color(FS_Text.color.r, FS_Text.color.g, FS_Text.color.b, 1f);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(true);
        DOVirtual.DelayedCall(time, () =>
        {
            isFreeSpinPopupActive = false;
        });
        yield return new WaitUntil(() => !isFreeSpinPopupActive);
        // DOVirtual.DelayedCall(time, () =>
        // {
        if (FS_Image) FS_Image.DOFade(0f, 1f).OnComplete(delegate
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
            if (FreeSpinPopup_Object) FreeSpinPopup_Object.SetActive(false);
        });
        if (FSTitle_Image) FSTitle_Image.DOFade(0f, 1f);
        if (FS_Text) FS_Text.DOFade(0f, 1f).OnComplete(() =>
        {
            FSPopUpActive = false;
        });

        // slotManager.FreeSpin(freeSpins);

        // });
    }

    private void ToggleKTR(bool isActive)
    {
        if (audioController) audioController.PlayBonusAudio("vault");
        if (isActive)
        {
            if (LockerMain_Object) LockerMain_Object.SetActive(true);
            if (LockerOpen_Object) LockerOpen_Object.SetActive(true);
            if (SlotMainKTR_Object) SlotMainKTR_Object.SetActive(true);
            if (FGSetupKTR_Object) FGSetupKTR_Object.SetActive(true);
            if (SlotBgSetupKTR_Object) SlotBgSetupKTR_Object.SetActive(true);
            if (ButtonSetupKTR_Object) ButtonSetupKTR_Object.SetActive(true);
            if (SlotMain_Object) SlotMain_Object.SetActive(false);
            if (FGSetup_Object) FGSetup_Object.SetActive(false);
            if (ButtonSetup_Object) ButtonSetup_Object.SetActive(false);
            if (SlotBGSetup_Object) SlotBGSetup_Object.SetActive(false);
            DOVirtual.DelayedCall(2f, () =>
            {
                if (LockerOpen_Object) LockerOpen_Object.SetActive(false);
                if (LockerMain_Object) LockerMain_Object.SetActive(false);
            });
        }
        else
        {
            if (LockerMain_Object) LockerMain_Object.SetActive(true);
            if (LockerClose_Object) LockerClose_Object.SetActive(true);
            if (SlotMainKTR_Object) SlotMainKTR_Object.SetActive(false);
            if (FGSetupKTR_Object) FGSetupKTR_Object.SetActive(false);
            if (ButtonSetupKTR_Object) ButtonSetupKTR_Object.SetActive(false);
            if (SlotMain_Object) SlotMain_Object.SetActive(true);
            if (FGSetup_Object) FGSetup_Object.SetActive(true);
            if (ButtonSetup_Object) ButtonSetup_Object.SetActive(true);
            if (SlotBGSetup_Object) SlotBGSetup_Object.SetActive(true);
            if (SlotBgSetupKTR_Object) SlotBgSetupKTR_Object.SetActive(false);
            DOVirtual.DelayedCall(2f, () =>
            {
                if (LockerClose_Object) LockerClose_Object.SetActive(false);
                if (LockerMain_Object) LockerMain_Object.SetActive(false);
            });
        }
    }

    private void StartPopupAnim(double amount)
    {
        double initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        DOTween.To(() => initAmount, (val) => initAmount = val, amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("f3");
        });

        ClosePopupTween = DOVirtual.DelayedCall(6f, () =>
        {
            ClosePopup(WinPopup_Object);
            slotManager.CheckPopups = false;
        });
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }

    internal void InitialiseUIData(Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }

    internal void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            string text = null;
            if (paylines.symbols[i].multiplier[0] != 0)
            {
                text += "5x = " + paylines.symbols[i].multiplier[0] * socketManager.initialData.bets[slotManager.BetCounter];
            }
            if (paylines.symbols[i].multiplier[1] != 0)
            {
                text += "\n4x = " + paylines.symbols[i].multiplier[1] * socketManager.initialData.bets[slotManager.BetCounter];
            }
            if (paylines.symbols[i].multiplier[2] != 0)
            {
                text += "\n3x = " + paylines.symbols[i].multiplier[2] * socketManager.initialData.bets[slotManager.BetCounter];
            }
            if (SymbolsText[i]) SymbolsText[i].text = text;
            if (KTRSymbolsText[i]) KTRSymbolsText[i].text = text;
        }

        for (int i = 0; i < paylines.symbols.Count; i++)
        {
            if (paylines.symbols[i].name.ToUpper() == "FREESPIN")
            {
                if (KTR_Text) KTR_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "SCATTER")
            {
                if (Scatter_Text) Scatter_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "BONUS")
            {
                if (TFC_Text) TFC_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "WILD")
            {
                if (Wild_Text) Wild_Text.text = paylines.symbols[i].description.ToString();
            }
        }
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    private void ToggleMusic()
    {
        isMusic = !isMusic;
        if (isMusic)
        {
            if (MusicOn_Object) MusicOn_Object.SetActive(true);
            if (MusicOff_Object) MusicOff_Object.SetActive(false);
            audioController.ToggleMute(false, "bg");
        }
        else
        {
            if (MusicOn_Object) MusicOn_Object.SetActive(false);
            if (MusicOff_Object) MusicOff_Object.SetActive(true);
            audioController.ToggleMute(true, "bg");
        }
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private void ToggleSound()
    {
        isSound = !isSound;
        if (isSound)
        {
            if (SoundOn_Object) SoundOn_Object.SetActive(true);
            if (SoundOff_Object) SoundOff_Object.SetActive(false);
            if (audioController) audioController.ToggleMute(false, "button");
            if (audioController) audioController.ToggleMute(false, "wl");
        }
        else
        {
            if (SoundOn_Object) SoundOn_Object.SetActive(false);
            if (SoundOff_Object) SoundOff_Object.SetActive(true);
            if (audioController) audioController.ToggleMute(true, "button");
            if (audioController) audioController.ToggleMute(true, "wl");
        }
    }

    internal void ToggleSquirrel(bool isActive)
    {
        if (!isActive)
        {
            if (SquirrelCoins_Object) SquirrelCoins_Object.SetActive(true);
            if (SquirrelStanding_Object) SquirrelStanding_Object.SetActive(false);
        }
        else
        {
            if (SquirrelCoins_Object) SquirrelCoins_Object.SetActive(false);
            if (SquirrelStanding_Object) SquirrelStanding_Object.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;  //images taken initially
    [SerializeField]
    private Sprite[] KTRImages;
    [SerializeField]
    private List<Sprite> Box_Sprites;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;     //class to store total images
    [SerializeField]
    private List<SlotImage> KTRimages;     //class to store total images
    [SerializeField]
    private List<SlotImage> Tempimages;     //class to store the result matrix
    [SerializeField]
    private List<SlotImage> Animimages;     //class to store the animation matrix
    [SerializeField]
    private List<BoxScript> TempBoxScripts;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;
    [SerializeField]
    private Transform[] KTRSlot_Transform;

    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button KTRStart_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button BetPlus_Button;
    [SerializeField]
    private Button BetMinus_Button;
    [SerializeField]
    private Button TBPlus_Button;
    [SerializeField]
    private Button TBMinus_Button;
    [SerializeField]
    private Button Settings_Button;



    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Shoe_Sprite;
    [SerializeField]
    private Sprite[] Fish_Sprite;
    [SerializeField]
    private Sprite[] Cheese_Sprite;
    [SerializeField]
    private Sprite[] Garlic_Sprite;
    [SerializeField]
    private Sprite[] Egg_Sprite;
    [SerializeField]
    private Sprite[] Boy_Sprite;
    [SerializeField]
    private Sprite[] LadyCoat_Sprite;
    [SerializeField]
    private Sprite[] OldMan_Sprite;
    [SerializeField]
    private Sprite[] FatLady_Sprite;
    [SerializeField]
    private Sprite[] Wild_Sprite;
    [SerializeField]
    private Sprite[] Scatter_Sprite;
    [SerializeField]
    private Sprite[] Keys_Sprite;
    [SerializeField]
    private Sprite[] Trash_Sprite;

    [Header("KTR Animated Sprites")]
    [SerializeField]
    private Sprite[] ShoeKTR_Sprite;
    [SerializeField]
    private Sprite[] FishKTR_Sprite;
    [SerializeField]
    private Sprite[] CheeseKTR_Sprite;
    [SerializeField]
    private Sprite[] GarlicKTR_Sprite;
    [SerializeField]
    private Sprite[] EggKTR_Sprite;
    [SerializeField]
    private Sprite[] BoyKTR_Sprite;
    [SerializeField]
    private Sprite[] LadyCoatKTR_Sprite;
    [SerializeField]
    private Sprite[] OldManKTR_Sprite;
    [SerializeField]
    private Sprite[] FatLadyKTR_Sprite;
    [SerializeField]
    private Sprite[] WildKTR_Sprite;
    [SerializeField]
    private Sprite[] ScatterKTR_Sprite;
    [SerializeField]
    private Sprite[] KeysKTR_Sprite;
    [SerializeField]
    private Sprite[] TrashKTR_Sprite;

    [Header("Miscellaneous UI")]
    [SerializeField]
    internal TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text LineBet_text;
    [SerializeField]
    private TMP_Text TotalWin_text;


    [Header("Audio Management")]
    [SerializeField]
    private AudioController audioController;

    [SerializeField]
    private UIManager uiManager;

    [Header("BonusGame Popup")]
    [SerializeField]
    private BonusController _bonusManager;

    [SerializeField]
    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField]
    private SocketIOManager SocketManager;

    private Coroutine BoxAnimRoutine = null;
    private Coroutine tweenroutine;

    [SerializeField] private bool IsAutoSpin = false;
    internal bool IsFreeSpin = false;
    private bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;

    internal int BetCounter = 0;
    private int FreeSpinCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    protected int Lines = 20;
    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 5;
    [Header(" for turbo and stop spin")]
    private bool IsTurboOn;

    [SerializeField] private Sprite TurboToggleSprite;
    private bool StopSpinToggle;
    [SerializeField] private Button Turbo_Button;
    [SerializeField] private Button StopSpin_Button;

    private Tween BalanceTween;

    [Header("for auto spin")]
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button AutoSpin_Button;
    private Coroutine AutoSpinRoutine = null;
    private float SpinDelay = 0.2f;
    private bool WasAutoSpinOn;

    [Header("for Free spins")]
    private Coroutine FreeSpinRoutine = null;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (BetPlus_Button) BetPlus_Button.onClick.RemoveAllListeners();
        if (BetPlus_Button) BetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });
        if (BetMinus_Button) BetMinus_Button.onClick.RemoveAllListeners();
        if (BetMinus_Button) BetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (TBPlus_Button) TBPlus_Button.onClick.RemoveAllListeners();
        if (TBPlus_Button) TBPlus_Button.onClick.AddListener(delegate { ToggleTotalBet(true); });

        if (TBMinus_Button) TBMinus_Button.onClick.RemoveAllListeners();
        if (TBMinus_Button) TBMinus_Button.onClick.AddListener(delegate { ToggleTotalBet(false); });

        if (KTRStart_Button) KTRStart_Button.onClick.RemoveAllListeners();
        if (KTRStart_Button) KTRStart_Button.onClick.AddListener(EnableFreespinPlayButton);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if (Turbo_Button) Turbo_Button.onClick.AddListener(TurboToggle);

        if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
        if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false); });

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);
    }



    void TurboToggle()
    {
        audioController.PlayButtonAudio();
        if (IsTurboOn)
        {
            IsTurboOn = false;
            Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Turbo_Button.image.sprite = TurboToggleSprite;
            // Turbo_Button.image.color = new Color(0.86f, 0.86f, 0.86f, 1);
        }
        else
        {
            IsTurboOn = true;
            Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
            // Turbo_Button.image.color = new Color(1, 1, 1, 1);
        }
    }
    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {

            IsAutoSpin = true;
            if (!AutoSpinStop_Button.interactable) AutoSpinStop_Button.interactable = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

        }
    }


    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
        }
        WasAutoSpinOn = false;
    }

    private void StopAutoSpin()
    {
        audioController.PlayButtonAudio();
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }


    private IEnumerator StopAutoSpinCoroutine()
    {
        WasAutoSpinOn = false;
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            AutoSpinRoutine = null;

        }
        if (tweenroutine != null)
        {
            StopCoroutine(tweenroutine);
            tweenroutine = null;
        }

    }

    internal void FreeSpin(int spins)
    {
        Debug.Log("spin chances" + spins);

        if (!IsFreeSpin)
        {
            // if (FSnum_text) FSnum_text.text = spins.ToString();
            // if (FSBoard_Object) FSBoard_Object.SetActive(true);
            IsFreeSpin = true;
            ToggleButtonGrp(false);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        int i = 0;
        AutoSpinStop_Button.interactable = false;
        uiManager.BonusWin = 0;
        while (i < spinchances)
        {
            StartSlots();
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
            i++;
        }
        if (!AutoSpinStop_Button.interactable) AutoSpinStop_Button.interactable = true;
        StartCoroutine(uiManager.FreeSpinProcessStop());
        Debug.Log($"#$############# free sppin process stop called freespincoroutine");

        FreeSpinCounter = 0;
        uiManager.FreeSpins = 0;
        // yield return new WaitForSeconds(5f);
        yield return new WaitUntil(() => !uiManager.isFreeSpinPopupActive);
        yield return new WaitForSeconds(2.5f);
        if (WasAutoSpinOn)
        {
            AutoSpin();
        }
        else
        {
            ToggleButtonGrp(true);
        }
        IsFreeSpin = false;
    }

    private void CompareBalance()
    {
        Debug.Log("Called");
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
        }

    }

    #region LinesCalculation
    //Fetch Lines from backend
    internal void FetchLines(string LineVal, int count)
    {
        y_string.Add(count + 1, LineVal);
    }


    #endregion

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.bets.Count - 1;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count).ToString();
        currentTotalBet = SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count;
        CompareBalance();
    }

    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.bets.Count - 1)
            {
                BetCounter++;
            }
            else
            {
                BetCounter = 0;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
            else
            {
                BetCounter = SocketManager.initialData.bets.Count - 1;
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count).ToString();
        currentTotalBet = SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count;


        // CompareBalance();
    }

    private void ToggleTotalBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.bets.Count - 1)
            {
                BetCounter++;
            }
            else
            {
                BetCounter = 0;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
            else
            {
                BetCounter = SocketManager.initialData.bets.Count - 1;
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count).ToString("f2");
        currentTotalBet = SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count;
        // CompareBalance();
        uiManager.PopulateSymbolsPayout(SocketManager.initUIData.paylines);
    }

    #region InitialFunctions
    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 11);
                Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }

        // SetWinningMatrix();
        // PlaywinningMatrixAnim();
    }

    internal void SetWinningMatrix()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                int value = SocketManager.Winmatrix[row, col];
                Tempimages[col].slotImages[row].sprite = myImages[value];
                PopulateAnimationSprites(Tempimages[col].slotImages[row].GetComponent<ImageAnimation>(), value);

            }
        }
    }

    internal void PlaywinningMatrixAnim()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                int value = SocketManager.Winmatrix[row, col];
                StartGameAnimation(Tempimages[col].slotImages[row].gameObject);

            }
        }
    }

    private void shuffleInitialMatrixKTR()
    {
        for (int i = 0; i < KTRimages.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 11);
                KTRimages[i].slotImages[j].sprite = KTRImages[randomIndex];
            }
        }
    }

    internal void SetInitialUI()
    {
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count).ToString();
        if (TotalWin_text) TotalWin_text.text = "0.000";
        if (Balance_text) Balance_text.text = SocketManager.playerdata.balance.ToString("f3");
        currentBalance = SocketManager.playerdata.balance;
        currentTotalBet = SocketManager.initialData.bets[BetCounter] * SocketManager.initialData.lines.Count;
        CompareBalance();
        uiManager.InitialiseUIData(SocketManager.initUIData.paylines);
    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 25f;
        switch (val)
        {
            case 0:
                for (int i = 0; i < Shoe_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Shoe_Sprite[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Fish_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Fish_Sprite[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Cheese_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Cheese_Sprite[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Garlic_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Garlic_Sprite[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Egg_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Egg_Sprite[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Boy_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Boy_Sprite[i]);
                }
                animScript.AnimationSpeed = 80f;
                break;
            case 6:
                for (int i = 0; i < LadyCoat_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(LadyCoat_Sprite[i]);
                }
                animScript.AnimationSpeed = 80f;
                break;
            case 7:
                for (int i = 0; i < OldMan_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(OldMan_Sprite[i]);
                }
                animScript.AnimationSpeed = 140f;
                break;
            case 8:
                for (int i = 0; i < FatLady_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FatLady_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 9:
                for (int i = 0; i < Wild_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Wild_Sprite[i]);
                }
                animScript.AnimationSpeed = 40f;
                break;
            case 10:
                for (int i = 0; i < Scatter_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Scatter_Sprite[i]);
                }
                animScript.AnimationSpeed = 35f;
                break;
            case 11:
                for (int i = 0; i < Keys_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Keys_Sprite[i]);
                }
                animScript.AnimationSpeed = 45f;
                break;
            case 12:
                for (int i = 0; i < Trash_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Trash_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;
        }
    }

    private void PopulateAnimationSpritesKTR(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        animScript.AnimationSpeed = 25f;
        switch (val)
        {
            case 0:
                for (int i = 0; i < ShoeKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(ShoeKTR_Sprite[i]);
                }
                break;
            case 1:
                for (int i = 0; i < FishKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FishKTR_Sprite[i]);
                }
                break;
            case 2:
                for (int i = 0; i < CheeseKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(CheeseKTR_Sprite[i]);
                }
                break;
            case 3:
                for (int i = 0; i < GarlicKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(GarlicKTR_Sprite[i]);
                }
                break;
            case 4:
                for (int i = 0; i < EggKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(EggKTR_Sprite[i]);
                }
                break;
            case 5:
                for (int i = 0; i < BoyKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(BoyKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 20f;
                break;
            case 6:
                for (int i = 0; i < LadyCoatKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(LadyCoatKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 20f;
                break;
            case 7:
                for (int i = 0; i < OldManKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(OldManKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 8:
                for (int i = 0; i < FatLadyKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FatLadyKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 9:
                for (int i = 0; i < WildKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(WildKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 95f;
                break;
            case 10:
                for (int i = 0; i < ScatterKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(ScatterKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 90f;
                break;
            case 11:
                for (int i = 0; i < KeysKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(KeysKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 45f;
                break;
            case 12:
                for (int i = 0; i < TrashKTR_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(TrashKTR_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;
        }
    }

    #region SlotSpin
    //starts the spin process
    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlaySpinButtonAudio();
        if (TotalWin_text) TotalWin_text.text = "0.000";
        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }

        WinningsAnim(false);
        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            // yield return new WaitForSeconds(0.5f);
            ToggleButtonGrp(true);
            yield break;
        }

        if (IsFreeSpin)
        {
            FreeSpinCounter--;
            uiManager.FreeSpins--;
            uiManager.UpdateUI(FreeSpinCounter, 0.000);
        }
        if (uiManager) uiManager.ToggleSquirrel(true);
        if (audioController) audioController.PlayWLAudio("spin");
        CheckSpinAudio = true;

        IsSpinning = true;

        ToggleButtonGrp(false);
        if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin)
        {
            StopSpin_Button.gameObject.SetActive(true);
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (!IsFreeSpin)
            {
                InitializeTweening(Slot_Transform[i]);
            }
            else
            {
                InitializeTweening(KTRSlot_Transform[i]);
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (!IsFreeSpin)
        {
            BalanceDeduction();
        }
        SocketManager.AccumulateResult(BetCounter);

        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int i = 0; i < SocketManager.ResultData.matrix.Count; i++)
        {
            for (int j = 0; j < SocketManager.ResultData.matrix[i].Count; j++)
            {
                int resultNum = int.Parse(SocketManager.ResultData.matrix[i][j]);
                //   Debug.Log($" Resukt Num: {resultNum} at {j} {i}");
                // print("resultNum: " + resultNum);
                // print("image loc: " + j + " " + i);
                if (!IsFreeSpin)
                {
                    Tempimages[j].slotImages[i].sprite = myImages[resultNum];
                    if (images[j].slotImages[i]) images[j].slotImages[i].sprite = myImages[resultNum];
                    if (Animimages[j].slotImages[i]) Animimages[j].slotImages[i].sprite = myImages[resultNum];
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].GetComponent<ImageAnimation>(), resultNum);
                }
                else
                {
                    if (KTRimages[j].slotImages[i]) KTRimages[j].slotImages[i].sprite = KTRImages[resultNum];
                    if (Animimages[j].slotImages[i]) Animimages[j].slotImages[i].sprite = KTRImages[resultNum];
                    PopulateAnimationSpritesKTR(Animimages[j].slotImages[i].gameObject.GetComponent<ImageAnimation>(), resultNum);



                    Tempimages[j].slotImages[i].sprite = myImages[resultNum];
                    if (images[j].slotImages[i]) images[j].slotImages[i].sprite = myImages[resultNum];
                    if (Animimages[j].slotImages[i]) Animimages[j].slotImages[i].sprite = myImages[resultNum];
                    PopulateAnimationSprites(Tempimages[j].slotImages[i].GetComponent<ImageAnimation>(), resultNum);
                }
            }
        }

        if (IsTurboOn || IsFreeSpin)
        {

            yield return new WaitForSeconds(0.1f);
            StopSpinToggle = true;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.1f);
                if (StopSpinToggle)
                {
                    break;
                }
            }
            StopSpin_Button.gameObject.SetActive(false);
        }

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (!IsFreeSpin)
            {
                yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
            }
            else
            {
                yield return StopTweening(5, KTRSlot_Transform[i], i, StopSpinToggle);
            }

        }
        StopSpinToggle = false;
        audioController.StopWLAaudio();
        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();
        if (SocketManager.ResultData.features.totalWinAmount > 0)
        {
            SpinDelay = 1.2f;
        }
        else
        {
            SpinDelay = 0.2f;
        }

        if (SocketManager.ResultData.payload.winAmount > 0)
        {
            if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.features.totalWinAmount.ToString("f3");
            List<int> winLine = new();
            foreach (var item in SocketManager.ResultData.payload.wins)
            {
                winLine.Add(item.lineIndex);
            }
            CheckPayoutLineBackend(winLine, 0);
            if (IsAutoSpin)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        //   CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.tempBonus.trashForCashWinningSymbols, SocketManager.resultData.scatterWinningSymbols);


        //comment 1
        if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.features.totalWinAmount.ToString("f3");
        BalanceTween?.Kill();
        if (Balance_text && !SocketManager.ResultData.features.bonus.enabled) Balance_text.text = SocketManager.ResultData.player.balance.ToString("f3");

        currentBalance = SocketManager.ResultData.player.balance;
        // if (SocketManager.ResultData.features.scatter.enabled)
        // {
        //     CheckPopups = true;
        //     CheckWinPopups(true);
        // }
        // yield return new WaitUntil(() => !CheckPopups);
        CheckPopups = true;
        if (SocketManager.ResultData.features.bonus.enabled)
        {
            CheckBonusGame();
        }
        else
        {
            CheckWinPopups();
        }
        yield return new WaitUntil(() => !CheckPopups);
        if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.features.totalWinAmount.ToString("f3");

        if (IsFreeSpin)
        {
            uiManager.UpdateUI(FreeSpinCounter, SocketManager.ResultData.features.totalWinAmount);
        }


        if (!IsAutoSpin && !IsFreeSpin)
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            // yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }

        if (SocketManager.ResultData.features.freeSpin.isFreeSpin)
        {
            Debug.Log($"################## free spin inside 0");
            FreeSpinCounter = (int)SocketManager.ResultData.features.freeSpin.count;
            if (IsAutoSpin)
            {
                WasAutoSpinOn = true;
                IsAutoSpin = false;
                // if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
                if (AutoSpinRoutine != null)
                {
                    StopCoroutine(AutoSpinRoutine);
                }
            }

            // if (IsFreeSpin)
            // {
            //     Debug.Log($"################## free spin inside freespin 1");
            //     IsFreeSpin = false;
            //     if (FreeSpinRoutine != null)
            //     {
            //         StopCoroutine(FreeSpinRoutine);
            //         FreeSpinRoutine = null;
            //     }
            //     uiManager.FreeSpinProcessStart(FreeSpinCounter, currentTotalBet);
            // }
            // else
            // {
            Debug.Log($"################## free spin inside 2");
            shuffleInitialMatrixKTR();
            if (uiManager) uiManager.ToggleBonusRText(true);
            if (audioController) audioController.SwitchBGSound(true);
            if (KTRStart_Button) KTRStart_Button.gameObject.SetActive(false);
            if (uiManager) uiManager.ToggleBonusRText(false);
            uiManager.FreeSpinProcessStart(FreeSpinCounter, currentTotalBet, true);
            PayCalculator.ResetLines();
            StopGameAnimation();
            //}
            yield return new WaitUntil(() => !uiManager.FSPopUpActive);
            yield return new WaitForSeconds(0.5f);

            FreeSpin(FreeSpinCounter);
        }

        //commnet 2

        // if (SocketManager.resultData.freeSpin.isNewAdded)
        // {
        //     FreeSpinCounter = (int)SocketManager.resultData.freeSpin.freeSpinCount;
        //     if (IsFreeSpin)
        //     {
        //         ToggleButtonGrp(true);
        //         uiManager.FreeSpinProcessStart((int)SocketManager.resultData.freeSpin.freeSpinCount, currentTotalBet);
        //     }
        //     else
        //     {
        //         IsFreeSpin = true;
        //         shuffleInitialMatrixKTR();
        //         if (uiManager) uiManager.ToggleBonusRText(true);
        //         if (KTRStart_Button) KTRStart_Button.gameObject.SetActive(true);
        //     }
        // }

        if (uiManager) uiManager.ToggleSquirrel(false);
        if (IsFreeSpin && FreeSpinCounter <= 0)
        {
            IsFreeSpin = false;
            //  uiManager.FreeSpinProcessStop();
            //   Debug.Log($"#$############# free sppin process stop called slot");

            // yield return new WaitForSeconds(4f);
            // ToggleButtonGrp(true);
            DOVirtual.DelayedCall(4f, () =>
            {
                ToggleButtonGrp(true);
            });
        }

    }

    private void EnableFreespinPlayButton()
    {
        if (audioController) audioController.SwitchBGSound(true);
        if (KTRStart_Button) KTRStart_Button.gameObject.SetActive(false);
        if (uiManager) uiManager.ToggleBonusRText(false);
        uiManager.FreeSpinProcessStart((int)SocketManager.resultData.freeSpin.count, currentTotalBet, true);
        PayCalculator.ResetLines();
        StopGameAnimation();
        ToggleButtonGrp(true);
    }

    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }
        double initAmount = balance;

        balance = balance - bet;

        BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("f3");
        });
    }

    internal void CheckWinPopups()
    {
        if (TotalWin_text) TotalWin_text.text = SocketManager.ResultData.features.totalWinAmount.ToString("f3");
        double winAmount = SocketManager.ResultData.features.totalWinAmount;
        // if (isScatter)
        // {
        //     double ScatterwinAmount = SocketManager.ResultData.features.scatter.amount;
        //     uiManager.PopulateWin(3, ScatterwinAmount);
        // }
        // else
        // {
        // = currentTotalBet * 10 && SocketManager.resultData.WinAmout < currentTotalBet * 15
        if (winAmount >= currentTotalBet * 5 && winAmount < currentTotalBet * 10)
        {
            uiManager.PopulateWin(1, winAmount);
        }
        else if (winAmount >= currentTotalBet * 10 && winAmount < currentTotalBet * 15)
        {
            uiManager.PopulateWin(2, winAmount);
        }
        else if (winAmount >= currentTotalBet * 15)
        {
            uiManager.PopulateWin(3, winAmount);
        }
        else
        {
            CheckPopups = false;
        }
        // }
    }

    internal void CheckBonusGame()
    {

        SocketManager.tempBonus = new BonusData();
        SocketManager.tempBonus.selectedBonusMultiplier = SocketManager.ResultData.features.bonus.selectedBonus.multiplier;
        // SocketManager.tempBonus.selectedBonusMultiplier = SocketManager.ResultData.features.bonus.selectedBonus.multiplier;
        SocketManager.tempBonus.shuffledBonusValues = SocketManager.ResultData.features.bonus.creditBonus;

        _bonusManager.StartBonus(SocketManager.tempBonus);
    }

    private void CheckPayoutLineBackend(List<int> LineId, double jackpot = 0)
    {
        Debug.Log($"LineId: {string.Join(", ", LineId)} ");
        List<int> y_points = null;
        if (LineId.Count > 0)
        {
            //  slotwinObject.SetActive(true);
            if (jackpot <= 0)
            {
                // if (audioController.m_Player_Listener.enabled) audioController.m_Win_Audio.Play();
            }

            for (int i = 0; i < LineId.Count; i++)
            {
                y_points = y_string[LineId[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
                PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineId[i] % 10);
                //   Debug.Log($"Payout Line: {LineId[i]} - {string.Join(", ", y_points)}");
            }

            if (jackpot > 0)
            {
                // if (audioController.m_Player_Listener.enabled) audioController.m_Win_Audio.Play();
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                List<KeyValuePair<int, int>> coords = new();
                for (int j = 0; j < LineId.Count; j++)
                {
                    for (int k = 0; k < SocketManager.ResultData.payload.wins[j].positions.Count; k++)
                    {
                        int rowIndex = SocketManager.initialData.lines[LineId[j]][k];
                        int columnIndex = k;
                        coords.Add(new KeyValuePair<int, int>(rowIndex, columnIndex));
                    }
                }

                foreach (var coord in coords)
                {
                    int rowIndex = coord.Key;
                    int columnIndex = coord.Value;
                    StartGameAnimation(Tempimages[columnIndex].slotImages[rowIndex].gameObject);
                    Tempimages[columnIndex].slotImages[rowIndex].gameObject.SetActive(true);
                }
            }
            WinningsAnim(true);               //change it here ashu
        }
        else
        {

            // if (audioController.m_Player_Listener.enabled) audioController.m_LooseAudio.Play();
        }

    }

    //generate the payout lines generated 
    // private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, List<string> BonusSymbols, List<string> ScatterSymbols)
    // {
    //     List<int> y_points = null;
    //     List<int> points_anim = null;
    //     if (LineId.Count > 0 || points_AnimString.Count > 0 || BonusSymbols.Count > 0 || ScatterSymbols.Count > 0)
    //     {
    //         if (audioController) audioController.PlayWLAudio("win");
    //         for (int i = 0; i < LineId.Count; i++)
    //         {
    //             y_points = y_string[LineId[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
    //             PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineId[i] % 10);
    //         }

    //         for (int i = 0; i < points_AnimString.Count; i++)
    //         {
    //             points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

    //             for (int k = 0; k < points_anim.Count; k++)
    //             {
    //                 if (points_anim[k] >= 10)
    //                 {
    //                     StartGameAnimation(Animimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject, TempBoxScripts[(points_anim[k] / 10) % 10].boxScripts[points_anim[k] % 10]);
    //                 }
    //                 else
    //                 {
    //                     StartGameAnimation(Animimages[0].slotImages[points_anim[k]].gameObject, TempBoxScripts[0].boxScripts[points_anim[k]]);
    //                 }
    //             }
    //         }

    //         if (BonusSymbols.Count > 0)
    //         {
    //             BonusSymbols = BonusSymbols.ConvertAll(item => item.Replace(",", ""));
    //             points_anim = null;
    //             for (int i = 0; i < BonusSymbols.Count; i++)
    //             {
    //                 points_anim = BonusSymbols[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

    //                 for (int k = 0; k < points_anim.Count; k++)
    //                 {
    //                     if (points_anim[k] >= 10)
    //                     {
    //                         StartGameAnimation(Animimages[points_anim[k] % 10].slotImages[(points_anim[k] / 10) % 10].gameObject, TempBoxScripts[points_anim[k] % 10].boxScripts[(points_anim[k] / 10) % 10], true);
    //                     }
    //                     else
    //                     {
    //                         StartGameAnimation(Animimages[points_anim[k]].slotImages[0].gameObject, TempBoxScripts[points_anim[k]].boxScripts[0], true);
    //                     }
    //                 }
    //             }
    //         }

    //         if (ScatterSymbols.Count > 0)
    //         {
    //             ScatterSymbols = ScatterSymbols.ConvertAll(item => item.Replace(",", ""));
    //             points_anim = null;
    //             for (int i = 0; i < ScatterSymbols.Count; i++)
    //             {
    //                 points_anim = ScatterSymbols[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

    //                 for (int k = 0; k < points_anim.Count; k++)
    //                 {
    //                     if (points_anim[k] >= 10)
    //                     {
    //                         StartGameAnimation(Animimages[points_anim[k] % 10].slotImages[(points_anim[k] / 10) % 10].gameObject, TempBoxScripts[points_anim[k] % 10].boxScripts[(points_anim[k] / 10) % 10], false, true);
    //                     }
    //                     else
    //                     {
    //                         StartGameAnimation(Animimages[points_anim[k]].slotImages[0].gameObject, TempBoxScripts[points_anim[k]].boxScripts[0], false, true);
    //                     }
    //                 }
    //             }
    //         }

    //         WinningsAnim(true);
    //     }
    //     else
    //     {
    //         if (audioController) audioController.StopWLAaudio();
    //     }
    //     if (LineId.Count > 0 || BonusSymbols.Count > 0 || ScatterSymbols.Count > 0)
    //     {
    //         BoxAnimRoutine = StartCoroutine(BoxRoutine(LineId, BonusSymbols.Count, ScatterSymbols.Count));
    //     }
    //     CheckSpinAudio = false;
    // }

    private double GetTotalWinAmount(int mode)
    {
        double winAmount = SocketManager.ResultData.payload.winAmount;

        // Add scatter win if present and mode >= 1
        if (mode >= 1 && SocketManager.ResultData.features.scatter != null)
        {
            winAmount += SocketManager.ResultData.features.scatter.amount;
        }

        // Add bonus win if present and mode >= 2
        if (mode >= 2 && SocketManager.ResultData.features.bonus != null)
        {
            winAmount += SocketManager.ResultData.features.bonus.amount;
        }

        return winAmount;
    }

    private IEnumerator BoxRoutine(List<int> LineIDs, int BonusCount, int scatterCount)
    {
        yield return new WaitForSeconds(2f);
        PayCalculator.DontDestroyLines.Clear();
        PayCalculator.DontDestroyLines.TrimExcess();
        PayCalculator.ResetLines();
        while (true)
        {
            List<int> y_points = null;
            if (LineIDs.Count > 0)
            {
                for (int i = 0; i < LineIDs.Count; i++)
                {
                    y_points = y_string[LineIDs[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
                    PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineIDs[i] % 10);
                    PayCalculator.DontDestroyLines.Add(LineIDs[i]);
                    for (int s = 0; s < 5; s++)
                    {
                        if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isBonus && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isScatter)
                        {
                            TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].SetBG(Box_Sprites[LineIDs[i] % 10]);
                            TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].transform.parent.gameObject.SetActive(true);
                        }
                    }
                    if (LineIDs.Count < 2)
                    {
                        yield break;
                    }
                    yield return new WaitForSeconds(2f);
                    for (int s = 0; s < 5; s++)
                    {
                        if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isBonus && !TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isScatter)
                        {
                            TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].ResetBG();
                            TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].transform.parent.gameObject.SetActive(false);
                        }
                    }
                    PayCalculator.DontDestroyLines.Clear();
                    PayCalculator.DontDestroyLines.TrimExcess();
                    PayCalculator.ResetLines();
                }
            }

            if (BonusCount > 0)
            {
                for (int i = 0; i < TempList.Count; i++)
                {
                    if (TempList[i].GetComponent<BoxScripting>().isBonus)
                    {
                        TempList[i].transform.parent.gameObject.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(2f);

                for (int i = 0; i < TempList.Count; i++)
                {
                    if (TempList[i].GetComponent<BoxScripting>().isBonus)
                    {
                        TempList[i].transform.parent.gameObject.SetActive(false);
                    }
                }
            }

            if (scatterCount > 0)
            {
                for (int i = 0; i < TempList.Count; i++)
                {
                    if (TempList[i].GetComponent<BoxScripting>().isScatter)
                    {
                        TempList[i].transform.parent.gameObject.SetActive(true);
                    }
                }
                yield return new WaitForSeconds(2f);

                for (int i = 0; i < TempList.Count; i++)
                {
                    if (TempList[i].GetComponent<BoxScripting>().isScatter)
                    {
                        TempList[i].transform.parent.gameObject.SetActive(false);
                    }
                }
            }

            for (int i = 0; i < LineIDs.Count; i++)
            {
                y_points = y_string[LineIDs[i] + 1]?.Split(',')?.Select(Int32.Parse)?.ToList();
                PayCalculator.GeneratePayoutLinesBackend(y_points, y_points.Count, LineIDs[i] % 10);
                PayCalculator.DontDestroyLines.Add(LineIDs[i]);
            }
            yield return new WaitForSeconds(2f);
            PayCalculator.DontDestroyLines.Clear();
            PayCalculator.DontDestroyLines.TrimExcess();
            PayCalculator.ResetLines();
        }
    }

    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
            WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
            WinTween.Kill();
            TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    #endregion

    internal void CallCloseSocket()
    {
        StartCoroutine(SocketManager.CloseSocket());
    }


    void ToggleButtonGrp(bool toggle)
    {

        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (BetMinus_Button) BetMinus_Button.interactable = toggle;
        if (BetPlus_Button) BetPlus_Button.interactable = toggle;
        if (TBPlus_Button) TBPlus_Button.interactable = toggle;
        if (TBMinus_Button) TBMinus_Button.interactable = toggle;
        if (Settings_Button) Settings_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        temp.StartAnimation();
        TempList.Add(temp);
        // boxscript.isAnim = true;
        // boxscript.isBonus = isBonus;
        // boxscript.isScatter = isScatter;
    }
    //  private void StartGameAnimation(GameObject animObjects, BoxScripting boxscript, bool isBonus = false, bool isScatter = false)
    // {
    //     ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
    //     temp.StartAnimation();
    //     TempList.Add(temp);
    //     boxscript.isAnim = true;
    //     boxscript.isBonus = isBonus;
    //     boxscript.isScatter = isScatter;
    // }

    //stop the icons animation
    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }

        if (BoxAnimRoutine != null)
        {
            StopCoroutine(BoxAnimRoutine);
            BoxAnimRoutine = null;
        }
        for (int i = 0; i < TempBoxScripts.Count; i++)
        {
            foreach (BoxScripting b in TempBoxScripts[i].boxScripts)
            {
                b.isAnim = false;
                b.isScatter = false;
                b.isBonus = false;
                b.ResetBG();
                b.transform.parent.gameObject.SetActive(false);
            }
        }
        TempList.Clear();
        TempList.TrimExcess();
    }


    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.Linear);
        tweener.Play();
        alltweens.Add(tweener);
    }



    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop)
    {
        alltweens[index].Kill();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.5f).SetEase(Ease.OutElastic);
        if (!isStop)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return null;
        }
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

    internal List<List<int>> GetTrashIndexes(bool freeSpin = false, bool trumpfreeSpin = false)
    {
        List<List<int>> BonusimagesIndex = new List<List<int>>();
        for (int i = 0; i < SocketManager.ResultData.matrix.Count; i++)
        {
            List<int> dummy = new List<int>();
            for (int j = 2; j < SocketManager.ResultData.matrix[i].Count; j++)
            {

                // if (Tempimages[j].slotImages[i].sprite = myImages[12])
                if (SocketManager.ResultData.matrix[i][j] == "12")
                {
                    if (!((i == 0 && j == 2) || (i == 4 && j == 2)))
                    {
                        // dummy.Add(j);
                        // dummy.Add(i);
                        Debug.Log($"Found Trash at {i} {j}");
                        var found = new List<int> { j, i };
                        BonusimagesIndex.Add(found);
                    }
                }
                // BonusimagesIndex.Add(dummy);
                // dummy.Clear();
            }
        }
        return BonusimagesIndex;
    }

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class BoxScript
{
    public List<BoxScripting> boxScripts = new List<BoxScripting>(10);
}


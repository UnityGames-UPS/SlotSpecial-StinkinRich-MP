using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField] SocketIOManager SocketManager;
    [SerializeField]
    private List<SlotImage> Bonusimages;
    [SerializeField]
    private Image Bonusbanner_Image;
    [SerializeField]
    private GameObject Bonusbanner_Object;
    [SerializeField]
    private GameObject MainBonus_Screen;
    [SerializeField]
    private GameObject MainGame_Title;
    [SerializeField]
    private GameObject TrashCash_Title;
    [SerializeField]
    private GameObject RayCast_Panel;
    [Header("WinPopup")]
    [SerializeField]
    private GameObject MainPopup_Object;
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private Image WinPopup_Image;
    [SerializeField]
    private TMP_Text Title_text;
    [SerializeField]
    private TMP_Text BonusTitle_text;
    [SerializeField]
    private TMP_Text Multi_text;
    [SerializeField]
    private TMP_Text BonusVal_text;
    [SerializeField]
    private TMP_Text MultiVal_text;
    [SerializeField]
    private TMP_Text TotalWIn_text;
    [SerializeField] private TMP_Text TotalWonTitle_Text;
    [SerializeField]
    private AudioController audioController;

    private double totalWin = 0;
    private double Win = 0;
    private List<TrashCashFunctionality> tempList = new List<TrashCashFunctionality>();
    private BonusData savedData;
 //   internal List< List<int>> Bonusindexes= new List<List<int>>();
  
    internal void StartBonus(BonusData data)
    {
        if (audioController) audioController.SwitchBGSound(true);
        ResetTrash();
        savedData = data;
        tempList.Clear();
        tempList.TrimExcess();
        totalWin = 0;
        Win = 0;
        //  List<string> points_AnimString = savedData.trashForCashWinningSymbols.ConvertAll(item => item.Replace(",", ""));
        List<int> points_anim = null;

        // foreach (string item in points_AnimString)
        // {
        //     Debug.Log(item);
        // }

        List<List<int>> Bonusindexes = slotManager.GetTrashIndexes();
        Debug.Log($"Trash for cash indexes:" + Bonusindexes.Count);
        for (int k = 0; k < Bonusindexes.Count; k++)
        {
            Debug.Log($"Trash for cash indexes:" + Bonusindexes[k][0] + " " + Bonusindexes[k][1]);
            Bonusimages[Bonusindexes[k][0]].slotImages[Bonusindexes[k][1]].gameObject.SetActive(true);
            tempList.Add(Bonusimages[Bonusindexes[k][0]].slotImages[Bonusindexes[k][1]].gameObject.GetComponent<TrashCashFunctionality>());
        }

        // for (int i = 0; i < points_AnimString.Count; i++)
        // {
        //     points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

        //     for (int k = 0; k < points_anim.Count; k++)
        //     {
        //         if (points_anim[k] >= 10)
        //         {
        //             Bonusimages[points_anim[k] % 10].slotImages[(points_anim[k] / 10) % 10].gameObject.SetActive(true);
        //             tempList.Add(Bonusimages[points_anim[k] % 10].slotImages[(points_anim[k] / 10) % 10].gameObject.GetComponent<TrashCashFunctionality>());
        //         }
        //         else
        //         {
        //             Bonusimages[points_anim[k]].slotImages[0].gameObject.SetActive(true);
        //             tempList.Add(Bonusimages[points_anim[k]].slotImages[0].gameObject.GetComponent<TrashCashFunctionality>());
        //         }
        //     }
        // }
        if (RayCast_Panel) RayCast_Panel.SetActive(false);
        if (MainBonus_Screen) MainBonus_Screen.SetActive(true);
        if (TrashCash_Title) TrashCash_Title.SetActive(true);
        if (MainGame_Title) MainGame_Title.SetActive(false);
        Showbanner();
    }

    internal int GetMultiplier()
    {
        return savedData.selectedBonusMultiplier;
    }

    internal double GetTrashValue()
    {
        if (audioController) audioController.PlayBonusAudio("trash");
        if (RayCast_Panel) RayCast_Panel.SetActive(true);
        //   return savedData.shuffledBonusValues[savedData.shuffledBonusValues.Count - 1];
     return SocketManager.ResultData.features.bonus.selectedBonus.credit;
        // return savedData.shuffledBonusValues[savedData.shuffledBonusValues.Count - 1];
    }

    internal void ClickedTrash()
    {
        int s = 0;
        for (int i = 0; i < tempList.Count; i++)
        {
            if (!tempList[i].isOpen)
            {
                tempList[i].OpenTrash(false, savedData.shuffledBonusValues[s]);
                s++;
            }
        }
        CalculateWin();
    }

    private void CalculateWin()
    {
        for (int i = 0; i < savedData.shuffledBonusValues.Count - 1; i++)
        {
            Win += savedData.shuffledBonusValues[i];
        }
        Debug.Log($"Win amount 0: {Win}");
        totalWin = Win * savedData.selectedBonusMultiplier;
                Debug.Log($"Win amount 1: {totalWin}");

        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (WinPopup_Image) WinPopup_Image.color = new Color(WinPopup_Image.color.r, WinPopup_Image.color.g, WinPopup_Image.color.b, 0f);
        // if(Title_text) Title_text.color = new Color(Title_text.color.r, Title_text.color.g, Title_text.color.b, 0f);
        // if(BonusTitle_text) BonusTitle_text.color = new Color(BonusTitle_text.color.r, BonusTitle_text.color.g, BonusTitle_text.color.b, 0f);
        // if(Multi_text) Multi_text.color = new Color(Multi_text.color.r, Multi_text.color.g, Multi_text.color.b, 0f);
        // if(BonusVal_text) BonusVal_text.color = new Color(BonusVal_text.color.r, BonusVal_text.color.g, BonusVal_text.color.b, 0f);
        // if(MultiVal_text) MultiVal_text.color = new Color(MultiVal_text.color.r, MultiVal_text.color.g, MultiVal_text.color.b, 0f);
        if (TotalWIn_text) TotalWIn_text.color = new Color(TotalWIn_text.color.r, TotalWIn_text.color.g, TotalWIn_text.color.b, 0f);
        if (TotalWonTitle_Text) TotalWonTitle_Text.color = new Color(TotalWonTitle_Text.color.r, TotalWonTitle_Text.color.g, TotalWonTitle_Text.color.b, 0);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        // if (BonusVal_text) BonusVal_text.text = Win.ToString();
        // if (MultiVal_text) MultiVal_text.text = savedData.selectedBonusMultiplier.ToString() + "x";
        // int multipliedValue = Win*savedData.selectedBonusMultiplier;
        double winnings = SocketManager.ResultData.features.bonus.amount;
        print("Trash for cash amount: " + winnings);
        if (TotalWIn_text) TotalWIn_text.text = winnings.ToString("F3");
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (WinPopup_Image) WinPopup_Image.DOFade(1f, .5f);
        yield return new WaitForSeconds(0.2f);
        // if (Title_text) Title_text.DOFade(1f, 0.3f);
        // yield return new WaitForSeconds(0.5f);
        // if (BonusTitle_text) BonusTitle_text.DOFade(1f, 0.3f);
        // yield return new WaitForSeconds(0.5f);
        // if (BonusVal_text) BonusVal_text.DOFade(1f, 0.3f);
        // yield return new WaitForSeconds(0.5f);
        // if (Multi_text) Multi_text.DOFade(1f, 0.3f);
        // yield return new WaitForSeconds(0.5f);
        // if (MultiVal_text) MultiVal_text.DOFade(1f, 0.3f);
        // yield return new WaitForSeconds(0.5f);
        if (TotalWIn_text) TotalWIn_text.DOFade(1f, 0.3f);
        if (TotalWonTitle_Text) TotalWonTitle_Text.DOFade(1, 0.3f);
        yield return new WaitForSeconds(3f);
        if (WinPopup_Image) WinPopup_Image.DOFade(0f, 1f);
        // if (Title_text) Title_text.DOFade(0f, 1f);
        // if (BonusTitle_text) BonusTitle_text.DOFade(0f, 1f);
        // if (BonusVal_text) BonusVal_text.DOFade(0f, 1f);
        // if (Multi_text) Multi_text.DOFade(0f, 1f);
        // if (MultiVal_text) MultiVal_text.DOFade(0f, 1f);
        if (TotalWIn_text) TotalWIn_text.DOFade(0f, 0.5f);
        if (TotalWonTitle_Text) TotalWonTitle_Text.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.6f);
        if (WinPopup_Object) WinPopup_Object.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        if (Bonusbanner_Object) Bonusbanner_Object.SetActive(false);
        if (MainBonus_Screen) MainBonus_Screen.SetActive(false);
        if (TrashCash_Title) TrashCash_Title.SetActive(false);
        if (MainGame_Title) MainGame_Title.SetActive(true);
        if (slotManager) slotManager.CheckWinPopups();
        slotManager.Balance_text.text = SocketManager.playerdata.balance.ToString("f3");
        if (!slotManager.IsFreeSpin)
        {
            if (audioController) audioController.SwitchBGSound(false);
        }
    }

    private void Showbanner()
    {
        if (Bonusbanner_Image) Bonusbanner_Image.color = new Color(Bonusbanner_Image.color.r, Bonusbanner_Image.color.g, Bonusbanner_Image.color.b, 0f);
        if (Bonusbanner_Object) Bonusbanner_Object.SetActive(true);
        if (Bonusbanner_Image) Bonusbanner_Image.DOFade(1f, 1f);
    }

    private void ResetTrash()
    {
        for (int i = 0; i < Bonusimages.Count; i++)
        {
            foreach (Image item in Bonusimages[i].slotImages)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }

}

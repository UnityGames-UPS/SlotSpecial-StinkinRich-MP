using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TrashCashFunctionality : MonoBehaviour
{
    [SerializeField]
    private BonusController _bonusManager;
    [SerializeField]
    private Button trash_Button;
    [SerializeField]
    private GameObject trashOpen_Object;
    [SerializeField]
    private GameObject trashClose_Object;
    [SerializeField]
    private TMP_Text Multiplier_Text;
    [SerializeField]
    private TMP_Text Value_Text;
    [SerializeField]
    private RectTransform Multiplier_Transform;
    [SerializeField]
    private RectTransform Value_Transform;
    [SerializeField]
    private GameObject Line_Object;
    internal bool isOpen = false;

    private void Start()
    {
        if (trash_Button) trash_Button.onClick.RemoveAllListeners();
        if (trash_Button) trash_Button.onClick.AddListener(delegate { OpenTrash(true); });
    }
    private void OnEnable()
    {
        isOpen = false;
        if (Value_Transform) Value_Transform.gameObject.SetActive(false);
        if (Multiplier_Transform) Multiplier_Transform.gameObject.SetActive(false);
        if (Line_Object) Line_Object.SetActive(false);
        if (Line_Object) Line_Object.SetActive(false);
        if (trashOpen_Object) trashOpen_Object.SetActive(false);
        if (trashClose_Object) trashClose_Object.SetActive(true);
    }

    internal void OpenTrash(bool isClicked,double value = 0)
    {
        if (trashOpen_Object) trashOpen_Object.SetActive(true);
        if (trashClose_Object) trashClose_Object.SetActive(false);
        if (isClicked)
        {
            if (Value_Text) Value_Text.text = _bonusManager.GetTrashValue().ToString();
        }
        else
        {
            if (Value_Text) Value_Text.text = value.ToString();
        }
        if (Value_Transform) Value_Transform.localScale = Vector3.zero;
        if (Value_Transform) Value_Transform.localPosition = new Vector3(Value_Transform.localPosition.x, -35, Value_Transform.localPosition.z);
        if (Value_Transform) Value_Transform.gameObject.SetActive(true);
        if (Value_Transform) Value_Transform.DOLocalMoveY(30f, 1f).SetDelay(0.9f);
        if (Value_Transform) Value_Transform.DOScale(Vector3.one,1f).SetDelay(0.9f);
        if (isClicked)
        {
            isOpen = true;
            StartCoroutine(MultiplierRoutine());
        }
    }

    private IEnumerator MultiplierRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (Value_Transform) Value_Transform.DOLocalMoveY(-30f, 1f);
        if (Value_Transform) Value_Transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 1f);
        yield return new WaitForSeconds(1f);
        if (Multiplier_Text) Multiplier_Text.text = _bonusManager.GetMultiplier().ToString() + "x";
        if (Multiplier_Transform) Multiplier_Transform.localScale = Vector3.zero;
        if (Multiplier_Transform) Multiplier_Transform.localPosition = new Vector3(Value_Transform.localPosition.x, -35, Value_Transform.localPosition.z);
        if (Line_Object) Line_Object.SetActive(true);
        if (Multiplier_Transform) Multiplier_Transform.gameObject.SetActive(true);
        if (Multiplier_Transform) Multiplier_Transform.DOLocalMoveY(30f, 1f);
        if (Multiplier_Transform) Multiplier_Transform.DOScale(Vector3.one, 1f);
        if (_bonusManager) _bonusManager.ClickedTrash();
    }

}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;

    [SerializeField] Button button;

    Action OnClick;
    private void Awake()
    {
        button.onClick.AddListener(OnClicked);
    }


    public void Set(ItemData item, Action onClick)
    {
        image.sprite = item.icon;
        nameText.text = item.name;

        this.OnClick = onClick;
    }

    private void OnClicked()
    {
        OnClick?.Invoke();
    }

  

}

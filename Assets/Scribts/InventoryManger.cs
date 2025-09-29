using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManger : MonoBehaviour
{
    [SerializeField] public List<Items> ItemsInInv;

    public Items CurrentItem;
    private int CurrentItemInt;
    [SerializeField] public GameObject ItemParent;
    [SerializeField] private Transform PlayerHand;
    [SerializeField] private GameObject ItemShowcaseLeft;
    [SerializeField] private GameObject ItemShowcaseMiddle;
    [SerializeField] private GameObject ItemShowcaseRight;
    [SerializeField] private TextMeshProUGUI CoinsText;

    public int Coins;

    private GameObject CurrentItemObject;
    void Start()
    {
        CheckItem();
        Coins = 9;
    }
    void Update()
    {
        CoinsText.text = Coins.ToString() + "$";
        if (CurrentItem == null && ItemsInInv != null)
        {
            CurrentItem = ItemsInInv[0];
            CheckItem();
        }
        if (ItemsInInv == null || ItemsInInv.Count == 0)
        {
            CurrentItem = null;
        }


    }
    public void NextItem()
    {

        CurrentItemInt += 1;
        if (CurrentItemInt >= ItemsInInv.Count)
        {
            CurrentItemInt = 0;
        }
        CurrentItem = ItemsInInv[CurrentItemInt];
        CheckItem();
    }
    public void LastItem()
    {

        CurrentItemInt -= 1;
        if (CurrentItemInt < 0)
        {
            CurrentItemInt = ItemsInInv.Count - 1;
        }
        CurrentItem = ItemsInInv[CurrentItemInt];

        CheckItem();
    }

    void CheckItem()
    {
        ItemParent.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = CurrentItem.ShowcaseImage;
        UpdateShowcase();
    }

    void LateUpdate()
    {
    }
    public void UpdateShowcase()
    {
        // left item
        int Leftint = CurrentItemInt - 1;
        if (Leftint < 0)
        {
            Leftint = ItemsInInv.Count - 1;
        }
        // right item
        int Rightint = CurrentItemInt + 1;
        if (Rightint >= ItemsInInv.Count)
        {
            Rightint = 0;
        }

        ItemShowcaseLeft.GetComponent<Image>().sprite = ItemsInInv[Leftint].ShowcaseImage;
        ItemShowcaseLeft.GetComponent<Animator>().Play("aniamtion");

        ItemShowcaseMiddle.GetComponent<Image>().sprite = ItemsInInv[CurrentItemInt].ShowcaseImage;
        ItemShowcaseMiddle.GetComponent<Animator>().Play("Middel");

        ItemShowcaseRight.GetComponent<Image>().sprite = ItemsInInv[Rightint].ShowcaseImage;
        ItemShowcaseRight.GetComponent<Animator>().Play("aniamtion");
    }
    public void ClearCurrentItem()
    {
        bool FoundItem = false;
        for (int i = 0; i < ItemsInInv.Count; i++)
        {
            if (ItemsInInv[i].name == CurrentItem.name && !FoundItem)
            {
                FoundItem = true;
                ItemsInInv.Remove(ItemsInInv[i]);
            }
        }
        LastItem();
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject ShopObject;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject ShopUi;
    [SerializeField] private GameObject ShopUiContent;
    [SerializeField] private List<Items> AllTools;
    [SerializeField] private List<Items> AllBuildings;
    [SerializeField] private List<Items> AllPlants;

    [SerializeField] private Button ToolsButton;
    [SerializeField] private Button BuildingButton;
    [SerializeField] private Button PlantButton;
    [SerializeField] private GameObject ShopElement;
    private string Filter;

    private BoxCollider2D PlayerBoxCollider;
    private BoxCollider2D ShopBoxCollider;
    private List<Button> BuyButtons = new List<Button>();
    [SerializeField] private InventoryManger inventoryManger;

    void Start()
    {
        Filter = "Tools";
        ShopUi.SetActive(false);

        ShopBoxCollider = ShopObject.GetComponent<BoxCollider2D>();
        PlayerBoxCollider = PlayerObject.GetComponent<BoxCollider2D>();

        BuildShop();

        ToolsButton.onClick.AddListener(() => { Filter = "Tools"; BuildShop(); });
        BuildingButton.onClick.AddListener(() => { Filter = "Buildings"; BuildShop(); });
        PlantButton.onClick.AddListener(() => { Filter = "Plants"; BuildShop(); });
    }

    void Update()
    {
        ShopUi.SetActive(PlayerBoxCollider.IsTouching(ShopBoxCollider));
    }
    public void CheckItemShop(string Name)
    {
        Items BuyItems = AllTools[0];
        switch (Filter)
        {
            case "Tools":
                for (int i = 0; i < AllTools.Count; i++)
                {
                    if (Name == AllTools[i].name)
                    {
                        BuyItems = AllPlants[i];
                    }
                }
                break;
            case "Buildings":
                for (int i = 0; i < AllBuildings.Count; i++)
                {
                    if (Name == AllBuildings[i].name)
                    {
                        BuyItems = AllPlants[i];
                    }
                }
                break;
            case "Plants":
                for (int i = 0; i < AllPlants.Count; i++)
                {
                    if (Name == AllPlants[i].name)
                    {
                        BuyItems = AllPlants[i];
                    }
                }
                break;

        }
        BuyItem(BuyItems);
    }
    public void BuyItem(Items item)
    {
        if (inventoryManger.Coins >= item.BuyPrice)
        {
            inventoryManger.Coins -= item.BuyPrice;
            inventoryManger.ItemsInInv.Add(item);
            inventoryManger.CurrentItem = item;
            inventoryManger.UpdateShowcase();
        }

    }

    public void BuildShop()
    {
        // Buttons resetten
        BuyButtons.Clear();

        // Vorherige Elemente löschen
        foreach (Transform child in ShopUiContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Richtige Liste holen
        List<Items> currentList = new List<Items>();
        switch (Filter)
        {
            case "Tools": currentList = AllTools; break;
            case "Buildings": currentList = AllBuildings; break;
            case "Plants": currentList = AllPlants; break;
        }

        // Elemente erstellen
        for (int i = 0; i < currentList.Count; i++)
        {
            Items item = currentList[i];

            GameObject newElement = Instantiate(ShopElement, ShopUiContent.transform);
            newElement.name = "BuyElement_" + i;

            newElement.transform.GetChild(0).GetComponent<Image>().sprite = item.ShowcaseImage;
            newElement.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name;
            newElement.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.BuyPrice.ToString();

            Button buyButton = newElement.transform.GetChild(3).GetComponent<Button>();
            buyButton.name = i.ToString();

            int index = i; // Lokale Kopie für Lambda
            buyButton.onClick.AddListener(() =>
            {
                inventoryManger.ItemsInInv.Add(currentList[index]);
                CheckItemShop(currentList[index].name);
            });

            BuyButtons.Add(buyButton);
        }
    }
}

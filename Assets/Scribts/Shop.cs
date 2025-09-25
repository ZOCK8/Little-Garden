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
    [SerializeField] private List<Items> AllSeeds;
    [SerializeField] private List<Items> AllPlantss;

    [SerializeField] private Button ToolsButton;
    [SerializeField] private Button SeedButton;
    [SerializeField] private Button PlantButton;
    [SerializeField] private GameObject ShopElement;
    private string Filter;
    private int ItemsCount;

    private BoxCollider2D PlayerBoxCollider;
    private BoxCollider2D ShopBoxCollider;
    private List<Button> BuyButtons;
    [SerializeField] private InventoryManger inventoryManger;
    void Start()
    {
        Filter = "Tools";
        ShopUi.SetActive(false);
        ShopBoxCollider = ShopObject.GetComponent<BoxCollider2D>();
        PlayerBoxCollider = PlayerObject.GetComponent<BoxCollider2D>();
        BuildShop();

        ToolsButton.onClick.AddListener(() => { Filter = "Tools"; ItemsCount = AllTools.Count; BuildShop(); });
        SeedButton.onClick.AddListener(() => { Filter = "Seeds"; ItemsCount = AllSeeds.Count; BuildShop(); });
        PlantButton.onClick.AddListener(() => { Filter = "Plants"; ItemsCount = AllPlantss.Count; BuildShop(); });

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerBoxCollider.IsTouching(ShopBoxCollider))
        {
            ShopUi.SetActive(true);
        }
        else
        {
            ShopUi.SetActive(false);
        }
        if (BuyButtons != null)
        {
            for (int i = 0; i < BuyButtons.Count; i++)
            {
                BuyButtons[i].onClick.AddListener(() =>
                {
                    int index;
                    switch (Filter)
                    {
                        case "Tools":
                            index = int.Parse(BuyButtons[i].name);
                            inventoryManger.ItemsInInv.Add(AllTools[index]);
                            break;
                        case "Seeds":
                            index = int.Parse(BuyButtons[i].name);
                            inventoryManger.ItemsInInv.Add(AllTools[index]);
                            break;
                        case "Plants":
                            index = int.Parse(BuyButtons[i].name);
                            inventoryManger.ItemsInInv.Add(AllTools[index]);
                            break;

                    }
                });
            }
        }
    }
    public void BuildShop()
    {
        if (BuyButtons != null)
        {
            BuyButtons.Clear();
        }
        Debug.Log("Building Shop");
        for (int i = ShopUiContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(ShopUiContent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < ItemsCount; i++)
        {
            GameObject newElement = Instantiate(ShopElement, ShopUiContent.transform);
            newElement.name = "BuyElement_" + i;
        }


        for (int i = 0; i < ShopUiContent.transform.childCount; i++)
        {
            GameObject Element = ShopUiContent.transform.GetChild(i).gameObject;
            switch (Filter)
            {
                case "Tools":
                    for (int s = 0; s < AllTools.Count; s++)
                    {
                        Element.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = AllTools[s].ShowcaseImage;
                        Element.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = AllTools[s].name;
                        Element.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = AllTools[s].BuyPrice.ToString();
                        BuyButtons.Add(Element.transform.GetChild(3).gameObject.GetComponent<Button>());
                        Element.transform.GetChild(3).gameObject.name = "" + s;
                    }
                    break;
                case "Seeds":
                    for (int s = 0; s < AllSeeds.Count; s++)
                    {
                        Element.name = "BuyElement" + i;
                        Element.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = AllSeeds[s].ShowcaseImage;
                        Element.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = AllSeeds[s].name;
                        Element.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = AllSeeds[s].BuyPrice.ToString();
                        BuyButtons.Add(Element.transform.GetChild(3).gameObject.GetComponent<Button>());
                        Element.transform.GetChild(3).gameObject.name = "" + s;

                    }
                    break;
                case "Plants":
                    for (int s = 0; s < AllPlantss.Count; s++)
                    {
                        Element.name = "BuyElement" + i;
                        Element.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = AllPlantss[s].ShowcaseImage;
                        Element.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = AllPlantss[s].name;
                        Element.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = AllPlantss[s].BuyPrice.ToString();
                        BuyButtons.Add(Element.transform.GetChild(3).gameObject.GetComponent<Button>());
                        Element.transform.GetChild(3).gameObject.name = "" + s;
                    }
                    break;
                default:
                    for (int s = 0; s < AllSeeds.Count; s++)
                    {
                        Element.name = "BuyElement" + i;
                        Element.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = AllSeeds[s].ShowcaseImage;
                        Element.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = AllSeeds[s].name;
                        Element.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = AllSeeds[s].BuyPrice.ToString();
                        BuyButtons.Add(Element.transform.GetChild(3).gameObject.GetComponent<Button>());
                        Element.transform.GetChild(3).gameObject.name = "" + s;

                    }
                    break;
            }

        }
    }
}

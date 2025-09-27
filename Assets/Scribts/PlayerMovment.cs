using System;
using NUnit.Framework.Internal.Commands;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] public GameObject PlayerObject;
    [SerializeField] private Tilemap GardenTile;
    [SerializeField] private RuleTile DirtTile;
    [SerializeField] private GameObject PlantsContainer;
    [SerializeField] private GameObject BuildingContainer;
    [SerializeField] private GameObject AddItemAnimator;
    private InputSystem_Actions InputSystem;
    private Rigidbody2D PlayerRb;
    private BoxCollider2D PlayerBc;
    private TilemapCollider2D TCGarden;
    private Vector2 moveInput;
    public InventoryManger inventoryManger;
    private Vector3 PlantingPos;
    void Awake()
    {
        InputSystem = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        InputSystem.Enable(); // Input System aktivieren
        InputSystem.Player.Move.performed += OnMove;
        InputSystem.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        InputSystem.Disable(); // Input System deaktivieren
        InputSystem.Player.Move.canceled -= OnMove;
        InputSystem.Player.Disable();
    }
    void Start()
    {
        TCGarden = GardenTile.GetComponent<TilemapCollider2D>();
        Vector2 moveInput = InputSystem.Player.Move.ReadValue<Vector2>();
        PlayerRb = PlayerObject.GetComponent<Rigidbody2D>();
        PlayerBc = PlayerObject.GetComponent<BoxCollider2D>();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    // Update is called once per frame
    void Update()
    {
        PlayerObject.transform.Translate(moveInput * Time.deltaTime * 5f);

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll < 0)
        {
            inventoryManger.NextItem();
        }
        if (scroll > 0)
        {
            inventoryManger.LastItem();
        }
        /////////////////////////////
        //// Scare Crow Placing System
        /////////////////////////////
        if (InputSystem.Player.Interact.WasPressedThisFrame() && PlayerRb.IsTouching(TCGarden) && inventoryManger.CurrentItem.name == "Scare Crow")
        {
            GameObject ScareCrow = Instantiate(inventoryManger.CurrentItem.ObjectInHand);
            Vector3 TilePos = GardenTile.WorldToCell(PlayerObject.transform.position) + new Vector3(0.5f, 0.7f, 0);
            ScareCrow.transform.position = TilePos;
            ScareCrow.transform.SetParent(BuildingContainer.transform);
            inventoryManger.ClearCurrentItem();
        }


        //////////////////////
        /// The Gras to farmland system
        /////////////////////////////

        if (InputSystem.Player.Interact.WasPressedThisFrame())
        {
            if (PlayerRb.IsTouching(TCGarden))
            {
                Debug.Log("is touching grass");
                if (inventoryManger.CurrentItem.name == "Shovel")
                {
                    Debug.Log("tryig to estroy");

                    Vector3Int TilePos = GardenTile.WorldToCell(PlayerObject.transform.position);
                    TileBase tile = GardenTile.GetTile(TilePos);
                    if (tile.name == "Grass")
                    {
                        Debug.Log("Ther is dirt to destroy at: " + tile.name);
                        GardenTile.SetTile(TilePos, DirtTile);
                    }
                }
            }
        }
        /////////////////////////////
        ///  Player Planting System
        /////////////////////////////
        if (InputSystem.Player.Interact.WasPressedThisFrame() && inventoryManger.CurrentItem.ItemType == ItemTypeEnum.Plant && PlayerRb.IsTouching(TCGarden))
        {
            Vector3Int TilePos = GardenTile.WorldToCell(PlayerObject.transform.position);
            Vector3 cellCenterPos = TilePos + new Vector3(0.5f, 0.95f, 0);
            PlantingPos = cellCenterPos;
            bool alreadyUsed = false;
            GameObject Plant = Instantiate(inventoryManger.CurrentItem.ObjectInHand);
            inventoryManger.ClearCurrentItem();
            for (int i = 0; i < PlantsContainer.transform.childCount; i++)
            {
                if (PlantsContainer.transform.GetChild(i).position == cellCenterPos)
                {
                    alreadyUsed = true;
                    break; // reicht, eine Pflanze gefunden → abbrechen
                }
            }

            if (!alreadyUsed && GardenTile.GetTile(TilePos)?.name == "FarmLand")
            {
                Debug.Log("Planting Plant");
                Planting(Plant);
            }


        }
        /////////////////////////////
        /// Player Plant Destroying
        /////////////////////////////
        if (InputSystem.Player.Interact.WasPressedThisFrame() && inventoryManger.CurrentItem.ItemName == "Sickle")
        {
            for (int i = 0; i < PlantsContainer.transform.childCount; i++)
            {
                if (PlayerBc.IsTouching(PlantsContainer.transform.GetChild(i).GetComponent<BoxCollider2D>()))
                {
                    var Plant = PlantsContainer.transform.GetChild(i).gameObject;
                    if (Plant.GetComponent<Plants>().IsDone)
                    {
                        AddItemAnimator.GetComponent<Animator>().SetTrigger("AddItem");
                        AddItemAnimator.transform.GetChild(0).GetComponent<Image>().sprite = Plant.GetComponent<Plants>().items.ShowcaseImage;
                        AddItemAnimator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+";
                        AddItemAnimator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.green;

                        inventoryManger.ItemsInInv.Add(Plant.GetComponent<Plants>().items);
                    }
                    DestroyPlant(Plant);
                }
            }
        }
        /////////////////////////////
        /// Player Watering can system
        /////////////////////////////
        if (inventoryManger.CurrentItem.ItemName == "Watering can" && InputSystem.Player.Interact.WasPressedThisFrame())
        {
            for (int i = 0; i < PlantsContainer.transform.childCount; i++)
            {
                if (PlayerBc.IsTouching(PlantsContainer.transform.GetChild(i).GetComponent<BoxCollider2D>()))
                {
                    PlantsContainer.transform.GetChild(i).GetComponent<Plants>().WasWatered = true;
                    Vector3Int PosTile = GardenTile.WorldToCell(PlantsContainer.transform.GetChild(i).transform.position);
                    GardenTile.SetColor(PosTile, Color.navajoWhite);
                }
            }
        }



    }
    public void Planting(GameObject PlantingPlant)
    {
        Vector3Int tilepos = GardenTile.WorldToCell(PlantingPos);
        GardenTile.SetColor(tilepos, Color.navajoWhite);
        PlantingPlant.transform.position = PlantingPos;
        if (PlantingPlant.GetComponent<Plants>().items == null)
        {
            PlantingPlant.GetComponent<Plants>().items = inventoryManger.CurrentItem;
        }
        else
        {
            Debug.LogError("The current Item has not Items Pleas add!");
        }
        PlantingPlant.transform.SetParent(PlantsContainer.transform);
    }
    public void DestroyPlant(GameObject PlantToDestroy)
    {
        if (PlantToDestroy.GetComponent<Plants>().IsDone == true && PlantToDestroy.GetComponent<Plants>().items != null)
        {
            inventoryManger.ItemsInInv.Add(PlantToDestroy.GetComponent<Plants>().items);
        }
        Destroy(PlantToDestroy.gameObject, 0.2f);
    }
}

using System;
using NUnit.Framework.Internal.Commands;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] public GameObject PlayerObject;
    [SerializeField] private Tilemap GardenTile;
    [SerializeField] private Tile DirtTile;
    [SerializeField] private Tile DirtPlantTile;
    [SerializeField] private GameObject PlantsContainer;
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
            inventoryManger.LastItem();
        }
        if (scroll > 0)
        {
            inventoryManger.NextItem();
        }

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
                    if (tile.name == "Grass_0")
                    {
                        Debug.Log("Ther is dirt to destroy at: " + tile.name);
                        GardenTile.SetTile(TilePos, DirtTile);
                    }
                    else
                    {
                        Debug.Log("no grass at: " + tile.name);
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
            Vector3 cellCenterPos = TilePos + new Vector3(0.5f, 0.7f, 0);
            PlantingPos = cellCenterPos;
            for (int i = 0; i < PlantsContainer.transform.childCount; i++)
            {
                if (PlantsContainer.transform.GetChild(i).transform.position != cellCenterPos)
                {
                    if (GardenTile.GetTile(TilePos) == DirtTile)
                    {
                        Debug.Log("Planting Plant");
                        Planting();
                    }

                }
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
                    DestroyPlant(PlantsContainer.transform.GetChild(i).gameObject);
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
                    GardenTile.SetTile(PosTile, DirtPlantTile);
                }
            }
        }



    }
    public void Planting()
    {
        GameObject PlantingPlant = Instantiate(inventoryManger.CurrentItem.ObjectInHand);
        Vector3Int tilepos = GardenTile.WorldToCell(PlantingPos);
        GardenTile.SetTile(tilepos, DirtPlantTile); //setzt die neue tile
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

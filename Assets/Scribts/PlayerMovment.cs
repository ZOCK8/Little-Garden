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
    [SerializeField] private GameObject PlantsContainer;
    private InputSystem_Actions InputSystem;
    private Rigidbody2D PlayerRb;
    private BoxCollider2D PlayerBc;
    private TilemapCollider2D TCGarden;
    private Vector2 moveInput;
    public InventoryManger inventoryManger;
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
            if (GardenTile.GetTile(TilePos) == DirtTile)
            {
                Debug.Log("Planting Plant");
                Planting();
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


    }
    public void Planting()
    {
        Vector3Int TilePos = GardenTile.WorldToCell(PlayerObject.transform.position);
        Vector3 cellCenterPos = TilePos + new Vector3(0.5f, 0.5f, 0);
        GameObject PlantingPlant = Instantiate(inventoryManger.CurrentItem.ObjectInHand);
        PlantingPlant.transform.position = cellCenterPos;
        if (PlantingPlant.GetComponent<Plants>().items == null)
        {
            PlantingPlant.GetComponent<Plants>().items = inventoryManger.CurrentItem;
        }
        else
        {
            Debug.LogError("The current Item has no Items Pleas add!");
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

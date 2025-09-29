using System;
using System.Net.NetworkInformation;
using NUnit.Framework.Internal.Commands;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.InferenceEngine;
public class PlayerMovment : MonoBehaviour
{
    [SerializeField] public GameObject PlayerObject;
    [SerializeField] private Tilemap GardenTile;
    [SerializeField] private RuleTile DirtTile;
    [SerializeField] private GameObject PlantsContainer;
    [SerializeField] private GameObject BuildingContainer;
    [SerializeField] private GameObject AddItemAnimator;
    [SerializeField] private DialogText dialogText;
    [SerializeField] private GameObject Sounds;


    private InputSystem_Actions InputSystem;
    private Rigidbody2D PlayerRb;
    private BoxCollider2D PlayerBc;
    private TilemapCollider2D TCGarden;
    private Vector2 moveInput;
    public InventoryManger inventoryManger;
    private Vector3 PlantingPos;
    public NightDay nightDay;
    public bool Planted;

    public Vector2 MoveInput { get; private set; }
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
        Planted = false;
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
        if (moveInput != Vector2.zero)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                PlayerObject.GetComponent<Animator>().Play("PlayerWalking Side");
                if (!Sounds.transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
                {
                    Sounds.transform.GetChild(0).GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                if (moveInput.y > 0)
                {
                    PlayerObject.GetComponent<Animator>().Play("PlayerWalking up");
                    if (!Sounds.transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
                    {
                        Sounds.transform.GetChild(0).GetComponent<AudioSource>().Play();
                    }
                }
                if (moveInput.y < 0)
                {
                    PlayerObject.GetComponent<Animator>().Play("PlayerWalking Side");
                    if (!Sounds.transform.GetChild(0).GetComponent<AudioSource>().isPlaying)
                    {
                        Sounds.transform.GetChild(0).GetComponent<AudioSource>().Play();
                    }
                }
            }
        }

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
            if (!Sounds.transform.GetChild(1).GetComponent<AudioSource>().isPlaying)
            {
                Sounds.transform.GetChild(1).GetComponent<AudioSource>().Play();
            }
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
                inventoryManger.ItemParent.transform.GetChild(0).GetComponent<Animator>().Play("Shovel");
                Debug.Log("is touching grass");
                if (inventoryManger.CurrentItem.name == "Shovel")
                {
                    Vector3Int TilePos = GardenTile.WorldToCell(PlayerObject.transform.position);
                    TileBase tile = GardenTile.GetTile(TilePos);
                    if (!Sounds.transform.GetChild(3).GetComponent<AudioSource>().isPlaying)
                    {
                        Sounds.transform.GetChild(3).GetComponent<AudioSource>().Play();
                    }
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

            for (int i = 0; i < PlantsContainer.transform.childCount; i++)
            {
                if (PlantsContainer.transform.GetChild(i).position == cellCenterPos)
                {
                    alreadyUsed = true;
                    break; // reicht, eine Pflanze gefunden → abbrechen
                }
            }

            if (!alreadyUsed && GardenTile.GetTile(TilePos)?.name == "FarmLand" && !inventoryManger.CurrentItem.IsDoneGrowing)
            {
                GameObject Plant = Instantiate(inventoryManger.CurrentItem.ObjectInHand);
                inventoryManger.ClearCurrentItem();
                Debug.Log("Planting Plant");
                Planting(Plant);
            }
            else if (inventoryManger.CurrentItem.IsDoneGrowing)
            {
                Debug.Log("Cant planting because the item is not a seed");
                dialogText.Text.color = Color.red;
                dialogText.TextToDisplay = "This is not a seed";
                StartCoroutine(dialogText.ShowText());
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
                    inventoryManger.ItemParent.transform.GetChild(0).GetComponent<Animator>().Play("Sickle");
                    if (!Sounds.transform.GetChild(4).GetComponent<AudioSource>().isPlaying)
                    {
                        Sounds.transform.GetChild(4).GetComponent<AudioSource>().Play();
                    }
                    var Plant = PlantsContainer.transform.GetChild(i).gameObject;
                    if (Plant.GetComponent<Plants>().IsDone)
                    {
                        AddItemAnimator.GetComponent<Animator>().SetTrigger("AddItem");
                        AddItemAnimator.transform.GetChild(0).GetComponent<Image>().sprite = Plant.GetComponent<Plants>().items.ShowcaseImage;
                        AddItemAnimator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "+";
                        AddItemAnimator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.green;
                        nightDay.PlantsHarvested += 1;
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
                    if (!Sounds.transform.GetChild(2).GetComponent<AudioSource>().isPlaying)
                    {
                        Sounds.transform.GetChild(2).GetComponent<AudioSource>().Play();
                    }
                    inventoryManger.ItemParent.transform.GetChild(0).GetComponent<Animator>().Play("Watering");
                    PlantsContainer.transform.GetChild(i).GetComponent<Plants>().WasWatered = true;
                    Vector3Int PosTile = GardenTile.WorldToCell(PlantsContainer.transform.GetChild(i).transform.position);
                    GardenTile.SetColor(PosTile, Color.navajoWhite);
                }
            }
        }



    }
    public void Planting(GameObject PlantingPlant)
    {
        Planted = true;
        if (!Sounds.transform.GetChild(1).GetComponent<AudioSource>().isPlaying)
        {
            Sounds.transform.GetChild(1).GetComponent<AudioSource>().Play();
        }
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
            Items instanceData = ScriptableObject.CreateInstance<Items>();
            Items original = PlantToDestroy.GetComponent<Plants>().items;
            instanceData.IsDoneGrowing = true;
            instanceData.name = "Grown" + original.name;
            instanceData.ShowcaseImage = original.GrownItem;
            instanceData.SellPrice = original.SellPrice * 2;
            instanceData.ItemName = instanceData.name;

            inventoryManger.ItemsInInv.Add(instanceData);
        }
        Destroy(PlantToDestroy.gameObject, 0.2f);
    }
}

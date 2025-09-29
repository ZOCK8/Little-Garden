using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DialogText : MonoBehaviour
{
    [SerializeField] private GameObject DialgTextGameOjekt;
    [SerializeField] private InventoryManger inventoryManger;
    [SerializeField] private PlayerMovment playerMovment;
    public TextMeshProUGUI Text;
    public string TextToDisplay;
    private Animator aniamtion;
    private InputSystem_Actions InputSystem;
    void Awake()
    {
        InputSystem = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        InputSystem.Enable(); // Input System aktivieren
    }

    private void OnDisable()
    {
        InputSystem.Disable(); // Input System deaktivieren
        InputSystem.Player.Disable();
    }
    void Start()
    {
        aniamtion = DialgTextGameOjekt.GetComponent<Animator>();
        StartCoroutine(Tutorial());

    }
    IEnumerator Tutorial()
    {
        TextToDisplay = "Welcome! Use W/A/S/D to move.";
        yield return StartCoroutine(ShowText());
        yield return new WaitUntil(() => InputSystem.Player.Move.WasPerformedThisFrame() && Text.text == TextToDisplay);

        TextToDisplay = "Open your inventory and select a seed.";
        yield return StartCoroutine(ShowText());
        yield return new WaitUntil(() => inventoryManger.CurrentItem.ItemType == ItemTypeEnum.Plant);

        TextToDisplay = "Plant the seed with E or use your shovel to create a field.";
        yield return StartCoroutine(ShowText());
        yield return new WaitUntil(() => playerMovment.Planted);

        TextToDisplay = "Place the scarecrow from your inventory. It protects against crows.";
        yield return StartCoroutine(ShowText());
        yield return new WaitUntil(() => inventoryManger.CurrentItem.ItemType == ItemTypeEnum.Buildings);

        TextToDisplay = "Great! Try your other tools and explore the farm.";
        yield return StartCoroutine(ShowText());
    }
    public IEnumerator ShowText()
    {
        Text.text = "";
        aniamtion.Play("FadeIn");

        for (int i = 0; i < TextToDisplay.Length; i++)
        {
            Text.text += TextToDisplay[i];
            yield return new WaitForSeconds(0.03f);
        }
    }

}

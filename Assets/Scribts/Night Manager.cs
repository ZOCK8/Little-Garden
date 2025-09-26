using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NightDay : MonoBehaviour
{
    [SerializeField] public int day;
    [SerializeField] private Tile DirtTile;
    [SerializeField] private GameObject NightUI;
    [SerializeField] private GameObject PlantsContainer;
    [SerializeField] private Tilemap GroundTileMap;
    [SerializeField] private Button EndDayButton;
    private bool EndDay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NightUI.SetActive(false);
        StartCoroutine(NightCooldown());
        EndDayButton.onClick.AddListener(() =>
        {
            EndDay = true;
        });
    }

    // Update is called once per frame
    void Update()
    {


    }
    IEnumerator NightCooldown()
    {
        NightUI.SetActive(false);
        yield return new WaitForSeconds(300);
        Debug.Log("Day Has Ended");
        day += 1;
        NightUI.SetActive(true);
        for (int i = 0; i < PlantsContainer.transform.childCount; i++)
        {
            Vector3Int TilePos = GroundTileMap.WorldToCell(PlantsContainer.transform.GetChild(i).gameObject.transform.position);
            GroundTileMap.SetTile(TilePos, DirtTile);
            if (PlantsContainer.transform.GetChild(i).GetComponent<Plants>().WasWatered == false)
            {
                PlantsContainer.transform.GetChild(i).GetComponent<Plants>().GrowStatus -= 1;
            }
            else
            {
                PlantsContainer.transform.GetChild(i).GetComponent<Plants>().GrowStatus += 1;
            }
        }
        yield return new WaitUntil(() =>
        {
            EndDay = true;
        });
        StartCoroutine(NightCooldown());
    }
}

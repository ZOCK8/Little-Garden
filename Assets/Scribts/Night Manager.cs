using System.Collections;
using UnityEngine;

public class NightDay : MonoBehaviour
{
    [SerializeField] public int day;
    [SerializeField] private GameObject NightUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


    }
    IEnumerator NightCooldown()
    {
        yield return new WaitForSeconds(300);
        Debug.Log("Day Has Ended");
        day += 1;
        NightUI.SetActive(true);
    }
}

using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogText : MonoBehaviour
{
    [SerializeField] private GameObject DialgTextGameOjekt;
    public TextMeshProUGUI Text;
    public string TextToDisplay;
    private Animator aniamtion;
    void Start()
    {
        aniamtion = DialgTextGameOjekt.GetComponent<Animator>();
        StartCoroutine(ShowText());

    }

    // Update is called once per frame
    public IEnumerator ShowText()
    {
        Text.text = null;
        aniamtion.Play("FadeIn");
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < TextToDisplay.Count(); i++)
        {
            yield return new WaitForSeconds(0.03f);
            Text.text += TextToDisplay[i];
        }
    }
}

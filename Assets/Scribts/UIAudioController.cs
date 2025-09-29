using UnityEngine;
using UnityEngine.UI;

public class UIAudioController : MonoBehaviour
{
    [Header("UI Elemente")]
    public Button ActivateSliderButton;
    public Button DeactivateObjectButton;
    public Slider VolumeSlider;
    public GameObject ObjectToDeactivate;

    private void Start()
    {
        ObjectToDeactivate.SetActive(false);
        ActivateSliderButton.onClick.AddListener(ActivateSlider);
        DeactivateObjectButton.onClick.AddListener(DeactivateObject);

        VolumeSlider.onValueChanged.AddListener(SetGlobalVolume);
    }

    private void ActivateSlider()
    {
        ObjectToDeactivate.SetActive(true);
    }

    private void DeactivateObject()
    {
        if (ObjectToDeactivate != null)
            ObjectToDeactivate.SetActive(false);
    }

    private void SetGlobalVolume(float value)
    {
        AudioListener.volume = value;
    }
}

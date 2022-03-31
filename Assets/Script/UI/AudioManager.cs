using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string VolumePref = "VolumePref";
    private int firstPlayInt;
    [SerializeField]
    private AudioSource volumeAudio;
    [SerializeField]
    private Slider volumeSlider;
    private float volumeFloat;


    // Start is called before the first frame update
    void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if(firstPlayInt == 0)
        {
            volumeFloat = 1f;
            volumeSlider.value = volumeFloat;
            PlayerPrefs.SetFloat(VolumePref, volumeFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else
        {
            volumeFloat = PlayerPrefs.GetFloat(VolumePref);
        }
    }

    public void SaveVolumeValue()
    {
        PlayerPrefs.SetFloat(VolumePref, volumeSlider.value);
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            SaveVolumeValue();
        }
    }

    public void UpdateSound()
    {
        AudioListener.volume = volumeSlider.value;
    }
}

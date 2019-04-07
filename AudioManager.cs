using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[System.Serializable]
public enum AudioSoundType
{
    Oof,
    Thud,
    Hop,
    CoinCollect,
    WingFlap,
    StopWingFlap,
    Bird,
    GravSwapUp,
    GravSwapDown,
    Pause,
    Hit,
    Monster,
    BreakCrate,
    Bubble,
    ChallengeCompleted,
    GetMagnet,
    LoseShield,
    RollingRock
}

public enum BeachSoundType 
{
    ClickOnElement,
    BackFromElement,
    ShopSelection,
    BuyFromShop,
    CantBuy,
    ConvertTreasure,
    StopConvertTreasure,
    EggHatch,
    DepartOnRun,
    Monster,
    ChangeColor,
    ChallengeComplete
}

public class AudioManager : MonoBehaviour {

    public AudioMixer masterMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup frootGroup;

    public AudioMixerSnapshot unpaused;
    public AudioMixerSnapshot paused;

    public Slider settingsMenuMusicSlider;
    public Slider settingsMenuSfxSlider;

    
    public virtual void Start()
    {
        if (settingsMenuMusicSlider.maxValue < PlayerPrefs.GetFloat("Music Audio"))
        {
            settingsMenuMusicSlider.value = settingsMenuMusicSlider.maxValue;
            masterMixer.SetFloat("musicVol", settingsMenuMusicSlider.value);
        }
        else
        {
            settingsMenuMusicSlider.value = PlayerPrefs.GetFloat("Music Audio");
            masterMixer.SetFloat("musicVol", settingsMenuMusicSlider.value);
        }

        if (settingsMenuSfxSlider.maxValue < PlayerPrefs.GetFloat("SFX Audio"))
        {
            settingsMenuSfxSlider.value = settingsMenuSfxSlider.maxValue;
            masterMixer.SetFloat("sfxVol", settingsMenuSfxSlider.value);
        }
        else
        {
            settingsMenuSfxSlider.value = PlayerPrefs.GetFloat("SFX Audio");
            masterMixer.SetFloat("sfxVol", settingsMenuSfxSlider.value);
        }

    }

    public void SaveMusicAudioLevels()
    {
        masterMixer.SetFloat("musicVol", settingsMenuMusicSlider.value);
        PlayerPrefs.SetFloat("Music Audio", settingsMenuMusicSlider.value);
    }

    public void SaveSFXAudioLevels()
    {
        masterMixer.SetFloat("sfxVol", settingsMenuSfxSlider.value);
        PlayerPrefs.SetFloat("SFX Audio", settingsMenuSfxSlider.value);
    }

    public AudioSource FindSource(List<AudioSource> sources)
    {
        int index = 0;
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                break;
            }
            index++;
        }
        if (index < sources.Count)
        {
            return sources[index];
        }
        return null;
    }

    public float GetRandomPitch(float min, float max)
    {
        return Random.Range(min, max);
    }
}

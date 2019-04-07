using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachAudioManager : AudioManager {
    public static BeachAudioManager bAudioInstance;

    [Header("Music")]
    public List<AudioClip> backgroundMusic;
    public AudioClip beachIntro;
    public AudioClip beachLoop;

    [Header("SFX")]
    public AudioClip clickOnElementSFX;
    public AudioClip backFromElementSFX;
    public AudioClip shopSelectionSFX;
    public AudioClip buyFromShopSFX;
    public AudioClip cantBuySFX;
    public AudioClip convertTreasureSFX;
    public AudioClip eggHatchSFX;
    public AudioClip departOnRunSFX;
    public AudioClip changeColorSFX;
    public AudioClip challengeCompleteSFX;

    List<AudioSource> sources;
    AudioSource convertTreasureSource;
    AudioSource cantBuySource;
    bool soundOneTaken;
    bool soundTwoTaken;

	void Awake()
	{
        if (bAudioInstance == null)
        {
            bAudioInstance = this;
        }
        
        sources = new List<AudioSource>(GetComponents<AudioSource>());
	}

	public override void Start()
	{
        base.Start();
        StartCoroutine("StartMainMusicLoop");
	}

    //Plays the music for the beach scene
    IEnumerator StartMainMusicLoop()
    {
        AudioSource source = FindSource(sources);
        source.outputAudioMixerGroup = musicGroup;
        source.clip = beachIntro;
        source.volume = 0.65f;
        source.Play();

        float fadeInTime = 0f;
        while (source.volume < 1f)
        {
            fadeInTime += Time.deltaTime;
            source.volume += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(beachIntro.length - fadeInTime);
        float audioTime = 0;
        float previousDSPTime = (float)AudioSettings.dspTime;
        while (audioTime < beachIntro.length - fadeInTime)
        {
            yield return null;
            audioTime += (float)AudioSettings.dspTime - previousDSPTime;
            previousDSPTime = (float)AudioSettings.dspTime;
        }

        AudioSource otherSource = FindSource(sources);
        otherSource.outputAudioMixerGroup = musicGroup;
        otherSource.clip = beachLoop;
        otherSource.volume = 0.7f;

        otherSource.pitch = 1f;
        otherSource.loop = true;
        otherSource.Play();
    }

    public void PlayBGMusic(AudioClip clip) 
    {
        // For now, play only background music. Can modify if we plan on having more
        sources[0].PlayOneShot(backgroundMusic[0]);         
    }

    IEnumerator FadeMusicForSFX(AudioClip clip, AudioSource source) 
    {
        float startVolume = sources[0].volume;
        float endVolume = 0;
        float timeOfLerp = 0;
        float percentageLerped = 0;

        while (timeOfLerp <= 0.5f)
        {
            sources[0].volume = Mathf.Lerp(startVolume, endVolume, percentageLerped);
            timeOfLerp += Time.deltaTime;
            percentageLerped = timeOfLerp / 0.5f;
            yield return null;
        }

        source.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        timeOfLerp = 0;
        percentageLerped = 0;

        while (timeOfLerp <= 0.5f) 
        {
            sources[0].volume = Mathf.Lerp(endVolume, startVolume, percentageLerped);
            timeOfLerp += Time.deltaTime;
            percentageLerped = timeOfLerp / 0.5f;
            yield return null;
        }
        sources[0].volume = startVolume;
    }

    static public void PlaySound(BeachSoundType type, AudioClip clip = null) 
    {
        switch (type) 
        {
            case BeachSoundType.ClickOnElement:
                bAudioInstance.PlayClickOnElement();
                break;
            case BeachSoundType.BackFromElement:
                bAudioInstance.PlayBackFromElement();
                break;
            case BeachSoundType.ShopSelection:
                bAudioInstance.PlayShopSelection();
                break;
            case BeachSoundType.BuyFromShop:
                bAudioInstance.PlayBuyFromShop();
                break;
            case BeachSoundType.CantBuy:
                bAudioInstance.PlayCantBuy();
                break;
            case BeachSoundType.ConvertTreasure:
                bAudioInstance.PlayConvertTreasure();
                break;
            case BeachSoundType.StopConvertTreasure:
                bAudioInstance.StopConvertTreasure();
                break;
            case BeachSoundType.EggHatch:
                bAudioInstance.PlayEggHatch();
                break;
            case BeachSoundType.DepartOnRun:
                bAudioInstance.PlayDepartOnRun();
                break;
            case BeachSoundType.Monster:
                bAudioInstance.PlayMonster(clip);
                break;
            case BeachSoundType.ChangeColor:
                bAudioInstance.PlayChangeColor();
                break;
            case BeachSoundType.ChallengeComplete:
                bAudioInstance.PlayChallengeComplete();
                break;
        }
    }

    void PlayMonster(AudioClip clip)
    {
        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = 1f;
            source.PlayOneShot(clip);
        }
    }

    void PlayClickOnElement() 
    {
        AudioSource source = FindSource(sources);
        if (source != null) 
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = GetRandomPitch(0.95f, 1.05f);
            source.PlayOneShot(clickOnElementSFX);
        }
    }

    void PlayBackFromElement() 
    {
        AudioSource source = FindSource(sources);
        if (source != null) 
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = GetRandomPitch(0.95f, 1.05f);
            source.PlayOneShot(backFromElementSFX);
        }
    }

    void PlayShopSelection() 
    {
        AudioSource source = FindSource(sources);
        if (source != null) 
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = GetRandomPitch(0.8f, 1f);
            source.PlayOneShot(shopSelectionSFX);
        }
    }

    void PlayBuyFromShop() 
    {
        AudioSource source = FindSource(sources);
        if (source != null) 
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = GetRandomPitch(0.9f, 1.1f);
            source.PlayOneShot(buyFromShopSFX);
        }
    }

    void PlayCantBuy() 
    {
        if (cantBuySource != null) 
        {
            if (!cantBuySource.isPlaying) 
            {
                AudioSource source = FindSource(sources);
                if (source != null)
                {
                    source.outputAudioMixerGroup = sfxGroup;
                    cantBuySource = source;
                    source.pitch = 1f;
                    source.PlayOneShot(cantBuySFX);
                } 
            } 
        } else {
            AudioSource source = FindSource(sources);
            if (source != null)
            {
                source.outputAudioMixerGroup = sfxGroup;
                cantBuySource = source;
                source.pitch = 1f;
                source.PlayOneShot(cantBuySFX);
            } 
        }

    }

    void PlayConvertTreasure() 
    {
        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            convertTreasureSource = source;
            source.loop = true;
            source.clip = convertTreasureSFX;
            source.pitch = 1f;
            source.Play();
        }
    }

    void StopConvertTreasure() 
    {
        convertTreasureSource.loop = false;
        convertTreasureSource.Stop();
    }

    void PlayEggHatch() 
    {
        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = 1f;
            source.PlayOneShot(eggHatchSFX);
        }
    }

    void PlayChangeColor()
    {
        if (!BeachManager.inCustomizer) return;

        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = 1f;
            source.PlayOneShot(changeColorSFX);
        }
    }

    void PlayDepartOnRun() 
    {
        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = 1f;
            source.PlayOneShot(departOnRunSFX);
        }
    }

    void PlayChallengeComplete() 
    {
        AudioSource source = FindSource(sources);
        if (source != null)
        {
            source.outputAudioMixerGroup = sfxGroup;
            source.pitch = 1f;
            StartCoroutine(FadeMusicForSFX(challengeCompleteSFX, source));
        }
    }
}

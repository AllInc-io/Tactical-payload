using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    //here is for all the sounds that shouldnt stop when reloading the scenes 
    //sfx can be player in more "local" audiosources as well


    public static SoundManager get;
    private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;

    public AudioClip ambientTrack;
    public AudioClip bossTrack;

    private int currentlyPlayingSoundIndex;

    [SerializeField] private AudioClip swipeSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip starUnlockedSound;
    [SerializeField] private AudioClip clearedLevelSound;


    public AudioClip[] backgroundSounds;

    private void Awake()
    {
        if (SoundManager.get == null)
        {
            get = this;
            DontDestroyOnLoad(get);
        }
        else Destroy(this.gameObject);

        audioSource = GetComponent<AudioSource>();


    }

    public void TurnOff()
    {
        musicSource.Pause();
    }

    public void TurnOn()
    {
        if(!musicSource.isPlaying)
            musicSource.Play();
    }


    private void Update()
    {
        /*
        if (!musicSource.isPlaying && R.get.hasMusic)
        {
            currentlyPlayingSoundIndex++;
            PlayMusic(ambientSounds[currentlyPlayingSoundIndex % ambientSounds.Count]);
        }
        */
    }

    public void FadeMusicToBoss()
    {
        if(musicSource.clip == bossTrack)
            return;

        StartCoroutine(AnimFadeMusicTo(bossTrack));
    }

    public void FadeMusicToAmbient()
    {

        if(musicSource.clip == ambientTrack)
            return;

        StartCoroutine(AnimFadeMusicTo(ambientTrack));
    }

    public void ChangeAmbientMusic(AudioClip clip)
    {
        if (ambientTrack == clip) return;

        ambientTrack = clip;
        if (!musicSource.isPlaying)
        {
            musicSource.clip = clip;
            float originalVolume = musicSource.volume;
            musicSource.volume = 0;
            musicSource.DOFade(originalVolume, 0.35f).SetEase(Ease.InQuad);
            if (R.get.hasMusic) musicSource.Play();
        }
        else StartCoroutine(AnimFadeMusicTo(clip));

    }


    IEnumerator AnimFadeMusicTo(AudioClip clip)
    {
        float currentVolume = musicSource.volume;
        
        musicSource.DOFade(0f, 0.35f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.35f);

        musicSource.clip = clip;

        if(R.get.hasMusic)
            musicSource.Play();

        musicSource.DOFade(currentVolume, 0.35f).SetEase(Ease.OutQuad);
    }

    public void PlaySwipeSound()
    {
        if(R.get.hasSFX) audioSource.PlayOneShot(swipeSound);
    }

    public void PlayWinSound()
    {
        if (R.get.hasSFX) audioSource.PlayOneShot(winSound);
    }
    public void PlayGameOverSound()
    {
        if (R.get.hasSFX) audioSource.PlayOneShot(loseSound);
    }

    public void PlayClearedLevelSound()
    {
        if (R.get.hasSFX) audioSource.PlayOneShot(clearedLevelSound);
    }

    public void PlayUnlockedStarSound()
    {
        if (R.get.hasSFX) audioSource.PlayOneShot(starUnlockedSound);
    }

}

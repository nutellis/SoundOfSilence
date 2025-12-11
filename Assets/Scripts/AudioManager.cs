using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip fightMusic;
    public AudioClip bossFightMusic;

    [Header("Music Volume Settings (0-1)")]
    [Range(0f, 1f)] public float backgroundMusicVolume = 0.3f;
    [Range(0f, 1f)] public float fightMusicVolume = 0.5f;
    [Range(0f, 1f)] public float bossFightMusicVolume = 0.6f;

    [Header("Sound Effects")]
    public AudioClip battleBeginSFX;
    public AudioClip[] playerDamageSounds;
    public AudioClip lossLongSFX;
    public AudioClip jumpSFX;
    public AudioClip fireCrackleSFX;
    public AudioClip fireBurstSFX;
    public AudioClip explosionSFX;

    [Header("SFX Volume Settings (0-1)")]
    [Range(0f, 1f)] public float battleBeginVolume = 1.0f;
    [Range(0f, 1f)] public float playerDamageVolume = 0.8f;
    [Range(0f, 1f)] public float lossLongVolume = 1.0f;
    [Range(0f, 1f)] public float jumpVolume = 0.7f;
    [Range(0f, 1f)] public float fireCrackleVolume = 0.8f;
    [Range(0f, 1f)] public float fireBurstVolume = 0.8f;
    [Range(0f, 1f)] public float explosionVolume = 0.9f;

    [Header("Footsteps")]
    public AudioClip[] footstepSounds;
    [Range(0f, 1f)] public float footstepVolume = 0.4f;
    public float footstepInterval = 0.5f; // Time between footsteps

    [Header("Enemy Footsteps")]
    public AudioClip[] enemyFootstepSounds;
    [Range(0f, 1f)] public float enemyFootstepVolume = 0.5f;
    public float enemyFootstepInterval = 0.6f; // Time between enemy footsteps

    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource footstepSource;

    private bool isInBattle = false;
    private float footstepTimer = 0f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupAudioSources()
    {
        // Create music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Create SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        // Create footstep source
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = false;
        footstepSource.playOnAwake = false;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !isInBattle)
        {
            if (musicSource.clip != backgroundMusic)
            {
                musicSource.clip = backgroundMusic;
                musicSource.volume = backgroundMusicVolume;
                musicSource.Play();
            }
        }
    }

    public void StartBattle()
    {
        isInBattle = true;

        // Play battle begin sound effect
        if (battleBeginSFX != null)
        {
            musicSource.volume = backgroundMusicVolume * 0.5f;
            sfxSource.PlayOneShot(battleBeginSFX, battleBeginVolume);
            musicSource.Stop();
        }

        // Wait for battle begin SFX to finish, then start fight music
        if (fightMusic != null)
        {
            float delay = battleBeginSFX != null ? battleBeginSFX.length - 2f : 0f;
            StartCoroutine(PlayFightMusicAfterDelay(delay));
        }
    }

    IEnumerator PlayFightMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicSource.Stop();
        musicSource.clip = fightMusic;
        musicSource.volume = fightMusicVolume;
        musicSource.Play();
    }

    public void EndBattle()
    {
        isInBattle = false;

        // Return to background music
        musicSource.Stop();
        musicSource.clip = backgroundMusic;
        musicSource.volume = backgroundMusicVolume;
        musicSource.Play();
    }

    public void PlayPlayerDamage()
    {
        if (playerDamageSounds != null && playerDamageSounds.Length > 0)
        {
            // Play random player damage sound
            int randomIndex = Random.Range(0, playerDamageSounds.Length);
            sfxSource.PlayOneShot(playerDamageSounds[randomIndex], playerDamageVolume);
        }
    }

    public void PlayPlayerDeath()
    {
        if (lossLongSFX != null)
        {
            // Stop all music
            musicSource.Stop();

            // Play death sound
            sfxSource.PlayOneShot(lossLongSFX, lossLongVolume);
        }
    }

    public void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0)
        {
            // Play random footstep sound
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstepSource.PlayOneShot(footstepSounds[randomIndex], footstepVolume);
        }
    }

    public void UpdateFootsteps(bool isWalking, float deltaTime)
    {
        if (isWalking)
        {
            footstepTimer += deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    public void PlayJump()
    {
        if (jumpSFX != null)
        {
            sfxSource.PlayOneShot(jumpSFX, jumpVolume);
        }
    }

    public void PlayFireCrackle()
    {
        if (fireCrackleSFX != null)
        {
            sfxSource.PlayOneShot(fireCrackleSFX, fireCrackleVolume);
        }
    }

    public void PlayFireBurst()
    {
        if (fireBurstSFX != null)
        {
            sfxSource.PlayOneShot(fireBurstSFX, fireBurstVolume);
        }
    }

    public void PlayExplosion()
    {
        if (explosionSFX != null)
        {
            sfxSource.PlayOneShot(explosionSFX, explosionVolume);
        }
    }

    public void PlayEnemyFootstep(Vector3 enemyPosition, Vector3 playerPosition)
    {
        if (enemyFootstepSounds != null && enemyFootstepSounds.Length > 0)
        {
            // Calculate distance-based volume (closer = louder)
            float distance = Vector3.Distance(enemyPosition, playerPosition);
            float maxDistance = 20f; // Maximum distance to hear footsteps
            float volumeMultiplier = Mathf.Clamp01(1f - (distance / maxDistance));

            // Play random enemy footstep sound with distance-adjusted volume
            int randomIndex = Random.Range(0, enemyFootstepSounds.Length);
            footstepSource.PlayOneShot(enemyFootstepSounds[randomIndex], enemyFootstepVolume * volumeMultiplier);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
        footstepSource.volume = Mathf.Clamp01(volume * 0.6f);
    }
}

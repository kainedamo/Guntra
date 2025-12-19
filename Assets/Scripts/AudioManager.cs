using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips")]
    public AudioClip playerShootClip;
    public AudioClip enemyShootClip;
    public AudioClip enemyDeathClip;
    public AudioClip bossDeathClip;
    public AudioClip playerHurtClip;
    public AudioClip powerupClip;
    public AudioClip bgmClip;

    [Header("Volumes")]
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Pitch Variation")]
    [Range(0f, 0.3f)] public float pitchVariation = 0.1f;

    private AudioSource[] sfxSources;
    private int nextSourceIndex = 0;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists restarts

            // Pool 8 SFX sources
            sfxSources = new AudioSource[8];
            for (int i = 0; i < sfxSources.Length; i++)
            {
                sfxSources[i] = gameObject.AddComponent<AudioSource>();
                sfxSources[i].playOnAwake = false;
                sfxSources[i].loop = false;
            }

            //Music
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.clip = bgmClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float volumeMod = 1f)
    {
        if (clip == null) return;

        AudioSource source = sfxSources[nextSourceIndex];
        source.clip = clip;
        source.volume = sfxVolume * volumeMod;
        source.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        source.Play();
        nextSourceIndex = (nextSourceIndex + 1) % sfxSources.Length;
    }

    public void PlayRandomDeath() => PlaySFX(enemyDeathClip, 0.9f); // Call from Enemy.Die()
}
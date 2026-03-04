using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ModularCollisionSound : MonoBehaviour
{
    public bool isTrigger = false;

    public enum TagMode { Single, Multiple }
    public TagMode tagMode = TagMode.Single;
    public string singleTag = "Player";
    public string[] multipleTags;

    public enum SoundMode { Single, Random }
    public SoundMode soundMode = SoundMode.Single;
    public AudioClip singleSound;
    public AudioClip[] randomSounds;

    [Header("Modularity Options")]
    public float volumeMin = 0.8f;
    public float volumeMax = 1.0f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;
    public float cooldownTime = 0.1f;

    private AudioSource audioSource;
    private float nextAllowedTime = 0f;

    void Reset()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isTrigger)
        {
            CheckAndPlay(collision.gameObject.tag);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
        {
            CheckAndPlay(other.gameObject.tag);
        }
    }

    void CheckAndPlay(string objectTag)
    {
        if (Time.time < nextAllowedTime) return;

        bool isValidTag = false;

        if (tagMode == TagMode.Single)
        {
            if (objectTag == singleTag) isValidTag = true;
        }
        else
        {
            for (int i = 0; i < multipleTags.Length; i++)
            {
                if (objectTag == multipleTags[i])
                {
                    isValidTag = true;
                    break;
                }
            }
        }

        if (isValidTag)
        {
            PlayAudio();
            nextAllowedTime = Time.time + cooldownTime;
        }
    }

    void PlayAudio()
    {
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);

        if (soundMode == SoundMode.Single && singleSound != null)
        {
            audioSource.PlayOneShot(singleSound);
        }
        else if (soundMode == SoundMode.Random && randomSounds.Length > 0)
        {
            int index = Random.Range(0, randomSounds.Length);
            if (randomSounds[index] != null)
            {
                audioSource.PlayOneShot(randomSounds[index]);
            }
        }
    }
}
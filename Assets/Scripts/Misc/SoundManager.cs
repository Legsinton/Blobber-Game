using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Player
{
    public class SoundFXManager : MonoBehaviour
    {
        public static SoundFXManager Instance;

        [SerializeField] private AudioSource soundFXObject;
        [SerializeField] private AudioSource[] loopFXSources;  // Array to hold multiple looping audio sources
        private Dictionary<SoundType, object> soundFXDict;
        [SerializeField] private List<SoundVolumeEntry> soundVolumeList;
        private Dictionary<SoundType, float> soundVolumeDict;

        private float timer = 0;
        private const float MinTime = 0.05f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
         
            InitializeSounds();
          
        }
        [System.Serializable]
        public class SoundVolumeEntry
        {
            public SoundType soundType;  // The sound effect type (key)
            public float volume;         // The volume level (value)
        }

        private void OnValidate()
        {
            if (soundVolumeList != null)
            {
                soundVolumeDict = new Dictionary<SoundType, float>();
                foreach (var entry in soundVolumeList)
                {
                    soundVolumeDict[entry.soundType] = entry.volume;
                }
            }
        }

        private void InitializeSounds()
        {
            soundFXDict = new Dictionary<SoundType, object>
            {
                //Single AudioClips
                { SoundType.Death, Resources.Load<AudioClip>("Sounds/Effects/Death") },
                { SoundType.Coin, Resources.Load<AudioClip>("Sounds/Effects/CoinPling") },
 
                //AudioClips
                { SoundType.Boing, Resources.LoadAll<AudioClip>("Sounds/Effects/Boing") },
                { SoundType.Smack, Resources.LoadAll<AudioClip>("Sounds/Effects/Smack") },
                { SoundType.Bonk, Resources.LoadAll<AudioClip>("Sounds/Effects/Bonk") },
                { SoundType.Splat, Resources.LoadAll<AudioClip>("Sounds/Effects/Splat") },
                { SoundType.Launch, Resources.LoadAll<AudioClip>("Sounds/Effects/Launch") }      
            };
        }

        private void Update()
        {
            timer += Time.deltaTime;
        }

        public void PlaySoundFX(SoundType type)
        {
            if (!soundFXDict.ContainsKey(type)) return;
            /*if (timer < MinTime) return;

            timer = 0;*/
            float volume = soundVolumeDict.ContainsKey(type) ? soundVolumeDict[type] : 1.0f;
            if (soundFXDict[type] is AudioClip singleClip)
            {
                soundFXObject.PlayOneShot(singleClip,volume);
            }
            else if (soundFXDict[type] is AudioClip[] clipArray)
            {
                int rand = Random.Range(0, clipArray.Length);
                soundFXObject.PlayOneShot(clipArray[rand],volume);
            }
        }

        private void SetupLoopingAudioSources()
        {
            loopFXSources = new AudioSource[3]; // Create 3 AudioSource components, for example

            // Add AudioSource components to the SoundManager GameObject
            for (int i = 0; i < loopFXSources.Length; i++)
            {
                loopFXSources[i] = gameObject.AddComponent<AudioSource>();  // Add an AudioSource to this object
                loopFXSources[i].loop = true; // Set them to loop
            }
        }

        public void StartLoop(SoundType type, float volume = 1f)
        {
            if (!soundFXDict.ContainsKey(type)) return;

            if (soundFXDict[type] is AudioClip singleClip)
            {
                foreach (var audioSource in loopFXSources)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = singleClip;
                        audioSource.loop = true;
                        audioSource.volume = volume;
                        audioSource.Play();
                        break;  // Exit once the clip starts playing to prevent overwriting
                    }
                }
            }
        }

        public void StopLoop(SoundType type)
        {
            if (!soundFXDict.ContainsKey(type)) return;

            if (soundFXDict[type] is AudioClip singleClip)
            {
                foreach (var audioSource in loopFXSources)
                {
                    if (audioSource.clip == singleClip)
                    {
                        audioSource.Stop();
                        audioSource.loop = false;
                    }
                }
            }
        }
    }

    public enum SoundType
    {
        // Array Of Sounds
        Break,
        Bonk,
        Boing,
        Coin,
        Death,
        Launch,
        Smash, 
        Smack,
        Splat
    }
}


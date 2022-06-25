using Fusion.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance => Singleton<AudioManager>.Instance;

    public AudioMixer masterMixer;
    public AudioMixer sfxMixer;
    public AudioMixer uiMixer;
    public AudioMixer musicMixer;

    public static readonly string masterVolumeParam = "Volume";
    public static readonly string sfxVolumeParam = "SFXVol";
    public static readonly string uiVolumeParam = "UIVol";
    public static readonly string musicVolumeParam = "MusicVol";

    [Header("Audio Groups")]
    [SerializeField] private List<AudioSource> PlayerAudios;
    [SerializeField] private List<AudioSource> MusicAudios;
    [SerializeField] private List<AudioSource> UIAudios;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        InitMixer();
    }

    private void InitMixer()
    {
        // Initialize Mixer pref from player saved prefs here
    }

    public void SaveMixerPrefs()
    {
        // Save Mixer pref when any changes are made
    }

    private float GetPref(string pref) => PlayerPrefs.GetFloat(pref, 1f);
    private void SetPref(string pref, float val) => PlayerPrefs.SetFloat(pref, val);

    // Play audio at a specific location
    public void Play(string clip, Vector3? position = null)
    {

    }
}

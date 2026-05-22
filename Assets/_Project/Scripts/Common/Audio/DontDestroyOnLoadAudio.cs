using UnityEngine;
using UnityEngine.Audio;

public class DontDestroyOnLoadAudio : MonoBehaviour
{
    public static DontDestroyOnLoadAudio instance;
    public AudioMixer audioMixer;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
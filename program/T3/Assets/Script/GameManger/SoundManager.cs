using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private AudioSource uiSource;
    private List<AudioSource> sfxPool = new List<AudioSource>();

    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float uiVolume = 1f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitPool();
            uiSource.spatialBlend = 0f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject sourceObj = new GameObject($"SFX_{i}");
            sourceObj.transform.parent = this.transform;

            AudioSource src = sourceObj.AddComponent<AudioSource>();
            src.spatialBlend = 1f; // 3D 사운드
            sfxPool.Add(src);
        }
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
                return src;
        }

        // 전부 사용 중이면 첫 번째 것을 강제로 재사용
        return sfxPool[0];
    }

    public void PlaySFX(AudioClip clip, Transform target)
    {
        if (clip == null || target == null) return;

        AudioSource src = GetAvailableSource();
        src.transform.position = target.position;
        src.volume = sfxVolume;
        src.PlayOneShot(clip);
    }

    public void PlaySFXUI(AudioClip clip)
    {
        if (clip == null) return;
        uiSource.volume = uiVolume;
        uiSource.PlayOneShot(clip);
    }
}

using System.Collections;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource ambience;
    [SerializeField] float fadeSeconds = 1f;

    public void PlaySceneAudio(AudioClip music, AudioClip ambienceClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(bgm, music));
        StartCoroutine(FadeTo(ambience, ambienceClip));
    }

    IEnumerator FadeTo(AudioSource source, AudioClip clip)
    {
        float startVolume = source.volume;

        // Fade out เสียงเดิม
        for (float t = 0; t < fadeSeconds; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeSeconds);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
        source.clip = clip;

        if (clip == null) yield break;

        source.loop = true;
        source.Play();

        // Fade in เสียงใหม่
        for (float t = 0; t < fadeSeconds; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, startVolume, t / fadeSeconds);
            yield return null;
        }

        source.volume = startVolume;
    }
}
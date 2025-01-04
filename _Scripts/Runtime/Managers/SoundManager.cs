using System.Collections;
using _Scripts.Runtime.Enums;
using DG.Tweening;
using FurtleGame.Singleton;
using Runtime.Data.UnityObject;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Header("Default Audio Source")] [SerializeField]
    public AudioSource audioSource;

    [Header("Glissando Audio Source")] [SerializeField]
    private AudioSource glissandoAudioSource;

    [Header("Random Pitch Audio Source")] [SerializeField]
    private AudioSource randomPitchAudioSource;

    [Header("Background Music Audio Source")] [SerializeField]
    private AudioSource backgroundMusicAudioSource;

    [Header("Ambience Music Audio Source")] [SerializeField]
    private AudioSource ambienceAudioSource;

    [Header("Sound's")] [SerializeField] private CD_GameSound COLLECTION;

    [Header("Glissando Settings")] [SerializeField]
    private float glissandoPitchRange = 2f;

    [SerializeField] private float glissandoDuration = 1f;
    [SerializeField] private float glissandoDefaultPitch = 1f;
    [SerializeField] private Coroutine glissandoCoroutine;

    public void PlaySound(GameSoundType soundType)
    {
        // if (SettingsManager.Instance.isSoundActive)
        // {
        foreach (var sound in COLLECTION.gameSoundData)
        {
            if (soundType == sound.gameSoundType)
            {
                if (sound.hasRandomPitch)
                {
                    randomPitchAudioSource.Stop();
                    randomPitchAudioSource.pitch = Random.Range(0.8f, 1.2f);
                    randomPitchAudioSource.PlayOneShot(sound.audioClip);
                    break;
                }
                else if (sound.hasGlissando)
                {
                    if (glissandoCoroutine != null)
                    {
                        StopCoroutine(glissandoCoroutine);
                    }

                    glissandoCoroutine = StartCoroutine(PlayGlissando(sound.audioClip));
                    break;
                }

                else
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(sound.audioClip);
                    break;
                }
            }
        }
        // }
    }

    private IEnumerator PlayGlissando(AudioClip clip)
    {
        float elapsedTime = 0f;
        float initialPitch = glissandoAudioSource.pitch;

        glissandoAudioSource.PlayOneShot(clip);

        while (elapsedTime < glissandoDuration)
        {
            float t = elapsedTime / glissandoDuration;
            glissandoAudioSource.pitch = Mathf.Lerp(initialPitch, initialPitch + glissandoPitchRange, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        glissandoAudioSource.pitch = glissandoDefaultPitch;
    }

    public void SetAllAudioSourcesAudioLowPassFilterLow()
    {
        float targetFrequency = 2000f;
        float duration = 1f;

        DOTween.To(() => audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency, duration);
        DOTween.To(() => randomPitchAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => randomPitchAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => glissandoAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => glissandoAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => backgroundMusicAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => backgroundMusicAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => ambienceAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => ambienceAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency, duration);
    }

    public void SetAllAudioSourcesAudioLowPassFilterHigh()
    {
        float targetFrequency = 5007.7f;
        float duration = 1f;

        DOTween.To(() => audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => audioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency, duration);
        DOTween.To(() => randomPitchAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => randomPitchAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => glissandoAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => glissandoAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => backgroundMusicAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => backgroundMusicAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency,
            duration);
        DOTween.To(() => ambienceAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency,
            x => ambienceAudioSource.GetComponent<AudioLowPassFilter>().cutoffFrequency = x, targetFrequency, duration);
    }
}
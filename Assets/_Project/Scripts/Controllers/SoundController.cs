using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SoundController : SingletonMonoBehaviour<SoundController> {

    AudioSource musicSource,
    sfxSource;

    public static bool soundMuted;

    public override void Awake() {
        base.Awake();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public static void PlayMusic(AudioClip clip, float volume) {
        if (Instance.musicSource.clip == clip)
            return;
        //instance.musicSource.clip = clip;
        //instance.musicSource.volume = volume;
        //instance.musicSource.Play();
        Instance.StartCoroutine(Instance.PlayMusicFade(clip, volume));
        Instance.musicSource.mute = soundMuted;
    }

    IEnumerator PlayMusicFade(AudioClip clip, float volume) {
        float t = 1, lastVolume = musicSource.volume;

        if (musicSource.isPlaying) {
            while (t > 0) {
                musicSource.volume = Mathf.Lerp(0, lastVolume, t);
                t -= 0.1f;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            musicSource.Stop();
        }

        musicSource.clip = clip;
        musicSource.volume = 0;
        musicSource.Play();
        t = 0;

        while (t < volume) {
            musicSource.volume = Mathf.Lerp(0, volume, t);
            t += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        musicSource.volume = volume;
    }

    public static void PlaySfx(AudioClip clip, float volume = 1f) {
        Instance.sfxSource.mute = soundMuted;
        Instance.sfxSource.clip = clip;
        Instance.sfxSource.volume = volume;
        Instance.sfxSource.Play();
    }

    public void PlaySfxNonStatic(AudioClip clip) {
        Instance.sfxSource.mute = soundMuted;
        Instance.sfxSource.clip = clip;
        Instance.sfxSource.volume = 1f;
        Instance.sfxSource.Play();
    }

    public static AudioSource PlaySfxInstance(AudioClip clip, float volume = 1f) {
        AudioSource sfxSource = Instantiate(
            Resources.Load<GameObject>("Prefabs/sfxSourcePrefab")
        ).GetComponent<AudioSource>();
        
        sfxSource.mute = soundMuted;
        sfxSource.clip = clip;
        sfxSource.volume = volume;
        sfxSource.Play();
        return sfxSource;
    }

    public static void StopMusic() {
        Instance.musicSource.Stop();
    }

    public static void FadeOut(AudioSource asrc) {
        Instance.StartCoroutine(Instance.FadeOutEnum(asrc));
    }

    IEnumerator FadeOutEnum(AudioSource asrc) {
        float t = 1, lastVolume = asrc.volume;
        if (asrc.isPlaying) {
            while (t > 0) {
                asrc.volume = Mathf.Lerp(0, lastVolume, t);
                t -= 0.1f;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            asrc.Stop();
        }
    }

    public static void Mute(bool mute = true) {
        Instance.musicSource.mute = mute;
        Instance.sfxSource.mute = mute;
    }
}

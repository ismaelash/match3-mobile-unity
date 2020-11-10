using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
    public class SoundController : Singleton<SoundController>
    {
        #region VARIABLES

        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;

        #endregion

        #region PUBLIC_METHODS

        public void PlayMusic(AudioClip clip, float volume = 1, bool loop = true)
        {
            musicAudioSource.clip = clip;
            musicAudioSource.volume = volume;
            musicAudioSource.loop = loop;
            musicAudioSource.Play();
        }

        public void StopMusic()
        {
            musicAudioSource.Stop();
        }

        public void PlaySfx(AudioClip clip, float volume = 1f, bool loop = false)
        {
            sfxAudioSource.clip = clip;
            sfxAudioSource.volume = volume;
            sfxAudioSource.loop = loop;
            sfxAudioSource.Play();
        }

        #endregion
    }
}
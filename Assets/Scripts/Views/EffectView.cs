using UnityEngine;

namespace Views
{
    public class EffectView : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private ParticleSystem _particleSystem;

        public bool IsPlaying()
        {
            return _particleSystem.isPlaying || _audioSource.isPlaying;
        }

        public void Play(float volume)
        {
            _audioSource.PlayOneShot(_audioSource.clip, volume);
            _particleSystem.Play();
        }
    }
}
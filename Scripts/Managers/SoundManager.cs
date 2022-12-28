using UnityEngine;

namespace PaperFold.Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void PlaySound(AudioClip clip, float pitch)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
    }
}
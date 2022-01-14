using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioClipPlayer : MonoBehaviour
{
    
    [SerializeField] private AudioClip[] audioClips;

    private AudioSource _audioSource;

    private void Awake () {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play () {
        _audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        _audioSource.Play();
    }
    
}

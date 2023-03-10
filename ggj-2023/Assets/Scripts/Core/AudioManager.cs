// The MIT License (MIT)
// Copyright (c) 2017 David Evans @phosphoer
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
  private Dictionary<GameObject, AudioGroup> _audioGroups = new Dictionary<GameObject, AudioGroup>();

  public class AudioInstance
  {
    public AudioSource AudioSource;
    public SoundBank SoundBank;

    private int _lastRandomClip;

    public AudioClip GetNextRandomClip()
    {
      int index = Random.Range(0, SoundBank.AudioClips.Length);
      if (index == _lastRandomClip)
      {
        index = (index + 1) % SoundBank.AudioClips.Length;
      }

      _lastRandomClip = index;
      return SoundBank.AudioClips[index];
    }
  }

  private class AudioGroup
  {
    public int InstanceCount { get { return _instanceMap.Count; } }

    private Dictionary<SoundBank, AudioInstance> _instanceMap = new Dictionary<SoundBank, AudioInstance>();

    public void AddAudioInstance(AudioInstance audioInstance)
    {
      _instanceMap.Add(audioInstance.SoundBank, audioInstance);
    }

    public void RemoveAudioInstance(AudioInstance audioInstance)
    {
      _instanceMap.Remove(audioInstance.SoundBank);
    }

    public AudioInstance GetAudioInstance(SoundBank forSoundBank)
    {
      AudioInstance audioInstance = null;
      _instanceMap.TryGetValue(forSoundBank, out audioInstance);
      return audioInstance;
    }
  }

  private void Awake()
  {
    Instance = this;
  }

  // Used if you want to fade in a sound on start but audio manager doesn't exist yet
  public static IEnumerator QueueFadeInSoundRoutine(GameObject source, SoundBank soundBank, float toVolume, float duration)
  {
    while (Instance == null)
    {
      yield return null;
    }

    Instance.FadeInSound(source, soundBank, toVolume, duration);
  }

  // Used if you want to play a sound on start but audio manager doesn't exist yet  
  public static IEnumerator QueuePlaySoundRoutine(GameObject source, SoundBank soundBank, float volumeScale = 1.0f)
  {
    while (Instance == null)
    {
      yield return null;
    }

    Instance.PlaySound(source, soundBank, volumeScale);
  }

  // Fade in a sound over a time period, if the source gets destroyed it will be cancelled
  public Coroutine FadeInSound(GameObject source, SoundBank soundBank, float duration, float toVolume = 1.0f)
  {
    AudioInstance audioInstance = PlaySound(source, soundBank, 0);
    return StartCoroutine(FadeAudioRoutine(audioInstance, 0, toVolume * soundBank.VolumeScale, duration));
  }

  // Fade out a sound over a time period, if the source gets destroyed it will be cancelled
  public Coroutine FadeOutSound(GameObject source, SoundBank soundBank, float duration, bool playIfStopped = false)
  {
    AudioInstance audioInstance = GetOrAddAudioInstance(source, soundBank);
    if (audioInstance.AudioSource == null)
      return null;

    if (!audioInstance.AudioSource.isPlaying && playIfStopped)
    {
      audioInstance.AudioSource.Play();
    }

    return StartCoroutine(FadeAudioRoutine(audioInstance, audioInstance.AudioSource.volume, 0, duration));
  }

  public void CancelFade(Coroutine fadeRoutine)
  {
    StopCoroutine(fadeRoutine);
  }

  // Play a sound on the global source (multiple simulataneous sounds supported)
  public void PlaySound(SoundBank soundBank, float volumeScale = 1.0f)
  {
    PlaySound(gameObject, soundBank, volumeScale);
  }

  // Play a sound from a specific source, use for spatial sounds or if you want to do things with the 
  // AudioInstance like modify the volume over time by accessing the unity AudioSource
  // There is always a unique audio source per simulatenous sound
  public AudioInstance PlaySound(GameObject source, SoundBank soundBank, float volumeScale = 1.0f)
  {
    AudioInstance audioInstance = GetOrAddAudioInstance(source, soundBank);
    if (audioInstance != null)
    {
      audioInstance.AudioSource.pitch = 1.0f + audioInstance.SoundBank.PitchOffset + audioInstance.SoundBank.PitchOffsetRange.RandomValue;
      audioInstance.AudioSource.volume = audioInstance.SoundBank.VolumeScale * volumeScale;

      // If the soundbank has sequenced clips we should play those in sequence instead of just playing the clip
      if (audioInstance.SoundBank.SequencedClips?.Length > 0)
      {
        StartCoroutine(PlaySequencedSoundAsync(audioInstance));
      }
      // Otherwise we just play normally
      else
      {
        audioInstance.AudioSource.clip = audioInstance.GetNextRandomClip();
        audioInstance.AudioSource.Play();
      }
    }
    else
    {
      Debug.LogWarning(string.Format("Couldn't find audio instance for {0}:{1}", source.name, soundBank.name));
    }

    return audioInstance;
  }

  private IEnumerator PlaySequencedSoundAsync(AudioInstance audioInstance)
  {
    // Play each sequenced clip in sequence and wait for it to end
    for (int i = 0; i < audioInstance.SoundBank.SequencedClips.Length && audioInstance.AudioSource != null; ++i)
    {
      var preloopClip = audioInstance.SoundBank.SequencedClips[i];
      if (audioInstance.AudioSource != null)
      {
        audioInstance.AudioSource.clip = preloopClip;
        audioInstance.AudioSource.loop = false;
        audioInstance.AudioSource.Play();
      }

      while (audioInstance.AudioSource != null && audioInstance.AudioSource.time < audioInstance.AudioSource.clip.length && audioInstance.AudioSource.isPlaying)
      {
        yield return null;
      }
    }

    // Play the final clip normally
    if (audioInstance.AudioSource != null && audioInstance.SoundBank.AudioClips.Length > 0)
    {
      audioInstance.AudioSource.loop = audioInstance.SoundBank.IsLooping;
      audioInstance.AudioSource.clip = audioInstance.GetNextRandomClip();
      audioInstance.AudioSource.Play();
    }
  }

  public AudioInstance PlaySoundClip(GameObject source, SoundBank soundBank, int clipIndex, float volumeScale = 1.0f)
  {
    AudioInstance audioInstance = GetOrAddAudioInstance(source, soundBank);
    if (audioInstance != null)
    {
      audioInstance.AudioSource.pitch = 1.0f + audioInstance.SoundBank.PitchOffset + audioInstance.SoundBank.PitchOffsetRange.RandomValue;
      audioInstance.AudioSource.volume = audioInstance.SoundBank.VolumeScale * volumeScale;
      audioInstance.AudioSource.clip = audioInstance.SoundBank.AudioClips[clipIndex];
      audioInstance.AudioSource.Play();
    }
    else
    {
      Debug.LogWarning(string.Format("Couldn't find audio instance for {0}:{1}", source.name, soundBank.name));
    }

    return audioInstance;
  }

  // Stop a currently playing sound on the global source
  public void StopSound(SoundBank soundBank)
  {
    StopSound(gameObject, soundBank);
  }

  // Stop a currently playing sound on a specific source
  public void StopSound(GameObject source, SoundBank soundBank)
  {
    AudioInstance audioInstance = GetAudioInstance(source, soundBank);
    if (audioInstance != null)
    {
      audioInstance.AudioSource.Stop();
    }
    else
    {
      Debug.LogWarning(string.Format("Couldn't find audio instance for {0}:{1}", source.name, soundBank.name));
    }
  }

  // Get a currently playing audio instance on an object
  public AudioInstance GetAudioInstance(GameObject forSource, SoundBank forSoundBank)
  {
    AudioGroup audioGroup = GetAudioGroup(forSource);
    AudioInstance audioInstance = audioGroup.GetAudioInstance(forSoundBank);
    if (audioInstance.AudioSource == null)
    {
      audioGroup.RemoveAudioInstance(audioInstance);
      if (audioGroup.InstanceCount == 0)
      {
        _audioGroups.Remove(forSource);
      }
      return null;
    }

    return audioInstance;
  }

  private IEnumerator FadeAudioRoutine(AudioInstance audioInstance, float fromVolume, float toVolume, float duration)
  {
    for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
    {
      yield return null;

      float t = time / duration;
      if (audioInstance.AudioSource != null)
      {
        audioInstance.AudioSource.volume = Mathf.Lerp(fromVolume, toVolume, t);
      }
      else
      {
        yield break;
      }
    }

    audioInstance.AudioSource.volume = toVolume;
    if (toVolume == 0)
    {
      StopSound(audioInstance.AudioSource.gameObject, audioInstance.SoundBank);
    }
  }

  private AudioGroup GetAudioGroup(GameObject forSource)
  {
    AudioGroup audioGroup = null;
    _audioGroups.TryGetValue(forSource, out audioGroup);
    return audioGroup;
  }

  private AudioGroup GetOrAddAudioGroup(GameObject forSource)
  {
    AudioGroup audioGroup = GetAudioGroup(forSource);
    if (audioGroup == null)
    {
      audioGroup = new AudioGroup();
      _audioGroups.Add(forSource, audioGroup);
    }

    return audioGroup;
  }

  // I didn't make much about the audio souces configurable, this is where you'd change the defaults
  private AudioInstance GetOrAddAudioInstance(GameObject forSource, SoundBank soundBank)
  {
    AudioGroup audioGroup = GetOrAddAudioGroup(forSource);
    AudioInstance audioInstance = audioGroup.GetAudioInstance(soundBank);
    if (audioInstance == null)
    {
      audioInstance = new AudioInstance();
      audioInstance.AudioSource = forSource.AddComponent<AudioSource>();
      audioInstance.AudioSource.playOnAwake = false;
      audioInstance.AudioSource.spatialize = soundBank.IsSpatial;
      audioInstance.AudioSource.spatialBlend = soundBank.IsSpatial ? 1.0f : 0.0f;
      audioInstance.AudioSource.volume = soundBank.VolumeScale;
      audioInstance.AudioSource.loop = soundBank.IsLooping;
      audioInstance.AudioSource.minDistance = soundBank.MinDistance;
      audioInstance.AudioSource.maxDistance = soundBank.MaxDistance;
      audioInstance.AudioSource.rolloffMode = AudioRolloffMode.Linear;
      audioInstance.AudioSource.outputAudioMixerGroup = soundBank.AudioMixerGroup;
      audioInstance.AudioSource.dopplerLevel = soundBank.DopplerLevel;
      audioInstance.SoundBank = soundBank;
      audioGroup.AddAudioInstance(audioInstance);
    }

    return audioInstance;
  }
}
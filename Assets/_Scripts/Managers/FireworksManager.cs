using System;
using System.Collections;
using UnityEngine;

public class FireworksManager : MonoBehaviour
{
    public static FireworksManager Instance { get; private set; }

    [SerializeField, Min(0)] private float fireworksLength = 3;
    [SerializeField] private ParticleSystem[] fireworks;

    private void Awake()
    {
        // Set the instance
        Instance = this;
    }

    private void Start()
    {
        WaveManager.Instance.onWaveComplete += PlayFireworksOnWaveComplete;
    }

    private void PlayFireworksOnWaveComplete()
    {
        // If the current wave is not a boss wave, return
        if (!WaveManager.Instance.CurrentWave.IsBossWave)
            return;
        
        // Play the fireworks
        PlayFireworks();
    }

    public void PlayFireworks()
    {
        // Start the fireworks coroutine
        StartCoroutine(FireworksCoroutine());
    }

    private IEnumerator FireworksCoroutine()
    {
        foreach (var firework in fireworks)
            firework.Play();

        yield return new WaitForSeconds(fireworksLength);

        foreach (var firework in fireworks)
            firework.Stop();
    }
}
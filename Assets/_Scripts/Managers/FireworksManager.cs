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
        WaveManager.Instance.onWaveComplete += PlayFireworks;
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
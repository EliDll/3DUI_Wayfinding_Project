using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using PathFinder;

public class FirefliesTrigger : MonoBehaviour
{

    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private Light light;
    [SerializeField] private float maxIntensity = 1;
    [SerializeField] private float minIntensity = 0;
    [SerializeField] private float fadeInDuration = 2;
    [SerializeField] private float fadeOutDuration = 5;

    private ICharacterSignals _characterSignals;
    private Transform _player;

    // flags to only run latest triggered coroutine
    bool stopFadeOut = false;
    bool stopFadeIn = false;

    private void Awake()
    {
        _player = PathFinderManager.Instance.player;
        _characterSignals = _player.GetComponent<ICharacterSignals>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.particleSystem.Stop();
        this.light.intensity = 0;

        _characterSignals.IsEffects.Subscribe(isEffects =>
        {
            this.light.intensity = 0;
            if (!isEffects)
            {
                this.particleSystem.Stop();
            }
        }).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        stopFadeOut = true;
        stopFadeIn = false;
        this.particleSystem.Play();
        this.StartCoroutine(fadeIn(fadeInDuration));
    }

    private void OnTriggerExit(Collider other)
    {
        stopFadeIn = true;
        stopFadeOut = false;
        this.particleSystem.Stop();
        this.StartCoroutine(fadeOut(fadeOutDuration));
    }

    IEnumerator fadeIn(float duration)
    {
        float currentIntensity = light.intensity;

        float counter = 0f;
        float from = light.intensity;
        float to = maxIntensity;

        while (counter < duration)
        {
            if (stopFadeIn) yield break;
            counter += Time.deltaTime;

            light.intensity = Mathf.Lerp(from, to, counter / duration);

            yield return null;
        }
    }

    IEnumerator fadeOut(float duration)
    {
        float currentIntensity = light.intensity;

        float counter = 0f;
        float from = light.intensity;
        float to = minIntensity;

        while (counter < duration)
        {
            if (stopFadeOut) yield break;
            counter += Time.deltaTime;

            light.intensity = Mathf.Lerp(from, to, counter / duration);

            yield return null;
        }
    }
}

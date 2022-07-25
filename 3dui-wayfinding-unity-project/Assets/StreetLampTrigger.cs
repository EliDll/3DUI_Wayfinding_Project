using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StreetLampTrigger : MonoBehaviour
{

    [SerializeField] private GameObject targetObject;
    [SerializeField] private Light light;
    [SerializeField] private float maxIntensity = 1;
    [SerializeField] private float minIntensity = 0;
    [SerializeField] private float fadeInDuration = 1;
    [SerializeField] private float fadeOutDuration = 2;

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private ICharacterSignals _characterSignals;
    private Transform _player;

    // flags to only run latest triggered coroutine
    bool stopFadeOut = false;
    bool stopFadeIn = false;
    bool _active = true;

    // Start is called before the first frame update
    void Start()
    {
        this.targetObject.active = true;
        this.light.intensity = 0;

        _characterSignals.IsEffects.Subscribe(isEffects =>
        {
            this.targetObject.active = isEffects;
            this.light.intensity = 0;
            if (!isEffects) setOffTexture();
            _active = isEffects;
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        _player = PathFinderManager.Instance.player;
        _characterSignals = _player.GetComponent<ICharacterSignals>();
    }

    private void setOnTexture()
    {
        if (!_active) return;
        Material[] mats = meshRenderer.materials;
        meshRenderer.materials = new Material[] { mats[0], onMaterial };
    }

    private void setOffTexture()
    {
        Material[] mats = meshRenderer.materials;
        meshRenderer.materials = new Material[] { mats[0], offMaterial };
    }

    private void OnTriggerEnter(Collider other)
    {
        stopFadeOut = true;
        stopFadeIn = false;
        this.StartCoroutine(fadeIn(fadeInDuration));
        setOnTexture();

    }

    private void OnTriggerExit(Collider other)
    {
        stopFadeIn = true;
        stopFadeOut = false;
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
        setOffTexture();
    }
}

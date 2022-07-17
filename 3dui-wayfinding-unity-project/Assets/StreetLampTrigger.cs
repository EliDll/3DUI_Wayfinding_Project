using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLampTrigger : MonoBehaviour
{

    [SerializeField] private GameObject targetObject;
    [SerializeField] private Light light;
    [SerializeField] private float maxIntensity = 1;
    [SerializeField] private float minIntensity = 0;

    // flags to only run latest triggered coroutine
    bool stopFadeOut = false;
    bool stopFadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        this.targetObject.active = true;
        this.light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {

    } 

    private void OnTriggerEnter(Collider other)
    {
        stopFadeOut = true;
        stopFadeIn = false;
        this.StartCoroutine(fadeIn(1));
    }

    private void OnTriggerExit(Collider other)
    {
        stopFadeIn = true;
        stopFadeOut = false;
        this.StartCoroutine(fadeOut(2));
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

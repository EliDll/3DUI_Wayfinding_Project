using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CompassBarElement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private bool useFixDirection = false;
    [SerializeField] private Vector3 fixDirection;

    private CompassBar bar;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        bar = GetComponentInParent<CompassBar>();
    }

    private void Update()
    {
        // Spitze minus Schaft
        Vector3 lookAt = useFixDirection ? fixDirection : target.position - player.position;

        var planarOrientation = new Vector2(player.forward.x, player.forward.z);
        var planarLookAt = new Vector2(lookAt.x, lookAt.z);

        // -180 to 180
        float andgleDiffDegree = Vector2.SignedAngle(planarLookAt, planarOrientation);
        // scale to -1, 1
        float mappedAngle = andgleDiffDegree / 180.0f;

        // [-1, 1] => [-compassPixelLen/2, compassPixelLen/2] => scale with 360 / Range .... everything greater than Range will not be in pixel range of compass

        float xPosition = mappedAngle * (bar.BarRectTransform.rect.width / 2.0f) * (360.0f / bar.BarRange);

        _rectTransform.anchoredPosition = new Vector2(xPosition, 0.0f);
    }
}
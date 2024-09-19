using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Camera mainCamera;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    Vector3 originalPosition;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        originalPosition = mainCamera.transform.position;
    }

    public void TriggerShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            Vector3 randomPoint = originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;

            mainCamera.transform.position = randomPoint;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalPosition;
    }
}

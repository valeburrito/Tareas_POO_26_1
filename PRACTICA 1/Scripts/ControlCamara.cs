using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                       // Se asigna con StartFollowing()
    public Vector3 offset = new Vector3(0f, 2.5f, -6f);
    public float smoothTime = 0.20f;

    [Header("Look")]
    public float lookAtHeight = 0.5f;              // mirar un poco arriba del centro de la bola

    [Header("Zoom dinámico (opcional)")]
    public bool dynamicZoom = true;
    public float minDistance = 4f;                 // distancia (positiva) mínima detrás de la bola
    public float maxDistance = 10f;                // distancia máxima detrás de la bola
    public float maxBallSpeedForZoom = 20f;        // velocidad a la que alcanzamos maxDistance

    private Vector3 velocity = Vector3.zero;
    private bool follow = false;

    void LateUpdate()
    {
        if (!follow || target == null) return;

        // Ajustar offset.z en base a velocidad
        Vector3 desiredOffset = offset;
        if (dynamicZoom)
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float speed = rb.velocity.magnitude;
                float t = Mathf.Clamp01(speed / maxBallSpeedForZoom);
                float z = Mathf.Lerp(-minDistance, -maxDistance, t); // negative z = behind target
                desiredOffset.z = z;
            }
        }

        Vector3 desiredPosition = target.position + desiredOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Rotar suavemente para mirar a la bola
        Vector3 lookAtPos = target.position + Vector3.up * lookAtHeight;
        Quaternion desiredRot = Quaternion.LookRotation(lookAtPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * 10f);
    }

    // Llamar desde el script de la bola al lanzar
    public void StartFollowing(Transform t)
    {
        target = t;
        follow = true;
    }

    public void StopFollowing()
    {
        follow = false;
        target = null;
    }
}

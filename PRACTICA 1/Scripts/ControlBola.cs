using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControlBola : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camaraPrincipal;
    public Rigidbody rb;

    [Header("Apuntado")]
    public float velocidadDeApuntado = 5f;
    public float limiteIzquierdo = -2f;
    public float limiteDerecho = 2f;

    [Header("Lanzamiento")]
    public float fuerzaDeLanzamiento = 18f;
    public ForceMode modoFuerza = ForceMode.Impulse;

    private bool haSidoLanzada = false;
    private float inputHorizontal;
    private CameraFollow camaraFollowScript;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Aseguramos que la física esté siempre activa
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (camaraPrincipal != null)
            camaraFollowScript = camaraPrincipal.GetComponent<CameraFollow>();
    }

    void Update()
    {
        if (!haSidoLanzada)
        {
            // Guardamos el input para procesarlo en FixedUpdate (mejor para física)
            inputHorizontal = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Lanzar();
            }
        }
    }

    void FixedUpdate()
    {
        if (!haSidoLanzada)
        {
            ApuntarFisicamente();
        }
    }

    void ApuntarFisicamente()
    {
        // Calculamos la posición deseada
        Vector3 nuevaPos = rb.position + Vector3.right * inputHorizontal * velocidadDeApuntado * Time.fixedDeltaTime;

        // Limitar el movimiento lateral dentro de los bordes del carril
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, limiteIzquierdo, limiteDerecho);

        // Movimiento físico
        rb.MovePosition(nuevaPos);
    }

    void Lanzar()
    {
        haSidoLanzada = true;

        // Limpiar velocidades previas
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Añadir fuerza hacia adelante (Z+)
        rb.AddForce(Vector3.forward * fuerzaDeLanzamiento, modoFuerza);

        // Activar seguimiento de cámara
        if (camaraFollowScript != null)
            camaraFollowScript.StartFollowing(transform);
        else if (camaraPrincipal != null)
        {
            CameraFollow cf = camaraPrincipal.GetComponent<CameraFollow>();
            if (cf != null) cf.StartFollowing(transform);
        }
    }

    // Reiniciar (opcional)
    public void ResetBola(Vector3 nuevaPos)
    {
        haSidoLanzada = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = nuevaPos;
    }
}
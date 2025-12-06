using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpostorSmoothTilt : MonoBehaviour
{
     public List<Transform> objectsToRotate; // Liste des objets à faire tourner
    public Transform cameraTransform;       // La caméra à pointer
    public float rotationSpeed = 5f;        // Vitesse de rotation sur l'axe Y

    // Variables pour l'oscillation
    public float tiltSpeed = 1f;            // Vitesse de tangage
    public float minTiltAngle = 5f;         // Amplitude minimum du tangage
    public float maxTiltAngle = 15f;        // Amplitude maximum du tangage
    private float randomTiltAngle;           // Amplitude de tangage aléatoire
    private float timeOffset;                // Décalage temporel pour des oscillations aléatoires

    void Start()
    {
        // Si aucune caméra n'est assignée, on utilise la caméra principale
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Générer un décalage temporel aléatoire pour varier l'effet de vent
        timeOffset = Random.Range(0f, 10f);

        // Générer une amplitude de tangage aléatoire entre minTiltAngle et maxTiltAngle
        randomTiltAngle = Random.Range(minTiltAngle, maxTiltAngle);
    }

    void Update()
    {
        foreach (Transform obj in objectsToRotate)
        {
            if (obj == null) continue; // S'assurer que l'objet est valide

            // Calcul de la direction vers la caméra
            Vector3 directionToCamera = cameraTransform.position - obj.position;
            directionToCamera.y = 0;

            // Rotation actuelle vers la caméra
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

            // Rotation lissée sur l'axe Y
            obj.rotation = Quaternion.Slerp(obj.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Calculer l'oscillation sur l'axe X et Z avec randomTiltAngle
            float tiltX = Mathf.Sin(Time.time * tiltSpeed + timeOffset) * randomTiltAngle;
            float tiltZ = Mathf.Cos(Time.time * tiltSpeed + timeOffset) * randomTiltAngle;

            // Appliquer la rotation au GameObject tout en gardant l'axe Y inchangé
            obj.localRotation = Quaternion.Euler(tiltX, obj.localEulerAngles.y, tiltZ);
        }
    }
}
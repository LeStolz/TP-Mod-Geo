using System.Collections.Generic;
using UnityEngine;

public class RotateAndLookAtCamera : MonoBehaviour
{
    public List<Transform> objectsToRotate; // Liste des objets � faire tourner
    public Transform cameraTransform;       // La cam�ra � pointer
    public float rotationSpeed = 5f;        // Vitesse de rotation sur l'axe Y

    // public bool rotateOnX = false;
    // public bool rotateOnY = true;
    // public bool rotateOnZ = false;


    void Start()
    {
        // Si aucune cam�ra n'est assign�e, on utilise la cam�ra principale
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        foreach (Transform obj in objectsToRotate)
        {
            if (obj == null) continue; // S'assurer que l'objet est valide

            // Calcul de la direction vers la cam�ra
            Vector3 directionToCamera = cameraTransform.position - obj.position;

            directionToCamera.y = 0;

            //   if (!rotateOnX) directionToCamera.x = 0; // On ignore l'axe x pour garder l'alignement horizontal
            //   if (!rotateOnY) directionToCamera.y = 0; // On ignore l'axe Y pour garder l'alignement horizontal

            // Rotation actuelle vers la cam�ra
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);

            // Rotation liss�e sur l'axe Y
            obj.rotation = Quaternion.Slerp(obj.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
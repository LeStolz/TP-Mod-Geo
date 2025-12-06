using System.Collections;
using Invector.vCharacterController;
using UnityEngine;

[RequireComponent(typeof(vThirdPersonInput))]
public class ResetTransform : MonoBehaviour
{
    private static readonly WaitForSeconds WAIT_FOR_SECONDS0_1 = new(.1f);
    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void Reset()
    {
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        GetComponent<vThirdPersonInput>().enabled = false;

        yield return WAIT_FOR_SECONDS0_1;

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        yield return WAIT_FOR_SECONDS0_1;

        GetComponent<vThirdPersonInput>().enabled = true;
    }
}

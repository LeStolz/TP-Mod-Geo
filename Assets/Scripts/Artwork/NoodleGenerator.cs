using UnityEngine;

public class NoodleGenderator : MonoBehaviour
{
    [SerializeField]
    GameObject noodlePrefab;

    void Start()
    {
        var initialTransform = noodlePrefab.transform;

        for (int i = 0; i < 360 / 6; i++)
        {
            Instantiate(
                noodlePrefab,
                initialTransform.position + 0.1f * i * Vector3.up,
                initialTransform.rotation * Quaternion.Euler(0, 0, i * 6),
                noodlePrefab.transform.parent
            );
        }
    }
}

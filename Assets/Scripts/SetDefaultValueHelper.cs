using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class SetDefaultValueHelper : MonoBehaviour
{
    [SerializeField] string playerPrefKey;

    void Start()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        int defaultValue = PlayerPrefs.GetInt(playerPrefKey, 0);
        dropdown.value = defaultValue;
    }
}

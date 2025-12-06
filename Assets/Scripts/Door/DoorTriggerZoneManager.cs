using UnityEngine;

public class DoorTriggerZoneManager : MonoBehaviour
{
    int numberOfPlayersInZone = 0;

    public bool IsPlayerInZone { get; private set; } = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            numberOfPlayersInZone++;
        }

        IsPlayerInZone = numberOfPlayersInZone > 0;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            numberOfPlayersInZone--;
        }

        IsPlayerInZone = numberOfPlayersInZone > 0;
    }
}

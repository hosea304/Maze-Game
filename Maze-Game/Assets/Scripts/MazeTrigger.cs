using UnityEngine;

public class MazeTrigger : MonoBehaviour
{
    public enum TriggerType { Entrance, Exit }
    public TriggerType triggerType;

    public MazeTimer mazeTimer;

    private bool hasTriggered = false; // Flag untuk mengecek apakah trigger sudah diaktifkan

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // Cek apakah belum pernah trigger
        {
            if (triggerType == TriggerType.Entrance)
            {
                mazeTimer.StartTimer();
                hasTriggered = true; // Set flag
            }
            else if (triggerType == TriggerType.Exit)
            {
                mazeTimer.StopTimer();
                hasTriggered = true; // Set flag
                gameObject.SetActive(false); // Nonaktifkan trigger setelah digunakan
            }
        }
    }

    // Method untuk reset trigger (bisa dipanggil saat generate maze baru)
    public void ResetTrigger()
    {
        hasTriggered = false;
        gameObject.SetActive(true);
    }
}
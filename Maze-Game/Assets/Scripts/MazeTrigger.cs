using UnityEngine;

public class MazeTrigger : MonoBehaviour
{
    public enum TriggerType { Entrance, Exit }
    public TriggerType triggerType;

    public MazeTimer mazeTimer;

    // Flag terpisah untuk timer dan pintu
    private bool hasTimerTriggered = false; // Flag untuk mengecek apakah timer sudah diaktifkan/dihentikan
    private bool hasDoorTriggered = false;  // Flag untuk mengecek apakah animasi pintu sudah diaktifkan

    Animator doorAnimator; // Referensi Animator melalui Inspector

    void Awake()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GameObject.Find("Parking Gate").GetComponent<Animator>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleTimerTrigger();
            HandleDoorTrigger(true); // Open door when player enters
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleDoorTrigger(false); // Close door when player exits
        }
    }

    // Menangani logika timer
    private void HandleTimerTrigger()
    {
        if (!hasTimerTriggered)
        {
            if (triggerType == TriggerType.Entrance)
            {
                mazeTimer.StartTimer();
                hasTimerTriggered = true; // Set flag untuk timer
            }
            else if (triggerType == TriggerType.Exit)
            {
                mazeTimer.StopTimer();
                hasTimerTriggered = true; // Set flag untuk timer
                gameObject.SetActive(false); // Nonaktifkan trigger setelah digunakan
            }
        }
    }

    // Menangani logika animasi pintu
    private void HandleDoorTrigger(bool isEntering)
    {
        if (!hasDoorTriggered)
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isTrigger", isEntering);
                hasDoorTriggered = true;
            }
            else
            {
                Debug.LogWarning("Animator tidak ditemukan pada 'Parking Gate'.");
            }
        }
        else
        {
            // Jika ingin pintu bisa dibuka/tutup setiap kali pemain masuk/keluar,
            // Anda bisa menghapus flag hasDoorTriggered atau mengaturnya kembali.
            // Berikut adalah contoh mengatur ulang flag setelah mengubah animasi:
            doorAnimator.SetBool("isTrigger", isEntering);
            // Tidak mengubah hasDoorTriggered agar animasi selalu bisa dipicu
        }
    }

    // Method untuk reset trigger (bisa dipanggil saat generate maze baru)
    public void ResetTrigger()
    {
        hasTimerTriggered = false;
        hasDoorTriggered = false;
        gameObject.SetActive(true);
        doorAnimator.SetBool("isTrigger", false); // Pastikan pintu tertutup saat reset
    }
}

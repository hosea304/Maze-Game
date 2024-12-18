using UnityEngine;

public class MazeTrigger : MonoBehaviour
{
    public enum TriggerType { Entrance, Exit }
    public TriggerType triggerType;

    // Referensi ke MazeTimer
    public MazeTimer mazeTimer;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerType == TriggerType.Entrance)
            {
                mazeTimer.StartTimer();
            }
            else if (triggerType == TriggerType.Exit)
            {
                mazeTimer.StopTimer();
            }
        }
    }
}

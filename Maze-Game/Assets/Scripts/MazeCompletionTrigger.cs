using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCompletionTrigger : MonoBehaviour
{
    public MazeWinUI mazeWinUI;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mazeWinUI.ShowWinUI();
        }
    }
}

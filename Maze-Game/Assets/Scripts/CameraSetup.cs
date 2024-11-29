using UnityEngine;
using System.Collections.Generic;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private float cameraHeight = 2f; // Tinggi kamera
    [SerializeField] private float distanceFromEntrance = 3f; // Jarak dari pintu
    [SerializeField] private float lookUpAngle = 10f; // Sudut mendongak kamera

    void Start()
    {
        PositionCamera();
    }

    public void PositionCamera()
    {
        // Dapatkan posisi entrance (pojok kiri bawah maze)
        Vector3 entrancePosition = Vector3.zero;

        // Posisikan kamera
        transform.position = entrancePosition + new Vector3(-distanceFromEntrance, cameraHeight, 0);

        // Arahkan kamera ke dalam maze
        transform.rotation = Quaternion.Euler(lookUpAngle, 90f, 0f);
    }
}
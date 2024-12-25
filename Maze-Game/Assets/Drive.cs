using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Untuk TextMeshPro

public class Drive : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;
    public TextMeshProUGUI scoreText; // Drag & drop TMP text di Inspector
    private int score = 0; // Inisialisasi skor

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        translation *= Time.fixedDeltaTime;
        rotation *= Time.fixedDeltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (translation != 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetFloat("characterSpeed", translation);
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetFloat("characterSpeed", 0);
        }
    }

    // Menangani tabrakan dengan capsule
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Capsule"))
        {
            // Tambahkan skor
            score += 1;

            // Perbarui teks skor
            scoreText.text = score.ToString();

            // Hapus capsule
            Destroy(other.gameObject);
        }
    }
}

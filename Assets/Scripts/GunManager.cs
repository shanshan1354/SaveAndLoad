using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public float speed = 1000;
    public GameObject bulletPrefab;
    public Transform bulletPosition;

    private AudioSource fireAudio;
    private float minXRotation = 0;
    private float maxXRotation = 70;
    private float minYRotation = 0;
    private float maxYRotation = 120;
    private float shootTime = 0.5f;
    private float shootTimer = 0;
    private Transform bulletHolder;

    private void Awake()
    {
        fireAudio = GetComponent<AudioSource>();
        bulletHolder = GameObject.Find("BulletHolder").GetComponent<Transform>();
    }
    private void Update()
    {
        if (!GameManager.Instance.isPause)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer > shootTime)
            {
                // 射击  实例化子弹
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject go = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
                    go.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                    go.transform.SetParent(bulletHolder);
                    gameObject.GetComponent<Animation>().Play();
                    shootTimer = 0;
                    UIManager.Instance.AddShootNum();
                    fireAudio.Play();
                }

            }
            float xPresent = Input.mousePosition.x / Screen.width;
            float yPresent = Input.mousePosition.y / Screen.height;
            float xAngle = -Mathf.Clamp(yPresent * maxXRotation, minXRotation, maxXRotation) + 20;
            float yAngle = Mathf.Clamp(xPresent * maxYRotation, minYRotation, maxYRotation) - 60;
            transform.eulerAngles = new Vector3(xAngle, yAngle, 0);
        }
    }
}

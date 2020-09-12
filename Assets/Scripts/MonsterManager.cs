using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public AnimationClip idleClip;
    public AnimationClip dieClip;
    public AudioSource kickAudio;
    public int monsterType;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        anim.clip = idleClip;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            anim.clip = dieClip;
            anim.Play();
            gameObject.GetComponent<Collider>().enabled = false;
            StartCoroutine("Deactivate");
            UIManager.Instance.AddScore();
            kickAudio.Play();
        }
    }

    private void OnDisable()
    {
        anim.clip = idleClip;
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1.25f);
        //通过父物体来更新monster
        GetComponentInParent<TargetManager>().UpdateMonsters();
    }
}

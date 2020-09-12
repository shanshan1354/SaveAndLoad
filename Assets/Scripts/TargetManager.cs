using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public GameObject[] monsterArray;
    public int targetPosition;
    public GameObject activeMonster = null;

    void Start()
    {
        foreach (GameObject monster in monsterArray)
        {
            monster.GetComponent<Collider>().enabled = false;
            monster.SetActive(false);
        }
        StartCoroutine("AliveTimer");
    }

    //随机生成怪物
    private void ActivateMonster()
    {
        
        if(activeMonster == null)
        {
            int index = Random.Range(0, monsterArray.Length);
            activeMonster = monsterArray[index];
            activeMonster.SetActive(true);
            activeMonster.GetComponent<Collider>().enabled = true;
            //调用激活死亡时间的协程
            StartCoroutine("DeathTimer");
        }
    }

    IEnumerator AliveTimer()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 5));
        ActivateMonster();
    }

    private void DeActivateMonster()
    {
        if (activeMonster != null)
        {
            activeMonster.GetComponent<Collider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
            //调用激活生成时间的协程
            StartCoroutine("AliveTimer");
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        DeActivateMonster();
    }
    //刷新monster
    public void UpdateMonsters()
    {
        StopAllCoroutines();
        if (activeMonster!=null)
        {
            activeMonster.GetComponent<Collider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        StartCoroutine("AliveTimer");
    }
    //清空monster，给activeMonster给定指定类型的monster
    public void ActivateMonsterByType(int type)
    {
        StopAllCoroutines();
        if (activeMonster != null)
        {
            activeMonster.SetActive(false);
            activeMonster.GetComponent<Collider>().enabled = false;
            activeMonster = null;
        }
        activeMonster = monsterArray[type];
        activeMonster.SetActive(true);
        activeMonster.GetComponent<Collider>().enabled = true;
        StartCoroutine("DeathTimer");
    }
}
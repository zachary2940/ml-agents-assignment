using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    //public GameObject _spawnPointContainer;


    //public delegate void OnOneTankLeft(Tank survivor);    // This will be called when only one tank left in the scene
    //public OnOneTankLeft dOnOneTankLeft = null;


    public Vector3 _center = Vector3.zero;
    public GameObject _tankPrefab;

    public float friendlyTimer = 2.5f;
    public float enemyTimer = 1.5f;
    public float _friendlyMoveSpeed=2f;
    public float _enemyMoveSpeed=2f;
    public float _reach;
    public float _spawnDistance;


    //public float _numTanks;

    protected Color[] colors =
    {
        Color.red,
        Color.blue,
    };
    protected List<GameObject> mTanks = new List<GameObject>();

    private void Awake()
    {

    }


    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public void Spawn(Vector3 center, GameObject prefab, bool enemy)
    {
        Vector3 posLocal = RandomCircle(Vector3.zero, _spawnDistance);
        Quaternion rotLocal = Quaternion.FromToRotation(Vector3.forward, Vector3.zero - posLocal);
        GameObject tankobj = Instantiate(prefab,this.transform,false);
        tankobj.transform.localPosition = posLocal;
        tankobj.transform.localRotation = rotLocal;

        mTanks.Add(tankobj);

        Tank tank = tankobj.GetComponent<Tank>();
        tank._radius = _reach;

        if (enemy)
        {
            MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = colors[0];
            tankobj.tag = "EnemyTank";
            tank._moveSpeed = _enemyMoveSpeed;


        }
        else
        {
            MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = colors[1];
            tankobj.tag = "FriendlyTank";
            tank._moveSpeed = _friendlyMoveSpeed;

        }
        tank.enemy = enemy;

    }


    public void SpawnWrtTimer()
    {
        friendlyTimer -= Time.deltaTime;
        enemyTimer -= Time.deltaTime;
        if (friendlyTimer <= 0)
        {
            Spawn(_center, _tankPrefab, false);
            friendlyTimer = 2.5f;
        }
        if (enemyTimer <= 0)
        {
            Spawn(_center, _tankPrefab, true);
            enemyTimer = 1.5f;
        }

    }

    public void Update()
    {
        SpawnWrtTimer();
    }

    public void Restart()
    {
        foreach (GameObject tank in mTanks)
        {

            if (tank != null)
            {
                tank.SetActive(false);
                Destroy(tank);
            }

        }
        friendlyTimer = 2.5f;
        enemyTimer = 1.5f;
}
}

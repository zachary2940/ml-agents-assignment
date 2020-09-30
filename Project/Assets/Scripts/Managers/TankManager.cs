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

    //public float _numTanks;

    protected Color[] colors =
    {
        Color.red,
        Color.blue,
    };

    //protected int mPlayerCount;
    protected List<GameObject> mTanks = new List<GameObject>();
    //protected List<Transform> mSpawnPoints = new List<Transform>();

    private void Awake()
    {
        // Setup the spawn points from spawn parent
        //Transform spawnTrans = _spawnPointContainer.transform;
        //for (int i = 0; i < spawnTrans.childCount; i++)
        //    mSpawnPoints.Add(spawnTrans.GetChild(i));
        //Spawn(_center,_tankPrefab,false);
        //SpawnTanks();
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
        //Vector3 pos = RandomCircle(center, 20.0f);
        // make the object face the center
        //Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
        Vector3 posLocal = RandomCircle(Vector3.zero, 20.0f);
        Quaternion rotLocal = Quaternion.FromToRotation(Vector3.forward, Vector3.zero - posLocal);
        GameObject tankobj = Instantiate(prefab,this.transform,false);
        tankobj.transform.localPosition = posLocal;
        tankobj.transform.localRotation = rotLocal;

        mTanks.Add(tankobj);

        Tank tank = tankobj.GetComponent<Tank>();

        if (enemy)
        {
            MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = colors[0];
            tankobj.tag = "EnemyTank";

        }
        else
        {
            MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = colors[1];
            tankobj.tag = "FriendlyTank";
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

    //public void OnTankDeath(Tank target)
    //{
    //    // Reduce the player count and put the dead tank to the back of the list
    //    mPlayerCount--;
    //    mTanks.Remove(target);
    //    mTanks.Add(target);

    //    // If it is the last tank standing, call delegate to announce the winner
    //    if(mPlayerCount == 1)
    //    {
    //        dOnOneTankLeft.Invoke(mTanks[0]); // First tank is always the winner
    //        mTanks[0]._inputIsEnabled = false;
    //    }
    //}

    public void Restart()
    {
        foreach (GameObject tank in mTanks)
        {
            //int num = tank._playerNum;
            //int num = 0;
            //tank.Restart(mSpawnPoints[num].position, mSpawnPoints[num].rotation);
            if (tank != null)
            {
                tank.SetActive(false);
                Destroy(tank);
            }

        }
        friendlyTimer = 2.5f;
        enemyTimer = 1.5f;
    //mPlayerCount = mTanks.Count;
}

    // Spawn and setup their color
    //public void SpawnTanks()
    //{
    //    mPlayerCount = mSpawnPoints.Count;

    //    for (int i = 0; i < mPlayerCount; i++)
    //    {
    //        // Spawn Tank and store it
    //        GameObject tank = Instantiate(_tankPrefab, mSpawnPoints[i].position, mSpawnPoints[i].rotation);
    //        mTanks.Add(tank.GetComponent<Tank>());
    //        //mTanks[i]._playerNum = i;
    //        mTanks[i].dTankDestroyed = OnTankDeath;

    //        // Color Setup
    //        MeshRenderer[] renderers = mTanks[i].GetComponentsInChildren<MeshRenderer>();
    //        foreach (MeshRenderer rend in renderers)
    //            rend.material.color = mPlayerColors[i];
    //    }
    //}

    //public Transform[] GetTanksTransform()
    //{
    //    int count = mTanks.Count;
    //    Transform[] tanksTrans = new Transform[count];
    //    for (int i = 0; i < count; i++)
    //    {
    //        tanksTrans[i] = mTanks[i].transform;
    //    }

    //    return tanksTrans;
    //}

    //public int NumberOfPlayers
    //{
    //    get { return mSpawnPoints.Count; }
    //}
}

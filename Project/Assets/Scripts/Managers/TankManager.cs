using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public GameObject _spawnPointContainer;
    public Vector3 _center;
    public GameObject _tankPrefab;

    public delegate void OnOneTankLeft(Tank survivor);    // This will be called when only one tank left in the scene
    public OnOneTankLeft dOnOneTankLeft = null;

    public float _numTanks;

    protected Color[] mPlayerColors =
    {
        Color.red,
        Color.blue,
    };

    protected int mPlayerCount;
    protected List<Tank> mTanks = new List<Tank>();
    protected List<Transform> mSpawnPoints = new List<Transform>();

    private void Awake()
    {
        // Setup the spawn points from spawn parent
        //Transform spawnTrans = _spawnPointContainer.transform;
        //for (int i = 0; i < spawnTrans.childCount; i++)
        //    mSpawnPoints.Add(spawnTrans.GetChild(i));
        Spawn(_center,_tankPrefab);
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

    public void Spawn(Vector3 center, GameObject prefab)
    {
        for (int i = 0; i < _numTanks; i++)
        {
            Vector3 pos = RandomCircle(center, 20.0f);
            // make the object face the center
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            Instantiate(prefab, pos, rot);
        }
    }
    public void OnTankDeath(Tank target)
    {
        // Reduce the player count and put the dead tank to the back of the list
        mPlayerCount--;
        mTanks.Remove(target);
        mTanks.Add(target);

        // If it is the last tank standing, call delegate to announce the winner
        if(mPlayerCount == 1)
        {
            dOnOneTankLeft.Invoke(mTanks[0]); // First tank is always the winner
            mTanks[0]._inputIsEnabled = false;
        }
    }

    public void Restart()
    {
        foreach (Tank tank in mTanks)
        {
            //int num = tank._playerNum;
            int num = 0;
            tank.Restart(mSpawnPoints[num].position, mSpawnPoints[num].rotation);
        }
        mPlayerCount = mTanks.Count;
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

    public Transform[] GetTanksTransform()
    {
        int count = mTanks.Count;
        Transform[] tanksTrans = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            tanksTrans[i] = mTanks[i].transform;
        }

        return tanksTrans;
    }

    public int NumberOfPlayers
    {
        get { return mSpawnPoints.Count; }
    }
}

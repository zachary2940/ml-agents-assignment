using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFiringSystem : MonoBehaviour
{
    public float _cooldown;
    public GameObject _shellPrefab;
    public Transform _spawnPoint;

    public enum State
    {
        ReadyToFire,
        OnCooldown
    }
    protected State mState = State.ReadyToFire;

    public float mCooldownCounter;

    private void Start()
    {
        mCooldownCounter = _cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.ReadyToFire:
                break;

            case State.OnCooldown:
                mCooldownCounter -= Time.deltaTime;
                if (mCooldownCounter <= 0)
                    state = State.ReadyToFire;
                break;

            default:
                break;
        }
    }

    public bool Fire()
    {
        if (state == State.ReadyToFire)
        {
            //change state
            state = State.OnCooldown;

            //spawn shell
            //GameObject shell = Instantiate(_shellPrefab, _spawnPoint.position, _spawnPoint.rotation);


            return true;
        }

        return false;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if(mState != value)
            {
                switch (mState)
                {
                    case State.ReadyToFire:
                        break;

                    case State.OnCooldown:
                        mCooldownCounter = _cooldown;
                        break;

                    default:
                        break;
                }

                mState = value;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float _rotationSpeed;
    //audio clip
    //public AudioSource _movementSFX;
    //public AudioSource _tankSFX;
    //public AudioClip _clipIdle;
    //public AudioClip _clipMoving;
    //public AudioClip _clipShotFired;
    //public AudioClip _clipTankExplode;
    //public float _pitchRange = 0.2f;

    public bool _inputIsEnabled = true;

    //public int _playerNum;
    public float _maxCounter = 10;
    public float tankCounter;

    public LayerMask _layerMask;

    //delegate
    //public delegate void TankDestroyed(Tank target);
    //public TankDestroyed dTankDestroyed;

    public enum State
    {
        Idle = 0,
        Moving,
        Death,
        Inactive
    };
    protected State mState;

    protected Rigidbody mRigidbody;
    protected TurretFiringSystem mTurretShot;
    //audio source
    protected float mOriginalPitch;

    protected string mVerticalAxisInputName = "Vertical1";
    protected string mHorizontalAxisInputName = "Horizontal1";
    protected string mFireInputName = "Fire1";
    protected float mVerticalInputValue = 0f;
    protected float mHorizontalInputValue = 0f;

    // Awake is called right at the beginning if the object is active
    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mTurretShot = GetComponent<TurretFiringSystem>();
        tankCounter = _maxCounter;
    }

    // Start is called before the first frame update
    void Start()
    {
        //mOriginalPitch = _movementSFX.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
            case State.Moving:
                if (_inputIsEnabled)
                {
                    MovementInput();
                    FireInput();
                }
                break;
            case State.Death:
                break;
            case State.Inactive:
                break;
            default:
                break;
        }
    }

    protected void MovementInput()
    {
        // Update input
        mVerticalInputValue = Input.GetAxis(mVerticalAxisInputName);
        mHorizontalInputValue = Input.GetAxis(mHorizontalAxisInputName);

        // Check movement and change states according to it
        if (Mathf.Abs(mHorizontalInputValue) > 0.1f)
            state = State.Moving;
        else state = State.Idle;
    }
    protected void FireInput()
    {
        //fire shots
        if (Input.GetButton(mFireInputName))
        {
            if (mTurretShot.Fire()) //returns shell
            {
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, _layerMask))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    Debug.Log("Did not Hit");
                }
            }
        }
    }

    //protected void ChangeMovementAudio(AudioClip clip)
    //{
    //    if (_movementSFX.clip != clip)
    //    {
    //        _movementSFX.clip = clip;
    //        _movementSFX.pitch = mOriginalPitch + Random.Range(-_pitchRange, _pitchRange);
    //        _movementSFX.Play();
    //    }
    //}
    //protected void PlaySFX(AudioClip clip)
    //{
    //    _tankSFX.clip = clip;
    //    _tankSFX.pitch = mOriginalPitch + Random.Range(-_pitchRange, _pitchRange);
    //    _tankSFX.Play();
    //}

    // Physic update. Update regardless of FPS
    void FixedUpdate()
    {
        //Move();
        Rotate();
    }

    // Move the tank based on speed
    //public void Move()
    //{
    //    Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime * mVerticalInputValue;
    //    mRigidbody.MovePosition(mRigidbody.position + moveVect);
    //}

    // Rotate the tank
    public void Rotate()
    {
        float rotationDegree = _rotationSpeed * Time.deltaTime * mHorizontalInputValue;
        Quaternion rotQuat = Quaternion.Euler(0f, rotationDegree, 0f);
        mRigidbody.MoveRotation(mRigidbody.rotation * rotQuat);
    }

    public void AbsorbTank()
    {
        if (mState != State.Inactive || mState != State.Death)
        {
            tankCounter -= 1;
            if (tankCounter <= 0)
                state = State.Death;
        }
    }

    protected void Death()
    {
        //PlaySFX(_clipTankExplode);
        StartCoroutine(ChangeState(State.Inactive, 1f));
    }

    public void Restart(Vector3 pos, Quaternion rot)
    {
        // Reset position, rotation and health
        transform.position = pos;
        transform.rotation = rot;
        tankCounter = _maxCounter;


        // Diable kinematic and activate the gameobject and input
        mRigidbody.isKinematic = false;
        gameObject.SetActive(true);
        _inputIsEnabled = true;

        // Change state
        state = State.Idle;
    }

    private IEnumerator ChangeState(State state, float delay)
    {
        // Delay
        yield return new WaitForSeconds(delay);

        // Change state
        this.state = state;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if (mState != value)
            {
                switch (value)
                {
                    case State.Idle:
                        //ChangeMovementAudio(_clipIdle);
                        break;

                    case State.Moving:
                        //ChangeMovementAudio(_clipMoving);
                        break;
                    case State.Death:
                        Death();
                        break;
                    case State.Inactive:
                        gameObject.SetActive(false);
                        //dTankDestroyed.Invoke(this);
                        mRigidbody.isKinematic = true;
                        _inputIsEnabled = false;
                        break;
                    default:
                        break;
                }

                mState = value;
            }
        }
    }
}

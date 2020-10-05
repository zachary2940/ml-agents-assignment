using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float _moveSpeed;
    public LayerMask _layerMask;
    public float _radius;


    //public float _rotationSpeed;
    //audio clip
    public AudioSource _movementSFX;
    public AudioSource _tankSFX;
    public AudioClip _clipIdle;
    public AudioClip _clipMoving;
    public AudioClip _clipShotFired;
    public AudioClip _clipTankExplode;
    public float _pitchRange = 0.2f;

    public bool _inputIsEnabled = true;

    //public int _playerNum;
    public float _maxHealth = 100;
    public float mHealth;

    public bool enemy;

    //delegate
    public delegate void TankDestroyed(Tank target);
    public TankDestroyed dTankDestroyed;

    public enum State
    {
        //Idle = 0,
        Moving,
        //Death,
        Inactive
    };
    protected State mState;

    protected Rigidbody mRigidbody;
    //protected TankFiringSystem mTankShot;
    //audio source
    protected float mOriginalPitch;

    protected string mVerticalAxisInputName = "Vertical";
    //protected string mHorizontalAxisInputName = "Horizontal";
    //protected string mFireInputName = "Fire";
    protected float mVerticalInputValue = 0f;
    protected float mHorizontalInputValue = 0f;

    // Awake is called right at the beginning if the object is active
    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        //mTankShot = GetComponent<TankFiringSystem>();
        mHealth = _maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        mOriginalPitch = _movementSFX.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            //case State.Idle:
            case State.Moving:
                //if (_inputIsEnabled)
                //{
                //    MovementInput();
                //    //FireInput();
                //}
                break;

            //case State.TakingDamage:
            //    break;
            //case State.Death:
            //    break;
            case State.Inactive:
                break;
            default:
                break;
        }
    }




    //protected void MovementInput()
    //{
    //    // Update input
    //    mVerticalInputValue = Input.GetAxis(mVerticalAxisInputName + (_playerNum + 1));
    //    //mHorizontalInputValue = Input.GetAxis(mHorizontalAxisInputName + (_playerNum + 1));

    //    // Check movement and change states according to it
    //    if (Mathf.Abs(mVerticalInputValue) > 0.1f || Mathf.Abs(mHorizontalInputValue) > 0.1f)
    //        state = State.Moving;
    //    else state = State.Idle;
    //}
    //protected void FireInput()
    //{
    //    //fire shots
    //    if (Input.GetButton(mFireInputName + (_playerNum + 1)))
    //    {
    //        if(mTankShot.Fire())
    //        {
    //            PlaySFX(_clipShotFired);
    //        }
    //    }
    //}

    protected void ChangeMovementAudio(AudioClip clip)
    {
        if(_movementSFX.clip != clip)
        {
            _movementSFX.clip = clip;
            _movementSFX.pitch = mOriginalPitch + UnityEngine.Random.Range(-_pitchRange, _pitchRange);
            _movementSFX.Play();
        }
    }
    protected void PlaySFX(AudioClip clip)
    {
        _tankSFX.clip = clip;
        _tankSFX.pitch = mOriginalPitch + UnityEngine.Random.Range(-_pitchRange, _pitchRange);
        _tankSFX.Play();
    }

    // Physic update. Update regardless of FPS
    void FixedUpdate()
    {
        Move();
        //Rotate();
    }

    // Move the tank based on speed
    public void Move()
    {
        Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime * 1;
        mRigidbody.MovePosition(mRigidbody.position + moveVect);
    }

    //// Rotate the tank
    //public void Rotate()
    //{
    //    float rotationDegree = _rotationSpeed * Time.deltaTime * mHorizontalInputValue;
    //    Quaternion rotQuat = Quaternion.Euler(0f, rotationDegree, 0f);
    //    mRigidbody.MoveRotation(mRigidbody.rotation * rotQuat);
    //}

    public void TakeDamage(float damage)
    {
        if (mState != State.Inactive)
        {
            mHealth -= damage;
            if (mHealth <= 0)
                state = State.Inactive;
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        Tank tank = collision.gameObject.GetComponent<Tank>(); //null
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.tag =="Player")
        {
            //state = State.Inactive;
            gameObject.SetActive(false);
        }
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        else
        {
            //Debug.Log("Do something else here");
        }
    }
    //protected void Death()
    //{
    //    PlaySFX(_clipTankExplode);
    //    StartCoroutine(ChangeState(State.Inactive, 1f));
    //}

    public void Restart(Vector3 pos, Quaternion rot)
    {
        // Reset position, rotation and health
        transform.position = pos;
        transform.rotation = rot;
        mHealth = _maxHealth;

        // Diable kinematic and activate the gameobject and input
        mRigidbody.isKinematic = false;
        gameObject.SetActive(true);
        _inputIsEnabled = true;

        // Change state
        state = State.Moving;
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
                    case State.Moving:
                        ChangeMovementAudio(_clipMoving);
                        break;
                    //case State.Death:
                    //    Death();
                    //    break;

                    case State.Inactive:
                        gameObject.SetActive(false);
                        dTankDestroyed.Invoke(this);
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

    public static implicit operator Tank(GameObject v)
    {
        throw new NotImplementedException();
    }
}

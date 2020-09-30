using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class Turret : Agent
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

    public delegate void OnTurretDestroyed(Turret turret);    // This will be called when turret is dead
    public OnTurretDestroyed dOnTurretDestroyed;
    public delegate void OnTurretWon(Turret turret);    // This will be called when turret is dead
    public OnTurretWon dOnTurretWon;



    public bool _inputIsEnabled = true;

    //public int _playerNum;
    public float _maxEnemyCounter = 10;
    public float tankEnemyCounter;
    public float _maxFriendlyCounter = 2;
    public float tankFriendlyCounter;
    public float _maxFireDistance = 2.5f;
    

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

    //protected string mVerticalAxisInputName = "Vertical1";
    protected string mHorizontalAxisInputName = "Horizontal1";
    protected string mFireInputName = "Fire1";
    protected float mVerticalInputValue = 0f;
    protected float mHorizontalInputValue = 0f;



    /// <summary>
    /// ML LOGIC
    /// </summary>
    //public Transform _target;
    //public Vector3 _startingPosition = new Vector3(0f, 0.5f, 0f);
    //public float _speed = 10f;

    private RayPerceptionSensorComponent3D rayPerception;


    public override void OnEpisodeBegin()
    {
        //if (this.transform.localPosition.y < 0)
        //{
        //    this.mRigidbody.angularVelocity = Vector3.zero;
        //    this.mRigidbody.velocity = Vector3.zero;
        //    this.transform.localPosition = _startingPosition;
        //}

        //_target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(_target.position); //3 for vector 3
        sensor.AddObservation(this.transform.position);

        float rayDistance = 20f;
        float[] rayAngles = { 30f, 60f, 90f, 120f, 150f };
        string[] detectableObjs = { "tank" };
        sensor.AddObservation(mRigidbody.angularVelocity.magnitude); //1

        //1 vector actions
        sensor.AddObservation(mRigidbody.angularVelocity.magnitude); //1

    }


    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1];
        Vector3 rotateDir = Vector3.zero;
        transform.Rotate(rotateDir, Time.deltaTime * 200f);


        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, _target.localPosition);


        //if (distanceToTarget < 1.24f)
        //{
        //    SetReward(1.0f);
        //    EndEpisode();
        //}
        //if (this.transform.localPosition.y < 0)
        //{
        //    SetReward(-1.0f);
        //    EndEpisode();
        //}

    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
    /// <summary>
    /// ML LOGIC END
    /// </summary>



    // Start is called before the first frame update
    void Start()
    {
        //mRigidbody = GetComponent<Rigidbody>();
    }




    // Awake is called right at the beginning if the object is active
    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mTurretShot = GetComponent<TurretFiringSystem>();
        rayPerception = GetComponent<RayPerceptionSensorComponent3D>();
        tankEnemyCounter = _maxEnemyCounter;
        tankFriendlyCounter = _maxFriendlyCounter;

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
        //mVerticalInputValue = Input.GetAxis(mVerticalAxisInputName);
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
                FireRay();
            }
        }
    }

    protected void FireRay()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,Mathf.Infinity, _layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            hit.transform.gameObject.SetActive(false);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
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

    public void AbsorbEnemyTank()
    {
        if (mState != State.Inactive || mState != State.Death)
        {
            tankEnemyCounter -= 1;
            if (tankEnemyCounter <= 0)
            {
                dOnTurretDestroyed.Invoke(this);
            }
        }
    }

    public void AbsorbFriendlyTank()
    {
        if (mState != State.Inactive || mState != State.Death)
        {
            tankFriendlyCounter -= 1;
            if (tankFriendlyCounter <= 0)
            {
                Debug.Log(this);
                Debug.Log(dOnTurretWon);
                dOnTurretWon.Invoke(this);
            }
        }
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnTriggerEnter(Collider collision)
    {
        Tank tank = collision.gameObject.GetComponent<Tank>(); //null
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (tank.enemy)
        {
            AbsorbEnemyTank();
            //If the GameObject's name matches the one you suggest, output this message in the console
            //Debug.Log("Do something here");
        }

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        else
        {
            AbsorbFriendlyTank();
            //If the GameObject has the same tag as specified, output this message in the console
            //Debug.Log("Do something else here");
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
        tankEnemyCounter = _maxEnemyCounter;
        tankFriendlyCounter = _maxFriendlyCounter;



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



public class RollerAgent : Agent
{



}

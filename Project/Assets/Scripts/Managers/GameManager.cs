using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public Turret _turret;
    public Vector3 _center;
    public GameObject _turretPrefab;
    //public TankManager _tankManager;


    //private int[] mWinHistory;

    public enum State
    {
        GameLoads = 0,
        GamePrep,
        GameLoop,
        GameEnds
    };
    private State mState = State.GameLoads;

    private void Start()
    {
        //mWinHistory = new int[_tankManager.NumberOfPlayers];

        // make the object face the center
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward,Vector3.forward);
        GameObject turretObj = Instantiate(_turretPrefab, _center, rot);
        Turret turret = turretObj.GetComponent<Turret>();
        turret.dOnTurretDestroyed = OnTurretDestroyedGM;
        turret.dOnTurretWon = OnTurretWonGM;
        Debug.Log("started GM");

        state = State.GamePrep;
    }

    public void OnTurretDestroyedGM(Turret turret)
    {
        if (state == State.GameLoop)
        {
            // Record wins
            //int winnerPlayerNum = winner._playerNum;
            //int winnerPlayerNum = 0;

            //mWinHistory[winnerPlayerNum]++;
            Debug.Log("Lose");

            // End the round
            state = State.GameEnds;
        }
    }

    public void OnTurretWonGM(Turret turret)
    {
        if (state == State.GameLoop)
        {
            // Record wins
            //int winnerPlayerNum = winner._playerNum;
            //int winnerPlayerNum = 0;

            //mWinHistory[winnerPlayerNum]++;

            // End the round
            Debug.Log("Win");
            state = State.GameEnds;
        }
    }

    private void InitGamePrep()
    {
        // Initialize all tanks
        //_tankManager.Restart();

        // Change state to game loop
        state = State.GameLoop;
    }

    private IEnumerator InitGameEnd()
    {
        // Delay before starting a new round
        yield return new WaitForSeconds(3f);

        // Reinitialize tanks
        state = State.GamePrep;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if(mState != value)
            {
                mState = value;

                switch (value)
                {
                    case State.GamePrep:
                        InitGamePrep();
                        break;

                    case State.GameLoop:
                        break;

                    case State.GameEnds:
                        StartCoroutine(InitGameEnd());
                        break;

                    default:
                        break;
                }
            }
        }
    }
}


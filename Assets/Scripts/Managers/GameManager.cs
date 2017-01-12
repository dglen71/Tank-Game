﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;  
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;
	public HealthPack m_HealthPack;
	public Transform[] healthSpawnLocations;
	[HideInInspector] static private int[] m_Wins; 
	//static so when new level loads it saves wins


	private static int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;   
	private HealthPack m_HealthInstance;
	private static bool started = false;


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

		if (started == false) {
			m_Wins = new int[m_Tanks.Length];
			started = true;
			m_RoundNumber -= 1;
			SceneManager.LoadScene (0);
		}
		 

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            //SceneManager.LoadScene(0);
			m_MessageText.text = "Press 'R' to Play Again";
			yield return StartCoroutine(WaitForKey (KeyCode.R));
			Reset ();
			SceneManager.LoadScene (0);
        }
        else
        {
			if (m_RoundNumber <= 2) {
				SceneManager.LoadScene (m_RoundNumber);
			} else if (m_RoundNumber > 2) {
				int choice = Random.Range (0, 3); // change if number of levels is different!!
				SceneManager.LoadScene (choice);
			}

			yield return m_EndWait;
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
		ResetAllTanks ();
		DisableTankControl ();

		//Health Pack
		m_HealthInstance = Instantiate(m_HealthPack, transform.position,transform.rotation) as HealthPack;
		m_HealthInstance.spawnLocations = healthSpawnLocations;

		m_CameraControl.SetStartPositionAndSize ();

		m_RoundNumber++;
		m_MessageText.text = "ROUND " + m_RoundNumber;

		yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
		EnableTankControl ();

		m_MessageText.text = string.Empty;

		while (!OneTankLeft ()) {

			yield return null;
		}


    }


    private IEnumerator RoundEnding()
    {
		DisableTankControl ();

		//Health Pack
		Destroy(m_HealthInstance.gameObject);

		m_RoundWinner = null;

		m_RoundWinner = GetRoundWinner ();

		if (m_RoundWinner != null) 
			m_Wins[m_RoundWinner.m_PlayerNumber - 1] += 1;

		m_GameWinner = GetGameWinner ();

		string message = EndMessage ();
		m_MessageText.text = message;

		yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
			if (m_Wins[i] == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
			message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Wins[i] + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

	private void Reset()
	{
		for (int i = 0; i < m_Tanks.Length; i++)
		{
			m_Wins[i] = 0;
		}

		started = false;
		m_RoundNumber = 0;
	}

	IEnumerator WaitForKey(KeyCode keycode)
	{
		while (Input.GetKeyDown (keycode) == false)
			yield return null;

	}
}
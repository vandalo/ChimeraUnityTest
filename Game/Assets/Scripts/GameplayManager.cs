using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [SerializeField]
    private int m_roundDuration;
    [SerializeField]
    private int m_scoreGoalIncrasePerRound;

    private float m_currentTime;
    private int m_goalScore;
    private int m_currentScore;
    private bool m_isPlaying;

    public delegate void OnGameEnd();
    public static event OnGameEnd EndGame;

    void OnEnable()
    {
        BoardManager.OnMatch += OnMatch;
        BoardManager.NewRoundStart += NewRound;
    }

    private void OnDisable()
    {
        BoardManager.OnMatch -= OnMatch;
        BoardManager.NewRoundStart -= NewRound;
    }

    void Start()
    {
        instance = GetComponent<GameplayManager>();
        NewRound();
    }

    private void InitGameplay()
    {
        m_goalScore = m_scoreGoalIncrasePerRound;
        m_currentScore = 0;
        m_currentTime = m_roundDuration;
    }

    void Update()
    {
        m_currentTime -= Time.deltaTime;
        if (m_currentTime <= 0.0f && m_isPlaying)
        {
            m_isPlaying = false;
            EndGame();
        }
    }

    private void OnMatch(int countMatch)
    {
        if(countMatch >= 5)
        {
            m_currentScore += 3;
        }
        else if(countMatch == 4)
        {
            m_currentScore += 2;
        }
        else
        {
            m_currentScore += 1;
        }
    }

    public float GetTime()
    {
        if(m_currentTime <= 0)
        {
            return 0.01f;
        }
        return m_currentTime;
    }

    public int GetRoundDurationTime()
    {
        return m_roundDuration;
    }

    public int GetScoreToIncrease()
    {
        return m_scoreGoalIncrasePerRound;
    }

    public int GetGoalScore()
    {
        return m_goalScore;
    }

    public int GetScore()
    {
        return m_currentScore;
    }

    public bool IsWin()
    {
        return m_currentScore >= m_goalScore;
    }

    private void NewRound()
    {
        m_isPlaying = true;
        if (IsWin())
        {
            m_goalScore += m_scoreGoalIncrasePerRound;
            m_currentScore = 0;
            m_currentTime = m_roundDuration;
        }
        else
        {
            InitGameplay();
        }
    }
}

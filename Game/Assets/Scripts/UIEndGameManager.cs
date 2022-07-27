using UnityEngine;
using UnityEngine.UI;

public class UIEndGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_resultGameObject;
    [SerializeField]
    private GameObject m_goalScoreGameObject;
    [SerializeField]
    private GameObject m_scoreGameObject;
    [SerializeField]
    private GameObject m_buttonGameObject;

    private Text m_resultText;
    private Text m_goalScoreText;
    private Text m_ScoreText;
    private Text m_buttonText;

    private const string SUCCESS = "GREAT!!";
    private const string LOSE = "YOU LOSE!";
    private const string NEXT = "NEXT";
    private const string EXIT = "EXIT";

    private void Awake()
    {
        m_resultText = m_resultGameObject.GetComponent<Text>();
        m_goalScoreText = m_goalScoreGameObject.GetComponent<Text>();
        m_ScoreText = m_scoreGameObject.GetComponent<Text>();
        m_buttonText = m_buttonGameObject.GetComponent<Text>();
    }

    void OnEnable()
    {
        int goalScore = GameplayManager.instance.GetGoalScore();
        int score = GameplayManager.instance.GetScore();
        if (score >= goalScore)
        {
            m_resultText.text = SUCCESS;
            m_buttonText.text = NEXT;
        }
        else
        {
            m_resultText.text = LOSE;
            m_buttonText.text = EXIT;
        }
        m_goalScoreText.text = goalScore.ToString();
        m_ScoreText.text = score.ToString();
    }
}

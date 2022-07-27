using UnityEngine;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_timeBar;
    [SerializeField]
    private GameObject m_scoreText;
    [SerializeField]
    private GameObject m_scoreGoalText;

    private Image m_timebarSprite;
    private int m_score;
    private int m_nextGoalScore;
    private bool m_updateScore;

    void OnEnable()
    {
        BoardManager.OnMatch += OnMatch; 
        BoardManager.NewRoundStart += NewRound;
    }

    private void OnDisable()
    {
        BoardManager.OnMatch -= OnMatch;
        BoardManager.NewRoundStart += NewRound;
    }

    private void Start()
    {
        m_score = 0;
        m_nextGoalScore += GameplayManager.instance.GetScoreToIncrease();
        m_timebarSprite = m_timeBar.GetComponent<Image>();
        m_scoreGoalText.GetComponent<Text>().text = "Goal: " + m_nextGoalScore.ToString();
    }
    void Update()
    {
        float fillAmount = 1 / (GameplayManager.instance.GetRoundDurationTime() / GameplayManager.instance.GetTime());
        m_timebarSprite.fillAmount = fillAmount;
        if (m_updateScore)
        {
            UpdateScore();
        }
    }

    private void OnMatch(int score)
    {
        m_updateScore = true;
    }

    private void NewRound()
    {
        m_nextGoalScore += GameplayManager.instance.GetScoreToIncrease();
        m_updateScore = true;
    }

    private void UpdateScore()
    {
        m_score = GameplayManager.instance.GetScore();
        m_scoreText.GetComponent<Text>().text = m_score.ToString();
        m_scoreGoalText.GetComponent<Text>().text = "Goal: " + GameplayManager.instance.GetGoalScore();
        m_updateScore = false;
    }
}

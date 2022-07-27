using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject m_mainMenu;

    [SerializeField]
    public GameObject m_gameplay;
    [SerializeField]
    public GameObject m_endGame;

    void OnEnable()
    {
        GameplayManager.EndGame += EndGame;
    }

    private void OnDisable()
    {
        GameplayManager.EndGame += EndGame;
    }

    private void Start()
    {
        m_gameplay.SetActive(false);
        m_mainMenu.SetActive(true);
        m_endGame.SetActive(false);
    }

    public void TransitionToGameplay()
    {
        m_gameplay.SetActive(true);
        m_mainMenu.SetActive(false);
        m_endGame.SetActive(false);
    }

    private void EndGame()
    {
        m_gameplay.SetActive(false);
        m_mainMenu.SetActive(false);
        m_endGame.SetActive(true);
    }

    public void TransitionFromEndGame()
    {
        if(GameplayManager.instance.IsWin())
        {
            m_gameplay.SetActive(true);
            m_mainMenu.SetActive(false);
        }
        else 
        {
            m_gameplay.SetActive(false);
            m_mainMenu.SetActive(true);
        }
        m_endGame.SetActive(false);
    }
}

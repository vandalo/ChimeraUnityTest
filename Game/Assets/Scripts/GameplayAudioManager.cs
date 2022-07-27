using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayAudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource m_match;
    [SerializeField]
    AudioSource m_swap;

    void OnEnable()
    {
        BoardManager.OnMatch += OnMatch;
        BoardManager.OnSwap += OnSwap;
    }

    private void OnDisable()
    {
        BoardManager.OnMatch -= OnMatch;
        BoardManager.OnSwap -= OnSwap;
    }

    private void OnMatch(int score)
    {
        m_match.Play();
    }

    private void OnSwap()
    {
        m_swap.Play();
    }
}

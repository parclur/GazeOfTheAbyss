﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{ 
    [HideInInspector] public static SceneLoader instance;

    [SerializeField] private string levelName;

    [SerializeField] private string winName;

    [SerializeField] private string loseName;

    void Awake()
    {
        instance = this;
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(loseName);
    }

    public void WinGame()
    {
        SceneManager.LoadScene(winName);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(levelName);
    }
}
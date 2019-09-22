using Fading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneHandler : MonoBehaviour
{
    [SerializeField]
    private Text _versionText;

    [SerializeField]
    private GameObject _playerOneJoined;

    [SerializeField]
    private GameObject _playerTwoJoined;

    [SerializeField]
    private GameObject _playerThreeJoined;

    [SerializeField]
    private GameObject _playerFourJoined;

    [SerializeField]
    private GameObject _menuWrapper;

    // Minimum and maximum values for the transition.
    private float _minimum = 3.0f;
    private float _maximum = 12.0f;

    // Time taken for the transition.
    private float _duration = 1.0f;

    private float _startTime;
    private bool _transitionToPlayers;
    private bool _hasTransitioned = false;

    private bool _isStarting = false;
    private IFadeService _fadeService;

    private void Start()
    {
        Game.HasControlsMenuBeenShow = false;
        
        _versionText.text = $"{Application.companyName} - {Application.productName} - version: {Application.version}";

        _fadeService = Game.Container.Resolve<IFadeService>();

        _fadeService.DoFade(FadeDirection.FromBlack, 1.5f);
    }

    private void Update()
    {
        if (_isStarting)
            return;

        if (_hasTransitioned)
        {
            if (Input.GetAxis("Player1Jump") > 0)
                _playerOneJoined?.SetActive(true);
            if (Input.GetAxis("Player2Jump") > 0)
                _playerTwoJoined?.SetActive(true);
            if (Input.GetAxis("Player3Jump") > 0)
                _playerThreeJoined?.SetActive(true);
            if (Input.GetAxis("Player4Jump") > 0)
                _playerFourJoined?.SetActive(true);

            for (int i = 0; i < 4; i++)
            {
                int playerNumber = i + 1;
                if (Input.GetAxis($"Player{playerNumber}Pause") > 0)
                {
                    StartGame();
                    break;
                }
            }
        }

        if (_transitionToPlayers)
        {
            // Calculate the fraction of the total duration that has passed.
            float t = (Time.time - _startTime) / _duration;
            _menuWrapper.transform.position = new Vector3(_menuWrapper.transform.position.x, _menuWrapper.transform.position.y + Mathf.SmoothStep(_minimum, _maximum, t), 0);

            if (_menuWrapper?.transform.position.y >= 0)
            {
                _transitionToPlayers = false;
                _hasTransitioned = true;
            }
        }

    }


    public void StartNewGame()
    {
        if (_hasTransitioned || _transitionToPlayers == true)
            return;

        _startTime = Time.time;
        _transitionToPlayers = true;

    }

    public void StartGame()
    {
        if (_isStarting == true)
            return;

        List<int> playersToStart = new List<int>();

        if (_playerOneJoined.activeSelf)
            playersToStart.Add(1);
        if (_playerTwoJoined.activeSelf)
            playersToStart.Add(2);
        if (_playerThreeJoined.activeSelf)
            playersToStart.Add(3);
        if (_playerFourJoined.activeSelf)
            playersToStart.Add(4);

        if (playersToStart.Count() == 0)
            return;

        _isStarting = true;

        _fadeService.DoFade(FadeDirection.ToBlack);

        Game.PlayersToStart = playersToStart;
        Game.PlayerColors = Game.PlayerColors.OrderBy(a => Guid.NewGuid()).ToList();
        SceneManager.LoadSceneAsync(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}


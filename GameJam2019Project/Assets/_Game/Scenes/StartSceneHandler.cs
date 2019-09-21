using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        _versionText.text = $"{Application.companyName} - {Application.productName} - version: {Application.version}";
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

        _isStarting = true;

        int numberOfPlayers = 0;
        if (_playerOneJoined.activeSelf)
            numberOfPlayers++;
        if (_playerTwoJoined.activeSelf)
            numberOfPlayers++;
        if (_playerThreeJoined.activeSelf)
            numberOfPlayers++;
        if (_playerFourJoined.activeSelf)
            numberOfPlayers++;

        if (numberOfPlayers == 0)
            return;

        SelectNumberOfPlayer(numberOfPlayers);
        SceneManager.LoadSceneAsync(1);
    }

    public void SelectNumberOfPlayer(int numberOfPlayers)
    {
        Game.NumberOfPlayers = numberOfPlayers;
    }

    public void Exit()
    {
        Application.Quit();
    }
}


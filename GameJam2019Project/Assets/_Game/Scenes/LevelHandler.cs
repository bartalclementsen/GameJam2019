using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{   
    [SerializeField]
    private Transform _playerSpawn;

    [SerializeField]
    private GameObject _playerPerfab;

    [SerializeField]
    private FollowCameraController _followCameraController;

    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private Text _countdownText;

    private EventSystem _eventSystem;

    private AudioSource _audioSource;

    private List<PlayerController> _players;

    private void Start()
    {
        _eventSystem = GameObject.FindObjectOfType<EventSystem>();
        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(nameof(InitializeGame));
    }

    private IEnumerator InitializeGame() 
    {
        //Spawn Players
        int playerCount = Game.NumberOfPlayers;
        if(playerCount == 0)
        {
            playerCount = 2;
        }

        _players = new List<PlayerController>();

        for(int i = 0; i < playerCount; i++) {
            GameObject player = Instantiate(_playerPerfab, _playerSpawn.position, Quaternion.identity, null);

            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.playerNumber = i + 1;
            playerController.canMakeActions = false;
            _players.Add(playerController);
        }

        //Countdown

        
        _audioSource.Play();
        
        yield return new WaitForSeconds(1.3f);
        _countdownText.text = "3";
        yield return new WaitForSeconds(0.5f);
        _countdownText.text = "2";
        yield return new WaitForSeconds(0.5f);
        _countdownText.text = "1";
        yield return new WaitForSeconds(0.5f);
        _countdownText.text = "GO!";

        //Start following with camera
        foreach(PlayerController pc in _players) {
            pc.canMakeActions = true;
        }
        _followCameraController.StartFollow();

        
        yield return new WaitForSeconds(1f);
        _countdownText.text = "";
    }

    
    private bool _isPausePressed = false;
    private int _menuPressedBy = -1;

    private void FixedUpdate(){
        if(_isPausePressed == false) {
            for(int i = 0; i < _players.Count; i++) {
                _isPausePressed = Input.GetAxis($"Player{(i + 1)}Pause") > 0;
                if(_isPausePressed) {
                    _menuPressedBy = i;
                    ShowMenu();
                    break;
                }           
            }   
        }
    }

    private void ShowMenu() {
        _audioSource.Pause();
        _menu.SetActive(true);
        _eventSystem.SetSelectedGameObject(_continueButton.gameObject, new BaseEventData(_eventSystem));
        SetPauseGame(true);
    }

    private void HideMenu() {
        _isPausePressed = false;
        _audioSource.UnPause();
        _menu.SetActive(false);
        SetPauseGame(false);        
    }

    private void SetPauseGame(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void MenuContinue() {
        HideMenu();
    }

    public void MenuExit() {
        SceneManager.LoadScene(0);
        SetPauseGame(false);   
    }

    public void Restart() {
        SceneManager.LoadScene(1);
        SetPauseGame(false);   
    }

    public void WinGame() 
    {
        SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 2);
    }

    public void LooseGame() 
    {
        SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1);
    }
}

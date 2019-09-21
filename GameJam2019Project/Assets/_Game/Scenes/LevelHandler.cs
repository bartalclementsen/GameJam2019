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
    private GameObject _pauseMenu;

    [SerializeField]
    private GameObject _victoryMenu;

    [SerializeField]
    private Text _victoryText;

    [SerializeField]
    private Button _victoryRestartButton;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private Text _countdownText;

    private EventSystem _eventSystem;

    private AudioSource _audioSource;

    private List<PlayerController> _players;

    private Core.Mediators.IMessenger _messenger;
    private Core.Mediators.ISubscriptionToken _victoryMessageSubscriptionToken;

    private void Start()
    {
        _eventSystem = GameObject.FindObjectOfType<EventSystem>();
        _audioSource = GetComponent<AudioSource>();
        _messenger = Game.Container.Resolve<Core.Mediators.IMessenger>();

        _victoryMessageSubscriptionToken = _messenger.Subscribe<VictoryMessage>(vm => {
            StartCoroutine(nameof(HandleVictory), vm.VictoriousPlayerController);
        });

        StartCoroutine(nameof(InitializeGame));
    }

    private IEnumerator HandleVictory(PlayerController playerController) {
        foreach(var player in _players) {

            if(player.playerNumber != playerController.playerNumber)  
            {
                player.gameObject.SetActive(false);
            }
        }

        _victoryText.text = "Player " + playerController.playerNumber + " wins";

        yield return new WaitForSeconds(1.3f);
        ShowVicotryMenu();
    }

    private void OnDestroy() {
        _victoryMessageSubscriptionToken?.Dispose();
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
                    ShowPauseMenu();
                    break;
                }           
            }   
        }
    }

    private void ShowVicotryMenu() {
        _audioSource.Pause();
        _menu.SetActive(true);
        _victoryMenu.SetActive(true);
        _eventSystem.SetSelectedGameObject(_victoryRestartButton.gameObject, new BaseEventData(_eventSystem));
        SetPauseGame(true);
    }

    private void ShowPauseMenu() {
        _audioSource.Pause();
        _menu.SetActive(true);
        _pauseMenu.SetActive(true);
        _eventSystem.SetSelectedGameObject(_continueButton.gameObject, new BaseEventData(_eventSystem));
        SetPauseGame(true);
    }

    private void HidePauseMenu() {
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
        HidePauseMenu();
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

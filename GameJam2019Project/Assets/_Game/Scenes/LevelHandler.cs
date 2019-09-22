using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fading;
using TMPro;
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
    private GameObject _failMenu;

    [SerializeField]
    private GameObject _controlsMenu;

    [SerializeField]
    private Button _failRestartButton;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private TextMeshProUGUI _countdownText;

    private EventSystem _eventSystem;

    private AudioSource _audioSource;

    private List<PlayerController> _players;


    private Core.Mediators.IMessenger _messenger;
    private Fading.IFadeService _fadeService;
    private Core.Mediators.ISubscriptionToken _victoryMessageSubscriptionToken;

    private void Start()
    {
        _hasControlsMenuBeenShow = Game.HasControlsMenuBeenShow;

        _eventSystem = GameObject.FindObjectOfType<EventSystem>();
        _audioSource = GetComponent<AudioSource>();
        _messenger = Game.Container.Resolve<Core.Mediators.IMessenger>();
        _fadeService = Game.Container.Resolve<Fading.IFadeService>();

        _victoryMessageSubscriptionToken = _messenger.Subscribe<VictoryMessage>(vm => {
            StartCoroutine(nameof(HandleVictory), vm.VictoriousPlayerController);
        });

        _fadeService.DoFade(FadeDirection.FromBlack, 1f);

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

    private bool _isControlsMenuVisible;
    private bool _hasControlsMenuBeenShow;

    private IEnumerator InitializeGame() 
    {
        if(_controlsMenu != null && _hasControlsMenuBeenShow == false) {
            _menu.SetActive(true);
            _controlsMenu.SetActive(true);
            _isControlsMenuVisible = true;
            _hasControlsMenuBeenShow = true;
        }
        else 
        {
            //Spawn Players
            List<int> playersToStart = Game.PlayersToStart;

            _players = new List<PlayerController>();

            if(playersToStart == null)
            {
                playersToStart = new List<int>() { 1, 2, 3, 4 };
            }

            for(int i = 0; i < playersToStart.Count(); i++) {
                GameObject player = Instantiate(_playerPerfab, _playerSpawn.position, Quaternion.identity, null);

                PlayerController playerController = player.GetComponent<PlayerController>();
                playerController.playerNumber = playersToStart[i];
                playerController.playerColor = Game.PlayerColors[(playerController.playerNumber - 1) % Game.PlayerColors.Count];
                playerController.canMakeActions = false;
                _players.Add(playerController);
            }

            //Countdown
            _audioSource.Play();
            
            yield return new WaitForSeconds(3.39f);
            _countdownText.text = "3";
            yield return new WaitForSeconds(0.5f);
            _countdownText.text = "2";
            yield return new WaitForSeconds(0.5f);
            _countdownText.text = "1";
            yield return new WaitForSeconds(0.48f);
            _countdownText.text = "GO!";

            //Start following with camera
            foreach(PlayerController pc in _players) {
                pc.canMakeActions = true;
            }
            _followCameraController.StartFollow();

            
            yield return new WaitForSeconds(1f);
            _countdownText.text = "";
        }
    }

    
    private bool _isPausePressed = false;
    private int _menuPressedBy = -1;

    private bool _allPlayersDead = false;

    private void FixedUpdate(){

        if(_allPlayersDead == true)
            return;

        if(_isPausePressed == false && _isControlsMenuVisible == false) {
            for(int i = 0; i < _players.Count; i++) {
                _isPausePressed = Input.GetAxis($"Player{(i + 1)}Pause") > 0;
                if(_isPausePressed) {
                    _menuPressedBy = i;
                    ShowPauseMenu();
                    break;
                }           
            }   
        }
        
        if(_isControlsMenuVisible == true) {

            bool shouldStartGame = Input.GetAxis($"Player1Jump") > 0 ||
            Input.GetAxis($"Player2Jump") > 0 ||
            Input.GetAxis($"Player3Jump") > 0 ||
            Input.GetAxis($"Player4Jump") > 0;

            if(shouldStartGame) {
                _menu.SetActive(false);
                _controlsMenu.SetActive(false);
                _isControlsMenuVisible = false;

                Game.HasControlsMenuBeenShow = true;

                StartCoroutine(nameof(InitializeGame));
            }
            return;
        }

        //Check if all players a dead
        bool isAnyPlayerAlive = _players.Any(p => p.gameObject.activeSelf);

        if(isAnyPlayerAlive == false) 
        {
            _allPlayersDead = true;
            StartCoroutine(nameof(HandleAllPlayersDead));
            Debug.Log("ALL PLAYERS ARE DEAD");
        }
    }

    private IEnumerator HandleAllPlayersDead() 
    {
        yield return new WaitForSeconds(0.5f);
        ShowFailMenu();
    }

    private void ShowVicotryMenu() {
        _audioSource.Pause();
        _menu.SetActive(true);
        _victoryMenu.SetActive(true);
        _eventSystem.SetSelectedGameObject(_victoryRestartButton.gameObject, new BaseEventData(_eventSystem));
        SetPauseGame(true);
    }

    private void ShowFailMenu() {
        _audioSource.Pause();
        _menu.SetActive(true);
        _failMenu.SetActive(true);
        _eventSystem.SetSelectedGameObject(_failRestartButton.gameObject, new BaseEventData(_eventSystem));
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

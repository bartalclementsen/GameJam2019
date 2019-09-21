using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[ExecuteInEditMode]
public class FollowCameraController : MonoBehaviour
{
    [SerializeField]
    private float _horizontalPadding = 5f;

    [SerializeField]
    private float _zoomSpeed = 0.1f;

    [SerializeField]
    private float _followSpeed = 0.1f;

    [SerializeField]
    private float _bottomPadding = 3f;

    [SerializeField]
    private float _minWidth = 8f;

    private GameObject[] _players;

    private Camera _camera;

    private bool _isFollowing = false;

    private Core.Loggers.ILogger _logger;

    // Start is called before the first frame update
    private void Start()
    {
        _logger = Game.Container.Resolve<Core.Loggers.ILoggerFactory>().Create(this);

        _camera = this.GetComponent<Camera>();
        
        if (_camera == null)
        {
            _logger.Error("No camera attached");
        }
    }

    public void StartFollow() 
    {
        _isFollowing = true;

        _players = GameObject.FindGameObjectsWithTag("Player");

        if (_players.Any() == false)
        {
            _logger.Error("No game object with tag player found in scene");
        }
    }

    private void Update()
    {
        if(_isFollowing == false)
        {
            return; //NO READY TO FOLLOW
        }
        
        Vector3 currentPosition = this.transform.position;

        GameObject[] sortedPlayersByHeight = _players
            .Where(p => p.activeSelf)
            .OrderBy(p => p.transform.position.y).ToArray();

        // Get all active players, ordered by x pos
        GameObject[] sortedPlayers = _players
            .Where(p => p.activeSelf)
            .OrderBy(p => p.transform.position.x).ToArray();

        GameObject highestPlayer = sortedPlayersByHeight[0];
        GameObject lowestPlayer = sortedPlayersByHeight[sortedPlayers.Length - 1];

        GameObject lastPlayer = sortedPlayers[0];
        GameObject firstPlayer = sortedPlayers[sortedPlayers.Length - 1];

        // Calculate the spread bethween the first and last player
        float playerSpreadX = firstPlayer.transform.position.x - lastPlayer.transform.position.x;
        float playerSpreadY = lowestPlayer.transform.position.y - highestPlayer.transform.position.y;

        // Find the midpoint bethween the first and last player
        Vector3 midpointVector = (firstPlayer.transform.position + lastPlayer.transform.position) / 2f;

        // Create the new desired position of the camera
        float cameraViewHeight = 2f * _camera.orthographicSize;
        float desiredY = highestPlayer.transform.position.y + ((cameraViewHeight - _bottomPadding) / 2f);

        Vector3 desiredNewPosition = new Vector3(midpointVector.x, desiredY, currentPosition.z);

        // Calculate the desired camera view width
        float desiredCameraViewWidth = playerSpreadX + _horizontalPadding;
        if(desiredCameraViewWidth < _minWidth) {
            desiredCameraViewWidth = _minWidth;
        }

        float desiredCameraViewHeight = playerSpreadY + _bottomPadding;

        //Debug.Log(desiredCameraViewWidth);
        //Debug.Log(desiredCameraViewHeight);
        // Calculate the desired zoom
        float desiredZoomByHeight = desiredCameraViewHeight / 2f;
        float desiredZoomByWidht = desiredCameraViewWidth / (2f * _camera.aspect);

        float desiredZoom = Mathf.Max(desiredZoomByHeight, desiredZoomByWidht);
        // Set camera Orthographic Size to the desired zoom (with a Lerp)
        _camera.orthographicSize =  Mathf.Lerp(_camera.orthographicSize, desiredZoom, _zoomSpeed);

        // Move camera a certain distance
        Vector3 newPosition = new Vector3(
            Mathf.Lerp(currentPosition.x, desiredNewPosition.x, _followSpeed) , 
            desiredNewPosition.y,
            desiredNewPosition.z);

        this.transform.position = newPosition;

        // Snap when close enough to prevent annoying lerp behavior
        if ((desiredNewPosition - currentPosition).magnitude <= 0.05f)
            this.transform.position = desiredNewPosition;
    }
}


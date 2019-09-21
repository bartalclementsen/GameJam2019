using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[ExecuteInEditMode]
public class FollowCameraController : MonoBehaviour
{
    [SerializeField]
    private float _horizontalPadding = 10f;

    [SerializeField]
    private float _zoomSpeed = 0.1f;

    [SerializeField]
    private float _followSpeed = 0.1f;

    [SerializeField]
    private float _bottomPadding = 4f;


    private GameObject[] _players;

    private Camera _camera;

    private Core.Loggers.ILogger _logger;

    // Start is called before the first frame update
    private void Start()
    {
        _logger = Game.Container.Resolve<Core.Loggers.ILoggerFactory>().Create(this);

        _camera = this.GetComponent<Camera>();
        _players = GameObject.FindGameObjectsWithTag("Player");

        if (_players.Any() == false)
        {
            _logger.Error("No game object with tag player found in scene");
        }
        if (_camera == null)
        {
            _logger.Error("No camera attached");
        }
    }

    private void Update()
    {
        Vector3 currentPosition = this.transform.position;

        // Get all active players, ordered by x pos
        GameObject[] sortedPlayers = _players
            .Where(p => p.activeSelf)
            .OrderBy(p => p.transform.position.x).ToArray();

        GameObject lastPlayer = sortedPlayers[0];
        GameObject firstPlayer = sortedPlayers[sortedPlayers.Length - 1];

        // Calculate the spread bethween the first and last player
        float playerSpread = firstPlayer.transform.position.x - lastPlayer.transform.position.x;

        // Find the midpoint bethween the first and last player
        Vector3 midpointVector = (firstPlayer.transform.position + lastPlayer.transform.position) / 2f;

        // Create the new desired position of the camera
        float cameraViewHeight = 2f * _camera.orthographicSize;
        float desiredY = midpointVector.y + (cameraViewHeight - _bottomPadding) / 2f;

        Vector3 desiredNewPosition = new Vector3(midpointVector.x, desiredY, currentPosition.z);

        // Calculate the desired camera view width
        float desiredCameraViewWidth = playerSpread + _horizontalPadding;

        // Calculate the desired zoom

        float desiredZoom = desiredCameraViewWidth / (2f * _camera.aspect);

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


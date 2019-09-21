using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        Animator a = _player.GetComponent<Animator>();

        a.SetTrigger("IsRunning");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

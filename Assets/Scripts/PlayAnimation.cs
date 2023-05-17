using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class PlayAnimation : MonoBehaviour
{
    public float gravity = -0.2f;


    [SerializeField]
    private AnimancerComponent _Animancer;

    [SerializeField]
    private ClipTransition _Walk;

    private CharacterController _CharacterController;

    

    // Start is called before the first frame update
    void Start()
    {
        _Animancer.Play(_Walk);
        _CharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _CharacterController.Move(new Vector3(0.0f, gravity, 0.0f));
    }
}

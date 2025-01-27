using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Video;


public class CineMachineSwitcher : MonoBehaviour
{
    private Animator _animator;
    
    
    
    private bool MenuCam = true;
    
   
    [SerializeField] private Animator PlayerAnimator; 
    [SerializeField] private GameObject fakePlayerObject; 
    [SerializeField] private GameObject uiObject;
    [SerializeField] private PlayableDirector cutscenePlayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    //switches between the player camera and the menu camera
    public void SwitchState()
    {
        if (MenuCam)
        {
            _animator.Play("PlayerCam");
            //turn off layer weight for idle animations
            PlayerAnimator.SetLayerWeight(2, 0);
            //play cutscene
            PlayCutscene();
            //turn on ui
           // TurnOnUi();
        }
        else
        {
            _animator.Play("MenuCam");
        }
        MenuCam = !MenuCam;
    }
    //plays the cutscene of fake player transitioning to realtime player
    public void PlayCutscene()
    {
        //play cutscene through timeline
        cutscenePlayer.Play();
    }

    public void TurnOnUi()
    {
        //set active gameobject to true
        uiObject.SetActive(true);
    }
}

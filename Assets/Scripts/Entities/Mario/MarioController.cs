using ObjectBasedStateMachine.UnLayered;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The Input Reader/Controller object for the system. Placed on the Root of Mario's entity because the game is simple enough that it shouldn't need to keep track of Input with any kind of cross-scene stuff. <br />
/// This asset contains references to all the input actions. While this is overengineered for this particular project, the "easy" "Generate C# class" option is deceptively rigid in just the wrong way to make stuff like rebinding impossible without really obtuse and unreliable fixes. <br />
/// So, I've worked to canibalize the generated C# class to make something that, while having to be manually upkept with every change to the Input Asset, at least is properly connected with it and doesn't deliberately make itself a pain in the ass. <br />
/// Future versions of this solution will likely be based on ScriptableObject Singletons.
/// </summary>
[DefaultExecutionOrder(-10)]
public class MarioController : MonoBehaviour, ISingleton<MarioController>
{
    #region Singleton

    protected static MarioController I;
    protected ISingleton<MarioController> S => this;
    public static MarioController Get() => ISingleton<MarioController>.Get(ref I);
    public static bool TryGet(out MarioController result) => ISingleton<MarioController>.TryGet(Get, out result);

    #endregion

    #region InputActions

    public InputActionAsset asset;

    public Gameplay GameplayGroup;
    [Serializable]
    public struct Gameplay
    {
        [SerializeField] InputActionReference r_XMovement;
        [SerializeField] InputActionReference r_YMovement;
        [SerializeField] InputActionReference r_Jump;

        public static float XMovement => Get().GameplayGroup.r_XMovement.action.ReadValue<float>();
        public static float YMovement => Get().GameplayGroup.r_YMovement.action.ReadValue<float>();
        public static InputAction Jump => Get().GameplayGroup.r_Jump.action;
    }

    public UI UIGroup;
    [Serializable]
    public struct UI
    {
        [SerializeField] InputActionReference r_Pause;
        public static InputAction Pause => Get().UIGroup.r_Pause.action;
    }


    public void Enable() => asset.Enable();
    public void Disable() => asset.Disable();

    #endregion

    #region References
    StateMachine marioStateMachine;
    #endregion

    public void Awake()
    {
        S.Initialize(ref I);
        asset.Enable();
        TryGetComponent(out marioStateMachine);
        Gameplay.Jump.performed += JumpAction;
    }

    private void OnDestroy()
    {
        Gameplay.Jump.performed -= JumpAction;
        S.DeInitialize(ref I);
    }

    private void JumpAction(InputAction.CallbackContext context) => marioStateMachine.SendSignal("Jump");




}

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

    public GameplayGroup Gameplay;
    [Serializable]
    public struct GameplayGroup
    {
        [SerializeField] InputActionReference r_XMovement;
        [SerializeField] InputActionReference r_YMovement;
        [SerializeField] InputActionReference r_Jump;

        public static float XMovement => Get().Gameplay.r_XMovement.action.ReadValue<float>();
        public static float YMovement => Get().Gameplay.r_YMovement.action.ReadValue<float>();
        public static InputAction Jump => Get().Gameplay.r_Jump.action;
    }

    public UIGroup UI;
    [Serializable]
    public struct UIGroup
    {
        [SerializeField] InputActionReference r_Pause;
        public static InputAction Pause => Get().UI.r_Pause.action;
    }


    public void Enable() => asset.Enable();
    public void Disable() => asset.Disable();

    #endregion


    public void Awake()
    {
        S.Initialize(ref I);
        asset.Enable();
    }
    private void OnDestroy()
    {

        S.DeInitialize(ref I);
    }






}

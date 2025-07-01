using OWML.Common;
using OWML.ModHelper;
using System.CodeDom;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GhostEverything
{
    public class GhostEverything : ModBehaviour
    {
        private int toggleMask = (1 << 0) | (1 << 9) | (1 << 15) | (1 << 28) | (1 << 2);
        private int hiddenMask = 0;
        private int visibleMask = 0;
        private bool hidden = false;
        private bool validScene = false;

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(GhostEverything)} is loaded!", MessageType.Success);

            //Only let it be used in specific scenes
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                validScene = loadScene == OWScene.SolarSystem || loadScene == OWScene.EyeOfTheUniverse;
            };

            //When the player wakes up, reset the mod
            GlobalMessenger.AddListener("WakeUp", OnceAwake);
        }

        /**
         * Wait a frame, then reset the mod state
         */
        private void OnceAwake()
        {
            ModHelper.Events.Unity.FireOnNextUpdate(() =>
            {
                visibleMask = Locator.GetPlayerCameraController()._origCullingMask;
                hiddenMask = visibleMask ^ toggleMask;
                hidden = true; 
                Locator.GetPlayerCamera().cullingMask = hiddenMask;
                Locator.GetPlayerCameraController()._origCullingMask = hiddenMask;
            });
        }

        private void Update()
        {
            if (Keyboard.current[Key.K].wasPressedThisFrame && validScene && Locator.GetPlayerCamera() != null)
            {
                if(hidden)
                {
                    hidden = false;
                    Locator.GetPlayerCamera().cullingMask = visibleMask;
                    Locator.GetPlayerCameraController()._origCullingMask = visibleMask;
                }
                else
                {
                    hidden = true;
                    Locator.GetPlayerCamera().cullingMask = hiddenMask;
                    Locator.GetPlayerCameraController()._origCullingMask = hiddenMask;
                }
            }
        }

    }
}
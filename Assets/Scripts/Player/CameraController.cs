using Fusion;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private UnityEngine.Behaviour[] localOnlyBehaviours;

    public override void Spawned()
    {
        bool isLocalPlayer = HasInputAuthority;

        if (playerCamera != null)
        {
            playerCamera.enabled = isLocalPlayer;
        }

        if (audioListener != null)
        {
            audioListener.enabled = isLocalPlayer;
        }

        if (localOnlyBehaviours != null)
        {
            for (int i = 0; i < localOnlyBehaviours.Length; i++)
            {
                if (localOnlyBehaviours[i] != null)
                {
                    localOnlyBehaviours[i].enabled = isLocalPlayer;
                }
            }
        }

        Debug.Log(
            $"PlayerViewController | Object: {name} | LocalPlayer: {Runner.LocalPlayer} | " +
            $"InputAuthority: {Object.InputAuthority} | HasInputAuthority: {HasInputAuthority}"
        );
    }
}
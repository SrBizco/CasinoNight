using Fusion;
using UnityEngine;

public struct PlayerNetworkInput : INetworkInput
{
    public Vector2 Move;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        Debug.Log(
            $"Spawned avatar | LocalPlayer: {Runner.LocalPlayer} | InputAuthority: {Object.InputAuthority} | " +
            $"HasInputAuthority: {HasInputAuthority} | HasStateAuthority: {HasStateAuthority}"
        );
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        if (GetInput(out PlayerNetworkInput inputData) == false)
        {
            return;
        }

        Vector3 moveDirection = new Vector3(inputData.Move.x, 0f, inputData.Move.y);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        _characterController.Move(moveDirection * moveSpeed * Runner.DeltaTime);
    }
}
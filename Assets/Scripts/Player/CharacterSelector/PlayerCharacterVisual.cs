using Fusion;
using UnityEngine;

public class PlayerCharacterVisual : NetworkBehaviour
{
    [Header("Visual Setup")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private GameObject[] characterPrefabs;

    [Networked, OnChangedRender(nameof(OnCharacterIndexChanged))]
    public byte CharacterIndex { get; set; }

    private GameObject _currentVisualInstance;

    public override void Spawned()
    {
        RefreshVisual();

        if (HasInputAuthority)
        {
            int selectedIndex = CharacterSelectionData.SelectedCharacterIndex;
            RPC_RequestCharacterSelection((byte)selectedIndex);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestCharacterSelection(byte requestedIndex)
    {
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("PlayerCharacterVisual has no character prefabs assigned.");
            return;
        }

        int clampedIndex = Mathf.Clamp(requestedIndex, 0, characterPrefabs.Length - 1);
        CharacterIndex = (byte)clampedIndex;

        RefreshVisual();
    }

    private void OnCharacterIndexChanged()
    {
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (visualRoot == null)
        {
            Debug.LogWarning("PlayerCharacterVisual is missing VisualRoot.");
            return;
        }

        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("PlayerCharacterVisual has no character prefabs assigned.");
            return;
        }

        int index = Mathf.Clamp(CharacterIndex, (byte)0, (byte)(characterPrefabs.Length - 1));

        if (_currentVisualInstance != null)
        {
            Destroy(_currentVisualInstance);
            _currentVisualInstance = null;
        }

        GameObject selectedPrefab = characterPrefabs[index];

        if (selectedPrefab == null)
        {
            Debug.LogWarning($"Character prefab at index {index} is null.");
            return;
        }

        _currentVisualInstance = Instantiate(selectedPrefab, visualRoot);
        _currentVisualInstance.transform.localPosition = Vector3.zero;
        _currentVisualInstance.transform.localRotation = Quaternion.identity;
        _currentVisualInstance.transform.localScale = Vector3.one;

        DisableLocalOnlyComponentsFromVisual(_currentVisualInstance);

        Debug.Log($"Applied character visual index {index} to {name}");
    }

    private void DisableLocalOnlyComponentsFromVisual(GameObject visualInstance)
    {
        Camera[] cameras = visualInstance.GetComponentsInChildren<Camera>(true);
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = false;
        }

        AudioListener[] audioListeners = visualInstance.GetComponentsInChildren<AudioListener>(true);
        for (int i = 0; i < audioListeners.Length; i++)
        {
            audioListeners[i].enabled = false;
        }
    }
}
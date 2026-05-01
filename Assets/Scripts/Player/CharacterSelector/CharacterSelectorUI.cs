using UnityEngine;

public class CharacterSelectionUI : MonoBehaviour
{
    public void SelectCharacter(int characterIndex)
    {
        CharacterSelectionData.SelectedCharacterIndex = characterIndex;
        Debug.Log($"Selected character index: {characterIndex}");
    }
}
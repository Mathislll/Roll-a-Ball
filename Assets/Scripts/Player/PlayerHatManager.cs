using UnityEngine;

public class PlayerHatManager : MonoBehaviour
{
    [Header("References")]
    public Transform hatParent;

    public void EquipHat(string hatID)
    {
        if (hatParent == null)
        {
            Debug.LogError("Le parent des chapeaux n est pas assigné");
            return;
        }

        foreach (Transform child in hatParent)
        {
            child.gameObject.SetActive(false);
        }

        Transform hatToEquip = hatParent.Find(hatID);

        if (hatToEquip != null)
        {
            hatToEquip.gameObject.SetActive(true);
        }

        // Ajout de l activation de l invincibilite
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            player.ActivateHatInvincibility();
        }
    }
}
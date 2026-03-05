using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Feedbacks;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject heartPrefab;
    public MMF_Player uiDamageFeedback;

    private PlayerController playerController;
    private List<GameObject> fullHearts = new List<GameObject>();
    private int lastKnownLives;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();

            lastKnownLives = playerController.currentLives;

            InitializeHearts();
            playerController.OnHealthChanged += UpdateHeartsUI;
        }
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.OnHealthChanged -= UpdateHeartsUI;
        }
    }

    void InitializeHearts()
    {
        if (playerController == null) return;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        fullHearts.Clear();

        for (int i = 0; i < playerController.maxLives; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);

            GameObject fullHeartImage = newHeart.transform.GetChild(0).gameObject;
            fullHearts.Add(fullHeartImage);
        }

        UpdateHeartsUI(playerController.currentLives);
    }

    void UpdateHeartsUI(int currentLives)
    {
        if (currentLives < lastKnownLives && uiDamageFeedback != null)
        {
            uiDamageFeedback.PlayFeedbacks();
        }

        for (int i = 0; i < fullHearts.Count; i++)
        {
            if (i < currentLives)
            {
                fullHearts[i].SetActive(true);
            }
            else
            {
                fullHearts[i].SetActive(false);
            }
        }

        lastKnownLives = currentLives;
    }
}
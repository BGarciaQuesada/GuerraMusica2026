using UnityEngine;
using System;

public class AttackMinigame : MonoBehaviour
{
    [Header("UI")]
    public RectTransform attackPanel;
    public RectTransform successZone;
    public RectTransform block;

    [Header("Configuración")]
    public float speed = 800f;
    public int minBlocks = 3;
    public int maxBlocks = 5;

    int remainingBlocks;
    RectTransform currentBlock;
    bool blockActivated;
    float endLimit;

    // Evento que notifica al BattleController
    public Action<bool> OnMinigameHit; // true = acierto, false = fallo
    public Action OnFinishMinigame;

    void Start()
    {
        endLimit = attackPanel.rect.width / 2;
    }

    public void StartMinigame()
    {
        gameObject.SetActive(true);
        remainingBlocks = UnityEngine.Random.Range(minBlocks, maxBlocks + 1);
        NextBlock();
    }

    void Update()
    {
        if (!blockActivated || currentBlock == null) return;

        currentBlock.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (currentBlock.anchoredPosition.x > endLimit)
        {
            // Fallo automático si se pasa
            HandleMiss();
        }
    }

    public void RecieveHit() // llamado desde BattleController / Input
    {
        if (!blockActivated) return;

        bool acierto = isInSuccessZone();
        OnMinigameHit?.Invoke(acierto);

        DestroyBlock();
        NextBlock();
    }

    void NextBlock()
    {
        if (remainingBlocks <= 0)
        {
            FinishMinigame();
            return;
        }

        currentBlock = Instantiate(block, attackPanel);
        currentBlock.gameObject.SetActive(true);

        float inicioX = -attackPanel.rect.width / 2;
        currentBlock.anchoredPosition = new Vector2(inicioX, 0);

        blockActivated = true;
        remainingBlocks--;
    }

    bool isInSuccessZone()
    {
        float bloqueX = currentBlock.anchoredPosition.x;

        float zonaMin = successZone.anchoredPosition.x - successZone.rect.width / 2;
        float zonaMax = successZone.anchoredPosition.x + successZone.rect.width / 2;

        return bloqueX >= zonaMin && bloqueX <= zonaMax;
    }

    void HandleMiss()
    {
        OnMinigameHit?.Invoke(false);
        DestroyBlock();
        NextBlock();
    }

    void DestroyBlock()
    {
        blockActivated = false;
        if (currentBlock != null)
            Destroy(currentBlock.gameObject);
    }

    void FinishMinigame()
    {
        gameObject.SetActive(false);
        OnFinishMinigame?.Invoke();
    }
}

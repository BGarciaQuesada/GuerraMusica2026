using UnityEngine;
using UnityEngine.UI;
using System;

public class AttackMinigame : MonoBehaviour
{
    [Header("UI")]
    public RectTransform backgroundBar;
    public RectTransform successZone;
    public RectTransform block;

    [Header("Configuración")]
    public float speed = 700f;
    public int minBlocks = 3;
    public int maxBlocks = 5;

    int remainingBlocks;
    RectTransform currentBlock;
    bool blockActivated;
    float limit;

    // Eventos para BattleController
    public Action<bool> OnMinigameHit; // true = acierto, false = fallo
    public Action OnMinigameFinish;

    void Start()
    {
        limit = backgroundBar.rect.width / 2;
    }

    public void MinigameStart()
    {
        gameObject.SetActive(true);
        remainingBlocks = UnityEngine.Random.Range(minBlocks, maxBlocks + 1);
        NextBlock();
    }

    void Update()
    {
        if (!blockActivated || currentBlock == null) return;

        currentBlock.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (currentBlock.anchoredPosition.x > limit)
        {
            // Fallo automático si se pasa
            HandleMiss();
        }
    }

    public void RecieveAttack() // llamado desde BattleController / Input
    {
        if (!blockActivated) return;

        OnMinigameHit?.Invoke(isInZone()); // (isInZone devuelve boolean, deja de mirarlo 30 veces)

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

        currentBlock = Instantiate(block, backgroundBar);
        currentBlock.gameObject.SetActive(true);

        float startX = -backgroundBar.rect.width / 2;
        currentBlock.anchoredPosition = new Vector2(startX, 0);

        blockActivated = true;
        remainingBlocks--;
    }

    bool isInZone()
    {
        float blockX = currentBlock.anchoredPosition.x;

        float minZone = successZone.anchoredPosition.x - successZone.rect.width / 2;
        float maxZone = successZone.anchoredPosition.x + successZone.rect.width / 2;

        return blockX >= minZone && blockX <= maxZone;
    }

    // [!] Si falla, fuera bloque!!!!!!!!!!!
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
        OnMinigameFinish?.Invoke();
    }
}

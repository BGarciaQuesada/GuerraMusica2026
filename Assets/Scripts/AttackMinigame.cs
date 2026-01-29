using UnityEngine;
using System;

// [!] Como quiero que el daño dependa de la precisión del golpe, en vez de un bool que diga acierto o fallo,
// devuelvo una enumeración con los distintos niveles de precisión.
public enum HitPrecision
{
    Perfect,
    Good,
    Miss
}

public class AttackMinigame : MonoBehaviour
{
    [Header("UI")]
    public RectTransform attackPanel;
    public RectTransform goodZone;
    public RectTransform perfectZone;
    public RectTransform block;

    [Header("Configuración")]
    public float speed = 800f;
    public int minBlocks = 3;
    public int maxBlocks = 5;

    [Header("Precisión (en píxeles desde el centro)")]
    public float perfectRange = 15f;
    public float goodRange = 40f;

    int remainingBlocks;
    RectTransform currentBlock;
    bool blockActivated;
    float endLimit;

    

    // Evento que notifica al BattleController
    public Action<HitPrecision> OnMinigameHit; // true = acierto, false = fallo
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
            RegisterHit(HitPrecision.Miss);
        }
    }

    public void RecieveHit() // llamado desde BattleController / Input
    {
        if (!blockActivated) return;

        HitPrecision precision = GetPrecisionFromZones();
        RegisterHit(precision);
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

    HitPrecision GetPrecisionFromZones()
    {
        float blockX = currentBlock.anchoredPosition.x;

        if (IsInside(blockX, perfectZone))
            return HitPrecision.Perfect;

        if (IsInside(blockX, goodZone))
            return HitPrecision.Good;

        return HitPrecision.Miss;
    }

    bool IsInside(float blockX, RectTransform zone)
    {
        float min = zone.anchoredPosition.x - zone.rect.width / 2;
        float max = zone.anchoredPosition.x + zone.rect.width / 2;
        return blockX >= min && blockX <= max;
    }

    void RegisterHit(HitPrecision precision)
    {
        OnMinigameHit?.Invoke(precision);
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

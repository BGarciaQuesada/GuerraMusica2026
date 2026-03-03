using UnityEngine;

/* Para mi yo futuro y quien sea que trabaje en esto:
 * MonoBehaviour -> Vive en la escena (va asociado a algo y se puede destruir)
 * ScriptableObject -> Vive como un ARCHIVO .ASSET (aka. PARA DATOS)
 * 
 * Resumen: es una plantilla
 */

// Esto de aquí es para que salga en el menú de Unity al hacer clic derecho y no estar media hora copiando y pegando
[CreateAssetMenu(fileName = "SOSkill", menuName = "Scriptable Objects/SOSkill")]
public class SOSkill : ScriptableObject
{
    public string skillName;

    [Header("Daño")]
    public int perfectDamage = 20;
    public int goodDamage = 10;

    [Header("Curación")]
    public bool heal;
    public int healAmount;

    [Header("Buff")]
    public bool buff;
    public int buffAmount;
    public int buffTurns;

    [Header("Debuff")]
    public bool debuff;
    public int debuffAmount;
    public int debuffTurns;

    [Header("Stun")]
    public bool stun;
    public int stunTurns;

    [Header("Otros")]
    public bool useMinigame;    // Si tiene minijuego o no
    

    // Coge el enum HitPrecision del minijuego que es publico y seg�n la situación, tal...
    public int GetDamage(HitPrecision precision)
    {
        switch (precision)
        {
            case HitPrecision.Perfect:
                Debug.Log("PERFECT");
                return perfectDamage;
            case HitPrecision.Good:
                Debug.Log("GOOD");
                return goodDamage;
            default:
                Debug.Log("MISS");
                return 0;
        }
    }

    // Skill con daño y efecto directo
    public void Apply(PlayerHealth player, EnemyHealth enemy, bool targetEnemy)
    {
        ApplyWithDamage(player, enemy, targetEnemy, perfectDamage);
    }

    // [!] Se ha movido aquí lo que había de skill y direct skill.

    // Skill CON MINIJUEGO
    public void ApplyWithDamage(
        PlayerHealth player,
        EnemyHealth enemy,
        bool targetEnemy,
        int damageAmount)
    {
        if (targetEnemy)
        {
            int finalDamage = enemy.GetModifiedDamage(damageAmount); // Considerar buff/debuff...
            enemy.TakeDamage(finalDamage);

            if (stun)
                enemy.ApplyStun(stunTurns);

            if (debuff)
                enemy.ApplyDamageModifier(-debuffAmount, debuffTurns);
        }
        else
        {
            int finalDamage = player.GetModifiedDamage(damageAmount);
            player.TakeDamage(finalDamage);

            if (stun)
                player.ApplyStun(stunTurns);

            if (buff)
                player.ApplyDamageModifier(buffAmount, buffTurns);

            if (heal)
                player.Heal(healAmount);
        }
    }

}

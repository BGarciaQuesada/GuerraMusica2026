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

    [Header("Efectos")]
    public bool useMinigame;    // Si tiene minijuego o no
    public bool stun;
    public int stunTurns;
    public bool buff;

    // Coge el enum HitPrecision del minijuego que es publico y según la situación, tal...
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

}

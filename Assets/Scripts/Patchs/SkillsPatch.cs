using UnityEngine;

public class SkillsPatch : MonoBehaviour
{
    public GameObject auxAssigns, player;

    public int dañoExtraLulú, turnosLulúBuff, dañoExtraParsifal, turnosParsifalBuff, totalBuffs, totalTurns;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
        player = auxAssigns.GetComponent<AuxiliarAssigns>().player;
    }

    // Update is called once per frame
    void Update()
    {
        if(turnosLulúBuff == 0)
        {
            totalBuffs = 0;
        }

        if(turnosParsifalBuff == 0)
        {
            totalBuffs = 0;
        }
    }

    public void ApplyHeal()
    {
        if(this.gameObject.GetComponent<PartnerAttackTextAssign>().numBotonAtaque == 0 && this.gameObject.GetComponent<PartnerAttackTextAssign>().currentPartner.name == "Parsifal")
        {
            player.GetComponent<PlayerHealth>().currentHP += player.GetComponent<BattleController>().currentPartner.gameObject.GetComponent<PartnerCombat>().skills[0].healAmount;
        }
    }

    public void ApplyBuff()
    {
        if(this.gameObject.GetComponent<PartnerAttackTextAssign>().numBotonAtaque == 0 && GameObject.FindWithTag("Partner").gameObject.name == "Lulu(Clone)")
        {
            
            turnosLulúBuff += 1;
            dañoExtraLulú += 30;

            Debug.Log($"daño y turnos{dañoExtraLulú}, {turnosLulúBuff}");

        }

        if(this.gameObject.GetComponent<PartnerAttackTextAssign>().numBotonAtaque == 0 && this.gameObject.GetComponent<PartnerAttackTextAssign>().currentPartner.name == "Parsifal")
        {
            turnosParsifalBuff += 1;
            dañoExtraParsifal += 15;
            
        }
    }

    public void ApplyExtraDamage()
    {
        totalBuffs += dañoExtraLulú + dañoExtraParsifal;
        totalTurns += turnosLulúBuff + turnosParsifalBuff;
    }

    
}

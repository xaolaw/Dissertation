using UnityEngine;
using UnityEngine.UI;

public class ToolTips
{
    //mechanics
    private string deathrattle = "Deathrattle: When this unit dies it triggers the effect";

    private string battlecry = "Battlecry: When this units is played it triggers the effect";

    private string onTurnEnd = "OnTurnEnd: When turn ends it triggers the effect";

    private string onAttack = "OnAttack: Before this unit attack it triggers the effect";

    private string onDamage = "OnDamage: After damaged this unit triggers the effect";
    //area of effect

    public string GetDeathrattle()
    {
        return deathrattle;
    }
    public string GetBattlecry()
    {
        return battlecry;
    }
    public string GetOnTurnEnd()
    {
        return onTurnEnd;
    }

    public string GetOnAttack()
    {
        return onAttack;
    }

    public string GetOnDamage()
    {
        return onDamage;
    }
}

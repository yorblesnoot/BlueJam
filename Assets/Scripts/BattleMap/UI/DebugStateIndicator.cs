using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugStateIndicator : MonoBehaviour
{
    [SerializeField] TMP_Text sign;
    Dictionary<PlayerBattleState, string> playerStates = new()
    {
        {PlayerBattleState.IDLE, "I" },
        { PlayerBattleState.TARGETING_CARD, "T"},
        { PlayerBattleState.AWAITING_TURN, "W"},
        { PlayerBattleState.PERFORMING_ACTION, "A"}
    };

    void Update()
    {
        sign.text = playerStates[PlayerUnit.playerState];
    }
}

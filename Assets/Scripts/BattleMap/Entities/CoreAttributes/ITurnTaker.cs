using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnTaker 
{
    public void ReceiveBeatsFromPlayer(int beats, PlayerUnit player);
    public void TakeTurn();
    public float BeatCount { get; }

    public void ShowBeatPreview(int beats);

    public AllegianceType Allegiance { get; }

    public GameObject gameObject { get; }

    public bool isSummoned { get; }

    public float TurnThreshold { get; }

}

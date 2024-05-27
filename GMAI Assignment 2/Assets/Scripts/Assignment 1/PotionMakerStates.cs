using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PotionMakerStates
{
    protected PotionMakerClass m_PotionMaker;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Execute();
}

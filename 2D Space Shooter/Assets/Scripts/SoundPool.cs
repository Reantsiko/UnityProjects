using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPool : Pool
{
    public static SoundPool instance;

    private void Awake() => instance = this;
}

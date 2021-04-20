using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class VFXPool : Pool
{
    public static VFXPool instance;
    private void Awake() => instance = this;
}

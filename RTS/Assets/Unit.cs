using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] public int hitPoints;
    [SerializeField] private int attackDamage;
    [SerializeField] private AttackType attack;
    [SerializeField] private CharacterArmor armor;
}

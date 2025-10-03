using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemType
{
    Potion,
    Weapon
}

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType; //  New: Define if it's a Potion or Weapon
    public Sprite itemIcon;
    public string itemDescription;
    public int itemCost;

    //  New: Stats for Weapons
    public int attackBonus;
    public int defenseBonus;

    //  New: Stats for Potions
    public int healAmount;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Potion", menuName = "Shop/Potion Data")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public Sprite potionIcon;
    public string potionDescription;
    public int potionCost;
}
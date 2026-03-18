using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_ItemList : ScriptableObject
{
    public List<ItemDetails> itemDetails = new List<ItemDetails>();
}

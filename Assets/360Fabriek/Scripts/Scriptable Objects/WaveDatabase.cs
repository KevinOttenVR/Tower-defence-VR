using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDatabase", menuName = "GameData/WaveDatabase")]
public class WaveDatabase : ScriptableObject
{
    public List<WaveData> waves;
}
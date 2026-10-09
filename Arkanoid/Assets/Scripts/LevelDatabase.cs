using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Level Database")]
public class LevelDatabase : ScriptableObject
{
    public List<BlockPrefabEntry> blockPrefabs;
    public GameObject lifeIcon;
    public GameObject paddle;
    public GameObject ballPrefab;

    public AudioClip getReadySound;

    public AudioClip destroyPaddleSound;
}

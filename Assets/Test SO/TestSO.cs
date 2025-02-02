using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Game/Player Stats")]
public class TestSO : ScriptableObject
{
    public string playerName;
    public int health = 100;
    public float speed = 5f;
    public int damage = 10;
}


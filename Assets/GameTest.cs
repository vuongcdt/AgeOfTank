using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameTest : MonoBehaviour
{
    [SerializeField] private Actor objPrefab;

    private Actor _player, _enemy;
    private Vector3 _start, _end;
    private float _time = 20;
    private int _count;

    void Start()
    {
        SpawnPrefab();
    }
    
    private void SpawnPrefab()
    {
        var random = Random.value;
        _start = new Vector3(-2, 0);
        _end = new Vector3(2, 0);

        objPrefab.tag = CONSTANTS.Tag.Player;
        objPrefab.GetComponentInChildren<SameTypeCollider>().tag = CONSTANTS.Tag.SameTypeCollider;
        
        _player = Instantiate(objPrefab, _start, Quaternion.identity, transform);
        _player.type = CONSTANTS.Tag.Player; 
        _player.GetComponentInChildren<HunterCollider2>().tag = CONSTANTS.Tag.HunterColliderPlayer;
        _player.GetComponentInChildren<WarriorCollider>().tag = CONSTANTS.Tag.WarriorColliderPlayer;
        
        // var player2 = Instantiate(objPrefab, _start, Quaternion.identity, transform);
        // player2.GetComponentInChildren<HunterCollider2>().gameObject.SetActive(false);
        // player2.GetComponentInChildren<WarriorCollider>().tag = CONSTANTS.Tag.WarriorColliderPlayer;
        // player2.type = CONSTANTS.Tag.Player; 
        
        objPrefab.tag = CONSTANTS.Tag.Enemy;
        _enemy = Instantiate(objPrefab, _end, Quaternion.identity, transform);
        _enemy.GetComponentInChildren<HunterCollider2>().tag = CONSTANTS.Tag.HunterColliderEnemy;
        _enemy.GetComponentInChildren<WarriorCollider>().tag = CONSTANTS.Tag.WarriorColliderEnemy;
        _enemy.type = CONSTANTS.Tag.Enemy; 
        
        var enemy2 = Instantiate(objPrefab, _end, Quaternion.identity, transform);
        enemy2.GetComponentInChildren<HunterCollider2>().gameObject.SetActive(false);
        enemy2.GetComponentInChildren<WarriorCollider>().tag = CONSTANTS.Tag.WarriorColliderEnemy;
        enemy2.type = CONSTANTS.Tag.Enemy; 
        
        _player.transform.DOMove(_end, _time);
        _enemy.transform.DOMove(_start, _time);
        // player2.transform.DOMove(_end, _time);
        enemy2.transform.DOMove(_start, _time);
    }
}
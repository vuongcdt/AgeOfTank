using DG.Tweening;
using UnityEngine;

public class GameTest : MonoBehaviour
{
    [SerializeField] private Actor actor;

    private Vector3 _start, _end;
    private float _time = 20;
    private int _count;

    void Start()
    {
        SpawnPrefab();
    }

    private void SpawnPrefab()
    {
        _start = new Vector3(-2.5f, 0);
        _end = new Vector3(2.5f, 0);

        SpawnPlayer();
        SpawnEnemy();
    }

    private void SpawnPlayer()
    {
        actor.id = 1;
        actor.type = ENUMS.CharacterType.Player;
        actor.typeClass = ENUMS.CharacterTypeClass.Warrior;
        var player = Instantiate(actor, _start, Quaternion.identity, transform);
        
        actor.id = 2;
        actor.typeClass = ENUMS.CharacterTypeClass.Hunter;
        var player2 = Instantiate(actor, _start, Quaternion.identity, transform);
        
        player.MoveToPoint(_end, _time);
        player2.MoveToPoint(_end, _time);
    }

    private void SpawnEnemy()
    {
        actor.type = ENUMS.CharacterType.Enemy;
        actor.typeClass = ENUMS.CharacterTypeClass.WarriorEnemy;
        actor.id = 1;
        var enemy = Instantiate(actor, _end, Quaternion.identity, transform);

        actor.id = 2;
        actor.typeClass = ENUMS.CharacterTypeClass.HunterEnemy;
        var enemy2 = Instantiate(actor, _end, Quaternion.identity, transform);

        enemy.MoveToPoint(_start, _time);
        enemy2.MoveToPoint(_start, _time);
    }
}
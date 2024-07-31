using System.Collections;
using System.Collections.Generic;
using Controllers.Game;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;

namespace Controllers.NewGame
{
    public class GameManager : BaseGameController
    {
        [SerializeField] private Actor actor;
        [SerializeField] private Vector3 start, end;
        [SerializeField] private int playerCount, enemyCount;
        [SerializeField] private int playerHunterCount;
        [SerializeField] private int enemyHunterCount;
        [SerializeField] private Button resetBtn;
        [SerializeField] private int spawnMore;

        private int _count;
        private int _idPlayer, _idEnemy;
        private List<Actor> _pool = new();

        void Start()
        {
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;

            this.RegisterEvent<Events.InitCharacter>(e => { SpawnPlayer(e.TypeClass); });

            resetBtn.onClick.RemoveAllListeners();
            resetBtn.onClick.AddListener(Reset);
            StartCoroutine(SpawnPrefab());
        }

        private void Reset()
        {
            _idPlayer = 0;
            _idEnemy = 0;
            foreach (var item in _pool)
            {
                item.name += "DELETE";
                item.gameObject.SetActive(false);
            }

            StartCoroutine(SpawnPrefab());
        }

        private IEnumerator SpawnPrefab()
        {
            start = new(-2.5f, 0);
            end = new(2.5f, 0);

            yield return new WaitForSeconds(1f);

            foreach (var i in new int[playerHunterCount])
            {
                SpawnPlayer(ENUMS.CharacterTypeClass.Hunter);
            }

            foreach (var i in new int[playerCount])
            {
                SpawnPlayer();
            }

            foreach (var i in new int[enemyHunterCount])
            {
                SpawnEnemy(ENUMS.CharacterTypeClass.Hunter);
            }

            foreach (var i in new int[enemyCount])
            {
                SpawnEnemy();
            }

            for (var index = 0; index < new int[spawnMore].Length; index++)
            {
                StartCoroutine(Spawn5Player((index + 1) * 3));
            }
        }

        private IEnumerator Spawn5Player(float time)
        {
            yield return new WaitForSeconds(time);
            foreach (var i in new int[5])
            {
                SpawnPlayer();
            }
        }

        private void SpawnPlayer(ENUMS.CharacterTypeClass characterTypeClass = ENUMS.CharacterTypeClass.Fighter)
        {
            _idPlayer++;
            actor.id = _idPlayer;
            actor.type = ENUMS.CharacterType.Player;
            actor.typeClass = characterTypeClass;
            actor.start = start;
            actor.end = end;

            var sameTypeCollider = actor.GetComponentInChildren<SameTypeCollider>();

            sameTypeCollider.tag = (int)characterTypeClass % 3 == 1
                ? CONSTANS.Tag.SameTypeColliderHunter
                : CONSTANS.Tag.SameTypeCollider;

            var random = (1 - Random.value) * 0.2f;

            var player = Instantiate(actor, new Vector3(start.x + random * 0.5f, start.y + random), Quaternion.identity,
                transform);
            _pool.Add(player);

            player.MoveToPoint(start.x, end.x);
        }

        private void SpawnEnemy(ENUMS.CharacterTypeClass characterTypeClass = ENUMS.CharacterTypeClass.FighterEnemy)
        {
            _idEnemy++;
            actor.id = _idEnemy;
            actor.type = ENUMS.CharacterType.Enemy;
            actor.typeClass = characterTypeClass;
            actor.start = end;
            actor.end = start;

            var sameTypeCollider = actor.GetComponentInChildren<SameTypeCollider>();

            sameTypeCollider.tag = (int)characterTypeClass % 3 == 1
                ? CONSTANS.Tag.SameTypeColliderHunter
                : CONSTANS.Tag.SameTypeCollider;

            var random = (1 - Random.value) * 0.2f;

            var enemy = Instantiate(actor, new Vector3(end.x + random * 0.5f, end.y + random), Quaternion.identity,
                transform);
            _pool.Add(enemy);

            enemy.MoveToPoint(end.x, start.x);
        }
    }
}
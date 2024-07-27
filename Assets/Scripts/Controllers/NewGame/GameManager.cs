using System.Collections;
using Commands.Game;
using Controllers.Game;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;

namespace Controllers.NewGame
{
    public class GameManager : BaseGameController
    {
        [SerializeField] private Actor actor;
        [SerializeField] private Vector3 start, end;
        [SerializeField] private int playerCount, enemyCount, playerHuterCount, enemyHuterCount;
        private float _time = 40;
        private int _count;
        private int _idPlayer, _idEnemy;

        void Start()
        {
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = Vector3.up;

            this.RegisterEvent<Events.Events.InitCharacter>(e => { SpawnPlayer(e.TypeClass); });

            StartCoroutine(SpawnPrefab());
        }

        private IEnumerator SpawnPrefab()
        {
            start = new(-2.5f, 0);
            end = new(2.5f, 0);

            yield return new WaitForSeconds(1.5f);

            foreach (var i in new int[playerCount])
            {
                SpawnPlayer();
            }

            foreach (var i in new int[playerHuterCount])
            {
                SpawnPlayer(ENUMS.CharacterTypeClass.Hunter);
            }


            foreach (var i in new int[enemyCount])
            {
                SpawnEnemy();
            }

            foreach (var i in new int[enemyHuterCount])
            {
                SpawnEnemy(ENUMS.CharacterTypeClass.Hunter);
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

            player.MoveToPoint(end, _time);
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

            enemy.MoveToPoint(start, _time);
        }
    }
}
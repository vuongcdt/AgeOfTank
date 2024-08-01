using System.Collections;
using System.Collections.Generic;
using Controllers.Game;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

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
        [SerializeField] private int[] spawnMore;
        [SerializeField] private int totalActorInRow = 10;
        [SerializeField] private float timeClearCount = 0.07f;

        private int _countActor;
        private IEnumerator _stopCountPlayerIE;
        private int _count;
        private int _idPlayer, _idEnemy;
        private List<Actor> _pool = new();
        private float _timeCountdown;

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
            _countActor = 0;
            foreach (var item in _pool)
            {
                item.name += "DELETE";
                item.gameObject.SetActive(false);
            }

            StartCoroutine(SpawnPrefab());
        }

        private IEnumerator SpawnPrefab()
        {
            start = new(-3f, 0);
            end = new(3f, 0);

            yield return new WaitForSeconds(1f);

            // foreach (var i in new int[playerHunterCount])
            // {
            //     SpawnPlayer(ENUMS.CharacterTypeClass.Hunter);
            // }
            //
            // foreach (var i in new int[playerCount])
            // {
            //     SpawnPlayer();
            // }

            foreach (var i in new int[enemyHunterCount])
            {
                SpawnEnemy(ENUMS.CharacterTypeClass.Hunter);
            }

            foreach (var i in new int[enemyCount])
            {
                SpawnEnemy();
            }

            for (var i = 0; i < spawnMore.Length; i++)
            {
                var value = spawnMore[i];

                foreach (var j in new int[value])
                {
                    SpawnPlayer();
                }

                yield return new WaitForSeconds((i + 1) * 3);
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
            var boxCollider2D = sameTypeCollider.GetComponent<BoxCollider2D>();
            sameTypeCollider.tag = (int)characterTypeClass % 3 == 1
                ? CONSTANS.Tag.SameTypeColliderHunter
                : CONSTANS.Tag.SameTypeCollider;

            // var random = (1 - Random.value) * 0.1f;
            //
            // var player = Instantiate(actor, new Vector3(start.x + random, start.y), Quaternion.identity,
            //     transform);

            _countActor++;
            
            var random = (1 - Random.value) * 0.1f;
            var row = _countActor / totalActorInRow;
            var posX = random - 0.3f * row ;
            
            // var posX = random - boxCollider2D.size.x - 0.5f * (row - _timeCountdown / timeClearCount);
            
            if (_stopCountPlayerIE != null)
            {
                StopCoroutine(_stopCountPlayerIE);
            }
            
            _stopCountPlayerIE = StopCountPlayer();
            StartCoroutine(_stopCountPlayerIE);
            
            var player = Instantiate(actor,
                new Vector3(start.x + posX, start.y + random),
                Quaternion.identity,
                transform);

            _pool.Add(player);

            player.MoveToTarget();
        }

        private void FixedUpdate()
        {
            if (_timeCountdown > timeClearCount)
            {
                _timeCountdown = 0;
            }

            _timeCountdown += Time.deltaTime;
        }

        private IEnumerator StopCountPlayer()
        {
            yield return new WaitForSeconds(timeClearCount);
            _timeCountdown = 0;
            _countActor = 0;
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

            var random = (1 - Random.value) * 0.1f;

            var enemy = Instantiate(actor, new Vector3(end.x + random, end.y), Quaternion.identity,
                transform);
            _pool.Add(enemy);

            enemy.MoveToTarget();
        }
    }
}
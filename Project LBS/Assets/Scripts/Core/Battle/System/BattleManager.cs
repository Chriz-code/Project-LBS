using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Refrences")]
        public static BattleManager inst = null;
        [SerializeField] TurnSystem turnSystem = null;
        public TurnSystem GetTurnSystem { get => turnSystem; }
        [SerializeField] AI_Computer ai = null;
        public AI_Computer GetAI { get => ai; }

        public Ability_Library ability_Library = null;
        [SerializeField] GameValues gameValues = null;
        [SerializeField] BattleValues battleValues = null;
        [SerializeField] GameObject enemyPrefab = null;
        [SerializeField] GameObject playerPrefab = null;

        [SerializeField] int playerIndex = 0;
        public int PlayerIndex
        {
            get => playerIndex;
            set
            {
                playerIndex = value % players.Count;
            }
        }
        public List<Entity_Player> players = new List<Entity_Player>();

        public List<Entity_Enemy> enemies = new List<Entity_Enemy>();

        [Header("Values")]
        public bool autoPlay = false;

        #region Access Methods
        public List<Entity_Character> GetAllCharacters
        {
            get
            {
                List<Entity_Character> chars = new List<Entity_Character>(players);
                chars.AddRange(enemies);
                return chars;
            }
        }
        public Entity_Character[] GetFaction(Entity_Preset.EntityFaction faction)
        {
            switch (faction)
            {
                case Entity_Preset.EntityFaction.Player:
                    return players.ToArray();
                case Entity_Preset.EntityFaction.Enemy:
                    return enemies.ToArray();
                default:
                    return null;
            }
        }
        /// <summary>
        /// Returns Random of Faction
        /// </summary>
        /// <param name="faction">Faction</param>
        /// <param name="entity">Target</param>
        /// <returns></returns>
        public bool TryGetRandom(Entity_Preset.EntityFaction faction, out Entity_Character entity)
        {
            Entity_Character[] array = GetFaction(faction);
            entity = array[Random.Range(0, array.Length)];
            return entity != null;
        }
        /// <summary>
        /// Returns Random from Faction excluding entities
        /// </summary>
        /// <param name="faction">Faction</param>
        /// <param name="entity">Target</param>
        /// <param name="entities">Exclusion</param>
        /// <returns></returns>
        public bool TryGetRandomExcluding(Entity_Character[] entities, Entity_Preset.EntityFaction faction, out Entity_Character entity)
        {
            List<Entity_Character> list = new List<Entity_Character>(GetFaction(faction));
            foreach (var item in entities) list.Remove(item);
            entity = list[Random.Range(0, list.Count)];
            return entity != null;
        }
        /// <summary>
        /// Returns Randoms of Faction
        /// </summary>
        /// <param name="faction">Faction</param>
        /// <param name="entity">Targets</param>
        /// <returns></returns>
        public bool TryGetRandoms(int amount, Entity_Preset.EntityFaction faction, out Entity_Character[] entities)
        {
            List<Entity_Character> e = new List<Entity_Character>();
            for (int i = 0; i < amount; i++)
            {
                if (TryGetRandom(faction, out Entity_Character entity))
                {
                    e.Add(entity);
                }
                else
                {
                    amount++;
                }
            }
            entities = e.ToArray();
            return entities.Length > 0;
        }
        /// <summary>
        /// Returns Randoms of Faction but not the same twice
        /// </summary>
        /// <param name="faction">Faction</param>
        /// <param name="entity">Targets</param>
        /// <returns></returns>
        public bool TryGetMultiRandoms(int amount, Entity_Preset.EntityFaction faction, out Entity_Character[] entities)
        {
            List<Entity_Character> e = new List<Entity_Character>();
            for (int i = 0; i < Mathf.Min(amount, GetFaction(faction).Length); i++)
            {
                if (TryGetRandomExcluding(e.ToArray(), faction, out Entity_Character entity))
                {
                    e.Add(entity);
                }
                else
                {
                    amount++;
                }
            }
            entities = e.ToArray();
            return entities.Length > 0;
        }
        public int TotalHealth(Entity_Preset.EntityFaction faction)
        {
            int h = 0;
            switch (faction)
            {
                case Entity_Preset.EntityFaction.Player:
                    foreach (var item in players)
                    {
                        h += item.stats.HP;
                    }
                    break;
                case Entity_Preset.EntityFaction.Enemy:
                    foreach (var item in enemies)
                    {
                        h += item.stats.HP;
                    }
                    break;
            }
            return h;
        }
        #endregion

        private void Awake()
        {
            if (inst)
            {
                Destroy(this);
            }
            inst = this;
            ability_Library.Initialize();
            CreateEnemies();
            CreatePlayers();
            GetTurnSystem.onPlayerEnter.AddListener(AutoPlay);
            GetTurnSystem.onPassiveAfter.Add(CheckEndBattle);
        }
        private void Start()
        {
            GetTurnSystem.Initiate();
        }
        void CreateEnemies()
        {
            if (battleValues.template != BattleTemplate.None)
            {
                EnemySpawnTemplate(battleValues.template, out List<Entity_Enemy> enemies);
                this.enemies.AddRange(enemies);
            }
            foreach (var item in battleValues.additionalEnemies)
            {
                Vector2 pos = Vector2.right * (5 + enemies.Count*2) + ((enemies.Count % 2 == 0) ? Vector2.up : Vector2.down);
                if (Instantiate(enemyPrefab, pos, Quaternion.identity, null).TryGetComponent(out Entity_Enemy enemy))
                {
                    enemy.Initiate(item, $"{item.name}:{enemies.Count}");
                    this.enemies.Add(enemy);
                }
            }
        }
        void EnemySpawnTemplate(BattleTemplate template, out List<Entity_Enemy> enemies)
        {
            enemies = new List<Entity_Enemy>();
            switch (template)
            {
                case BattleTemplate.Test:
                    Vector2 pos = Vector2.right * (5 + enemies.Count*2) + ((enemies.Count % 2 == 0) ? Vector2.up : Vector2.down);
                    if (Instantiate(enemyPrefab, pos, Quaternion.identity, null).TryGetComponent(out Entity_Enemy enemy))
                    {
                        enemy.Initiate(battleValues.additionalEnemies[0], $"{battleValues.additionalEnemies[0].name}:{enemies.Count}");
                        enemies.Add(enemy);
                    }
                    break;
                case BattleTemplate.None:
                default:
                    break;
            }
        }
        void CreatePlayers()
        {
            foreach (var item in gameValues.playerCharacters)
            {
                Vector2 pos = Vector2.left * (5 + players.Count*2) + ((players.Count % 2 == 0) ? Vector2.up : Vector2.down);
                if (Instantiate(playerPrefab, pos, Quaternion.identity, null).TryGetComponent(out Entity_Player player))
                {
                    player.Initiate(item, $"{item.name}:{players.Count}");
                    players.Add(player);
                }
            }
        }
        public void RemoveEntity(Entity entity)
        {
            if (entity.Faction == Entity_Preset.EntityFaction.Enemy) enemies.Remove(entity as Entity_Enemy);
            if (entity.Faction == Entity_Preset.EntityFaction.Player) players.Remove(entity as Entity_Player);
            CheckEndBattle();
        }

        /// <summary>
        /// Done with Player Turn
        /// </summary>
        public void EndTurn()
        {
            turnSystem.StartTurn(TurnSystem.Turns.Player);
        }
        public void SkipTurn()
        {
            players[PlayerIndex].SetAbility(string.Empty);
            if (PlayerIndex >= players.Count - 1)
                EndTurn();
            PlayerIndex++;
        }
        public void EndBattle()
        {
            turnSystem.ForceTurn(TurnSystem.Turns.PassiveEnd);
        }
        public void CheckEndBattle(out float duration)
        {
            duration = 0;
            CheckEndBattle();
        }
        public void CheckEndBattle()
        {
            if (TotalHealth(Entity_Preset.EntityFaction.Player) <= 0 || TotalHealth(Entity_Preset.EntityFaction.Enemy) <= 0)
            {
                EndBattle();
            }
        }

        public void AutoPlay()
        {
            if (autoPlay == false)
                return;

            foreach (var item in players)
            {
                item.activeBook = item.rank.books[0].Value;
            }
            EndTurn();
        }
    }
    public enum BattleTemplate
    {
        None = 0,
        Test = 1,
    }
}
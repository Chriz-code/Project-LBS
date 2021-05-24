using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor.EventSystems;
using TMPro;
namespace Battle
{
    public class Battle_UI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI turnText = null;
        [SerializeField] CustomButton[] abilityButtons = new CustomButton[4];
        [SerializeField] int buttonIndex = 0;
        [SerializeField] bool cycleButtons = false;

        [Header("Targeting")]
        [SerializeField] Ability_Book activeBook = null;
        [SerializeField] Entity_Player activePlayer = null;
        [SerializeField] int enemyIndex = 0;
        [SerializeField] bool cycleEnemies = false;
        List<Entity_Enemy> targets = new List<Entity_Enemy>();

        private void Start()
        {
            BattleManager b = BattleManager.inst;
            UpdateTurnText(b.GetTurnSystem.Turn);
            b.GetTurnSystem.onTurnChange.AddListener(UpdateTurnText);
            b.GetTurnSystem.onPlayerEnter.AddListener(() => BattleManager.inst.PlayerIndex = 0);
            b.GetTurnSystem.onPlayerEnter.AddListener(EnableAbilityButtons);
            b.GetTurnSystem.onPlayerExit.AddListener(DisableAbilityButtons);
        }

        public void UpdateTurnText(TurnSystem.Turns turn)
        {
            if (!turnText)
            {
                Debug.LogWarning("Missing Reference");
                return;
            }
            switch (turn)
            {
                case TurnSystem.Turns.Start:
                    turnText.text = "Start";
                    break;
                case TurnSystem.Turns.PassiveStart:
                    break;
                case TurnSystem.Turns.PrePlayer:
                    break;
                case TurnSystem.Turns.Player:
                    turnText.text = "Player";
                    break;
                case TurnSystem.Turns.Enemy:
                    turnText.text = "Enemy";
                    break;
                case TurnSystem.Turns.PassiveBefore:
                    break;
                case TurnSystem.Turns.Play:
                    turnText.text = "Play";
                    break;
                case TurnSystem.Turns.PassiveAfter:
                    break;
                case TurnSystem.Turns.PassiveEnd:
                    break;
                case TurnSystem.Turns.End:
                    turnText.text = "End";
                    break;
            }
        }

        #region Buttoning
        public void EnableAbilityButtons()
        {
            Entity_Player player = BattleManager.inst.players[BattleManager.inst.PlayerIndex];
            for (int i = 0; i < abilityButtons.Length; i++)
            {
                int index = Mathf.Min(i, player.rank.books.Count - 1);
                Ability_Book book = BattleManager.inst.ability_Library.Books[player.rank.books[index].Value];
                string abilityName = book.Key;
                abilityButtons[i].Interactive = (book.ManaCost <= player.stats.MP);
                abilityButtons[i].text.text = abilityName;
                abilityButtons[i].value = abilityName;
            }
            //Last Button Always Skip
            abilityButtons[abilityButtons.Length - 1].Interactive = true;
            abilityButtons[abilityButtons.Length - 1].text.text = "Skip";
            abilityButtons[abilityButtons.Length - 1].value = "Skip";

            abilityButtons[buttonIndex].Select();
            cycleButtons = true;
        }
        public void DisableAbilityButtons()
        {
            cycleButtons = false;
            foreach (var item in abilityButtons)
            {
                item.Interactive = false;
            }
        }
        void CycleButtons(Vector2 val)
        {
            abilityButtons[buttonIndex].Deselect();

            buttonIndex -= Mathf.RoundToInt(val.y);

            buttonIndex = LoopValue(buttonIndex, abilityButtons.Length);
            abilityButtons[buttonIndex].Select();
        }
        void SelectButton()
        {
            if (abilityButtons[buttonIndex].Interactive)
            {
                if (abilityButtons[buttonIndex].value == "Skip")
                {
                    EndPlayerTurn();
                }
                else
                {
                    SelectAbility(abilityButtons[buttonIndex].value);
                }
            }
        }
        public void SelectAbility(string ability)
        {
            activePlayer = BattleManager.inst.players[BattleManager.inst.PlayerIndex];
            activeBook = BattleManager.inst.ability_Library.Books[ability];
            activePlayer.SetAbility(ability);

            DisableAbilityButtons();
            enemyIndex = 0;
            cycleEnemies = true;
            StartCoroutine(FlashEnemies());
        }
        #endregion

        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;
            Vector2 val = ctx.ReadValue<Vector2>();
            if (cycleButtons)
                CycleButtons(val);
            else if (cycleEnemies)
                CycleEnemies(val);

        }
        public void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;

            if (cycleButtons)
                SelectButton();
            else if (cycleEnemies)
                SelectEnemy();
        }
        int LoopValue(int a, int length)
        {
            if (a < 0)
                return length + a;
            return a % length;
        }

        #region Targeting
        void CycleEnemies(Vector2 val)
        {
            enemyIndex += Mathf.RoundToInt(val.x);
            enemyIndex = LoopValue(enemyIndex, BattleManager.inst.enemies.Count);
        }
        void SelectEnemy()
        {
            //Damage
            if (!activeBook.AbilityType.HasFlag(AbilityType.Support)) //Has to change when adding support, Shit too messy
            {
                Debug.Log($"{activeBook.Key} : {activeBook.TargetType}");
                if (activeBook.TargetType.HasFlag(TargetType.Random))
                {
                    Entity_Character[] targets;
                    if (activeBook.TargetType.HasFlag(TargetType.Multi))
                    {
                        BattleManager.inst.TryGetMultiRandoms(
                            activeBook.TargetCount,
                            Entity_Preset.EntityFaction.Enemy,
                            out targets);
                    }
                    else
                    {
                        BattleManager.inst.TryGetRandoms(
                            activeBook.TargetCount,
                            Entity_Preset.EntityFaction.Enemy,
                            out targets);
                    }
                    SelectFlash(BattleManager.inst.enemies.ToArray(), Color.black);
                    activePlayer.SetTargets(targets);
                    EndPlayerTurn();
                } //If Random or All
                else if (!activeBook.TargetType.HasFlag(TargetType.Random)) //If Not Random
                {
                    Entity_Enemy target = BattleManager.inst.enemies[enemyIndex];
                    if (activeBook.TargetType.HasFlag(TargetType.Single))
                    {
                        if (!targets.Contains(target))
                        {
                            targets.Add(target);
                            SelectFlash(target, Color.black);
                        }
                        if (targets.Count >= activeBook.TargetCount || targets.Count >= BattleManager.inst.enemies.Count)
                        {
                            activePlayer.SetTargets(targets.ToArray());
                            targets = new List<Entity_Enemy>();
                            EndPlayerTurn();
                        }
                    }
                    else
                    {
                        targets.Add(target);
                        SelectFlash(target, Color.black);
                        if (targets.Count >= activeBook.TargetCount)
                        {
                            activePlayer.SetTargets(targets.ToArray());
                            targets = new List<Entity_Enemy>();
                            EndPlayerTurn();
                        }
                    }
                }
            }
        }

        float flashTime = 0.1f;
        void SelectFlash(Entity_Character target, Color col)
        {
            target.Flash(col, flashTime);
        }
        void SelectFlash(Entity_Character[] targets, Color col)
        {
            foreach (var item in targets)
            {
                item.Flash(col, flashTime);
            }
        }
        IEnumerator FlashEnemies()
        {
            while (cycleEnemies)
            {
                if (!activeBook.TargetType.HasFlag(TargetType.Random))
                {
                    BattleManager.inst.enemies[enemyIndex].Flash(Color.gray, flashTime);
                }
                else
                {
                    foreach (var item in BattleManager.inst.enemies)
                    {
                        item.Flash(Color.gray, flashTime);
                    }
                }
                yield return new WaitForSeconds(flashTime * 2);
            }
        }
        #endregion

        void EndPlayerTurn()
        {
            cycleEnemies = false;
            activePlayer = null;
            activeBook = null;
            enemyIndex = 0;

            if (BattleManager.inst.PlayerIndex + 1 >= BattleManager.inst.players.Count)
            {
                BattleManager.inst.EndTurn();
                DisableAbilityButtons();
            }
            else
            {
                BattleManager.inst.PlayerIndex++;
                EnableAbilityButtons();
            }
        }
    }
    [System.Serializable]
    public class CustomButton
    {
        public Image image = null;
        public Text text = null;
        public string value = "";

        public Color normalColor = Color.white;
        public Color disabledColor = Color.gray;
        public Color highlightColor = Color.cyan;
        bool enabled = true;
        public bool Interactive
        {
            get => enabled;
            set
            {
                if (value == false)
                {
                    image.color = disabledColor;
                }
                else
                {
                    image.color = normalColor;
                }
                enabled = value;
            }
        }
        public void Select()
        {
            if (Interactive)
                image.color = highlightColor;
            else
            {
                image.color = highlightColor * disabledColor;
            }
        }
        public void Deselect()
        {
            if (Interactive)
                image.color = normalColor;
            else
                image.color = disabledColor;
        }
    }
}

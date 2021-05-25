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
        [SerializeField] Transform abilityButtonParent = null;
        [SerializeField] GameObject abilityButtonPrefab = null;
        [SerializeField] List<CustomButton> abilityButtons = new List<CustomButton>();
        [SerializeField] int buttonIndex = 0;
        [SerializeField] bool cycleButtons = false;

        [Header("Targeting")]
        [SerializeField] Ability_Book activeBook = null;
        [SerializeField] Entity_Player activePlayer = null;
        [SerializeField] int targetIndex = 0;
        [SerializeField] bool cycleTargets = false;
        List<Entity_Character> targets = new List<Entity_Character>();

        Entity_Preset.EntityFaction targetFaction = Entity_Preset.EntityFaction.Neutral;

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
            foreach (var item in abilityButtons) Destroy(item.image.gameObject);
            abilityButtons = new List<CustomButton>();
            for (int i = 0; i < player.rank.books.Count; i++)//Temp
            {
                Ability_Book book = BattleManager.inst.ability_Library.Books[player.rank.books[i].Value];
                abilityButtons.Add(new CustomButton(Instantiate(abilityButtonPrefab, abilityButtonParent), book.Key, book.ManaCost <= player.Stats.MP));
            }
            abilityButtons.Add(new CustomButton(Instantiate(abilityButtonPrefab, abilityButtonParent), "Skip"));
            abilityButtons.Add(new CustomButton(Instantiate(abilityButtonPrefab, abilityButtonParent), "Defend"));

            abilityButtons[buttonIndex].Select();
            cycleButtons = true;
        }
        public void DisableAbilityButtons()
        {
            cycleButtons = false;
            foreach (var item in abilityButtons)
            {
                item.SetInteractive(false);
            }
        }
        void CycleButtons(Vector2 val)
        {
            abilityButtons[buttonIndex].Deselect();

            buttonIndex -= Mathf.RoundToInt(val.y);

            buttonIndex = LoopValue(buttonIndex, abilityButtons.Count);
            abilityButtons[buttonIndex].Select();
        }
        void SelectButton()
        {
            if (abilityButtons[buttonIndex].GetInteractive)
            {
                cycleButtons = false;
                SelectAbility(abilityButtons[buttonIndex].book.Value);
            }
        }
        public void SelectAbility(string ability)
        {
            activePlayer = BattleManager.inst.players[BattleManager.inst.PlayerIndex];
            activeBook = BattleManager.inst.ability_Library.Books[ability];
            activePlayer.SetAbility(ability);

            DisableAbilityButtons();

            targetIndex = 0;
            targetFaction = activeBook.TargetType.HasFlag(TargetType.Support) ? Entity_Preset.EntityFaction.Player : Entity_Preset.EntityFaction.Enemy;
            if (activeBook.TargetType.HasFlag(TargetType.Self))
            {
                activePlayer.SetTarget(activePlayer);
                EndPlayerTurn();
            }
            else
            {
                cycleTargets = true;
                StartCoroutine(FlashTargets());
            }
        }
        #endregion

        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;
            Vector2 val = ctx.ReadValue<Vector2>();
            if (cycleButtons)
                CycleButtons(val);
            else if (cycleTargets)
                CycleTargets(val);

        }
        public void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;

            if (cycleButtons)
                SelectButton();
            else if (cycleTargets)
                SelectTarget();
        }
        int LoopValue(int a, int length)
        {
            if (a < 0)
                return length + a;
            return a % length;
        }

        #region Targeting
        void CycleTargets(Vector2 val)
        {
            targetIndex += Mathf.RoundToInt(val.x);
            targetIndex = LoopValue(targetIndex, BattleManager.inst.GetFaction(targetFaction).Length);
        }
        void SelectTarget()
        {
            Debug.Log($"{activeBook.Key} : {activeBook.TargetType}");
            if (activeBook.TargetType.HasFlag(TargetType.Random)) //If Random or All
            {
                Entity_Character[] targets;
                if (activeBook.TargetType.HasFlag(TargetType.Multi))
                {
                    BattleManager.inst.TryGetMultiRandoms(
                        activeBook.TargetCount,
                        targetFaction,
                        out targets);
                }
                else
                {
                    BattleManager.inst.TryGetRandoms(
                        activeBook.TargetCount,
                        targetFaction,
                        out targets);
                }
                activePlayer.SetTargets(targets);
                EndPlayerTurn();
            }
            else
            {
                Entity_Enemy target = BattleManager.inst.enemies[targetIndex];
                if (activeBook.TargetType.HasFlag(TargetType.Single))
                {
                    if (!targets.Contains(target))
                    {
                        targets.Add(target);
                    }
                    if (targets.Count >= activeBook.TargetCount || targets.Count >= BattleManager.inst.enemies.Count)
                    {
                        activePlayer.SetTargets(targets.ToArray());
                        targets = new List<Entity_Character>();
                        EndPlayerTurn();
                    }
                }
                else
                {
                    targets.Add(target);
                    if (targets.Count >= activeBook.TargetCount)
                    {
                        activePlayer.SetTargets(targets.ToArray());
                        targets = new List<Entity_Character>();
                        EndPlayerTurn();
                    }
                }
            }
        }

        float flashTime = 0.1f;
        IEnumerator FlashTargets()
        {
            while (cycleTargets)
            {
                if (!activeBook.TargetType.HasFlag(TargetType.Random))
                {
                    BattleManager.inst.GetFaction(targetFaction)[targetIndex].Flash(Color.gray, flashTime);
                }
                else
                {
                    foreach (var item in BattleManager.inst.GetFaction(targetFaction))
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
            cycleTargets = false;
            activePlayer = null;
            activeBook = null;
            targetIndex = 0;
            buttonIndex = 0;

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
        public BookReference book;

        public Color normalColor = Color.white;
        public Color disabledColor = Color.gray;
        public Color highlightColor = Color.cyan;
        [SerializeField] bool enabled = true;
        public void SetInteractive(bool value)
        {
            enabled = value;
            if (!value)
            {
                image.color = disabledColor;
            }
            else
            {
                image.color = normalColor;
            }
        }
        public bool GetInteractive { get => enabled; }

        public void Select()
        {
            if (GetInteractive)
                image.color = highlightColor;
            else
            {
                image.color = highlightColor * disabledColor;
            }
        }
        public void Deselect()
        {
            if (GetInteractive)
                image.color = normalColor;
            else
                image.color = disabledColor;
        }

        public CustomButton(GameObject g, string value, bool interactive = true)
        {
            image = g.GetComponent<Image>();
            text = image.transform.GetChild(0).GetComponent<Text>();
            text.text = value;
            book = new BookReference(value);
            SetInteractive(interactive);
        }
    }
}

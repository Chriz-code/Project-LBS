using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    [RequireComponent(typeof(BattleManager))]
    public class TurnSystem : MonoBehaviour
    {
        public float timeScale = 1;
        public delegate void TurnEvent(out float duration);
        #region Turn
        public enum Turns
        {
            Start,
            PassiveStart,

            //---Loop
            PrePlayer,
            Player,
            Enemy,
            PassiveBefore,
            Play, //Can jump out to end
            PassiveAfter,
            //---Loop
            PassiveEnd,
            End,
        }
        [SerializeField] Turns turn = Turns.Start;

        public Turns Turn
        {
            get => turn;
            set
            {

                if (turn != value)
                {
                    if (turn == Turns.Player) onPlayerExit.Invoke();
                    turn = value;
                    onTurnChange.Invoke(turn);
                    if (turn == Turns.Player) onPlayerEnter.Invoke();
                }
            }
        }
        [HideInInspector] public UnityEvent<Turns> onTurnChange = new UnityEvent<Turns>();
        [HideInInspector] public UnityEvent onPlayerEnter = new UnityEvent();
        [HideInInspector] public UnityEvent onPlayerExit = new UnityEvent();


        /// <summary>
        /// Before PassiveStart
        /// </summary>
        public List<TurnEvent> onStart = new List<TurnEvent>();
        /// <summary>
        /// Before PrePlayer, After Start
        /// </summary>
        public List<TurnEvent> onPassiveStart = new List<TurnEvent>();
        /// <summary>
        /// BeforePlayer, After PassiveStart
        /// </summary>
        public List<TurnEvent> onPrePlayer = new List<TurnEvent>();
        /// <summary>
        /// Before Enemy, After PrePlayer
        /// </summary>
        public List<TurnEvent> onPlayer = new List<TurnEvent>();
        /// <summary>
        /// Before PassiveBefore, After Player
        /// </summary>
        public List<TurnEvent> onEnemy = new List<TurnEvent>();
        /// <summary>
        /// Before Playe, After Enemy
        /// </summary>
        public List<TurnEvent> onPassiveBefore = new List<TurnEvent>();
        /// <summary>
        /// Before PasiveAfter, After PassiveBefore
        /// </summary>
        public List<TurnEvent> onPlay = new List<TurnEvent>();
        /// <summary>
        /// Before PassiveEnd, After Play
        /// </summary>
        public List<TurnEvent> onPassiveAfter = new List<TurnEvent>();
        /// <summary>
        /// Before End, After PassiveAfter
        /// </summary>
        public List<TurnEvent> onPassiveEnd = new List<TurnEvent>();
        /// <summary>
        /// After PassiveEnd
        /// </summary>
        public List<TurnEvent> onEnd = new List<TurnEvent>();
        #endregion


        [SerializeField] List<Entity_Character> characters;
        public void Initiate()
        {
            SortCharacters();
            StartTurn(Turns.Start);
        }
        /// <summary>
        /// Clears all play actions and reinserts based on speed
        /// </summary>
        public void SortCharacters()
        {
            //Sort
            characters = BattleManager.inst.GetAllCharacters.OrderByDescending(o => o.stats.Spd).ToList();
        }
        public void SetAbilities()
        {
            SortCharacters();
            //Add
            onPlay = new List<TurnEvent>();
            for (int i = 0; i < characters.Count; i++)
            {
                onPlay.AddRange(characters[i].battleActions);
            }
        }

        public void StartTurn(Turns turn)
        {
            if (routine != null)
            {
                Debug.LogWarning("Something tried to start coroutine during routine at:" + turn);
                return;
            }
            routine = Play(turn);
            StartCoroutine(routine);
        }
        public void ForceTurn(Turns turn)
        {
            Debug.LogWarning("Forced Turn:" + turn);
            StopCoroutine(routine);
            Turn = turn;
            routine = Play(turn);
            StartCoroutine(routine);
        }

        IEnumerator routine = null;
        IEnumerator Play(Turns turn)
        {
            //Set
            Turn = turn;
            List<TurnEvent> events;
            switch (turn)
            {
                case Turns.Start:
                    events = onStart;
                    break;
                case Turns.PassiveStart:
                    events = onPassiveStart;
                    break;
                case Turns.PrePlayer:
                    events = onPrePlayer;
                    break;
                case Turns.Player:
                    events = onPlayer;
                    break;
                case Turns.Enemy:
                    events = onEnemy;
                    break;
                case Turns.PassiveBefore:
                    events = onPassiveBefore;
                    break;
                case Turns.Play:
                    SetAbilities();
                    events = onPlay;
                    break;
                case Turns.PassiveAfter:
                    events = onPassiveAfter;
                    break;
                case Turns.PassiveEnd:
                    events = onPassiveEnd;
                    break;
                case Turns.End:
                default:
                    events = onEnd;
                    break;
            }

            //Play
            foreach (var item in events)
            {
                item(out float duration);
                yield return new WaitForSeconds(duration);
            }

            //End
            routine = null;
            switch (turn)
            {
                case Turns.Start:
                    StartTurn(Turns.PassiveStart);
                    break;
                case Turns.PassiveStart:
                    StartTurn(Turns.PrePlayer);
                    break;

                case Turns.PrePlayer: //---Loop
                    Turn = Turns.Player;
                    break;
                case Turns.Player:
                    StartTurn(Turns.Enemy);
                    break;
                case Turns.Enemy:
                    StartTurn(Turns.PassiveBefore);
                    break;
                case Turns.PassiveBefore:
                    StartTurn(Turns.Play);
                    break;
                case Turns.Play:
                    StartTurn(Turns.PassiveAfter);
                    break;
                case Turns.PassiveAfter:
                    StartTurn(Turns.PrePlayer);
                    break; //---Loop

                case Turns.PassiveEnd:
                    StartTurn(Turns.End);
                    break;
            }
        }

        private void OnValidate()
        {
            Time.timeScale = timeScale;
        }
    }
}
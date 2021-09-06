using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UI_Manager : MonoBehaviour
    {
        public GameObject playerMenu = null;
        public UI_Navigation nav = null;
        public UI_Visuals vis = null;
        public UI_Action act = null;
        public enum UIState { Cube, List, Target }
        [SerializeField] UIState state = UIState.Cube;
        public UIState State
        {
            get => state;
            set
            {
                UIState newVal = (UIState)Mathf.Clamp((int)value, 0, nav.yLeave.Length - 1);
                if (state != newVal)
                {
                    onStateChange.Invoke(newVal);
                }
                state = newVal;
            }
        }
        [HideInInspector] public UnityEngine.Events.UnityEvent<UIState> onStateChange = new UnityEngine.Events.UnityEvent<UIState>();

        private void Awake()
        {
            onStateChange.AddListener(StateChange);
            nav.onXIndexChange.AddListener(XUpdate);
            nav.onYIndexChange.AddListener(YUpdate);
            act.onTargetSet.AddListener(EndPlayerTurn);
            vis.UpdateCubeText(0);
        }
        private void Start()
        {
            BattleManager.inst.GetTurnSystem.onTurnChange.AddListener(vis.UpdateTurnText);
            BattleManager.inst.GetTurnSystem.onTurnChange.AddListener(TurnState);
        }

        public void XUpdate(int xVal, int xDir)
        {
            UIState os = State;
            if (os == UIState.List && nav.yLeave[0] == 0 && xDir > 0) act.SetAbility(nav.YIndex);
            State = (UIState)xVal;
            if (os == UIState.Cube && xDir > 0) vis.CubeAnimation(Vector2.right);
            if (os == UIState.List && xDir < 0) vis.CubeAnimation(Vector2.left);
            if (os == UIState.Target && xDir > 0) act.SetTarget(nav.YIndex);
        }
        public void YUpdate(int yVal, int yDir)
        {
            switch (State)
            {
                case UIState.Cube:
                    vis.UpdateCubeText(yVal);
                    vis.CubeAnimation(Vector2.up * yDir);
                    break;
                case UIState.List:
                    vis.ListAnimate(nav.yLeave[0], yVal);
                    break;
                case UIState.Target:
                    vis.TargetAnimate(yVal);
                    break;
            }

        }
        public void StateChange(UIState newState)
        {
            Debug.Log(newState);
            switch (newState)
            {
                case UIState.Cube:
                    vis.ListExit();
                    vis.TargetExit();
                    act.Reset();
                    nav.ySize = 4;
                    vis.CubeEnter();
                    break;
                case UIState.List:
                    vis.CubeExit();
                    vis.TargetExit();
                    act.Reset();
                    nav.ySize = nav.yLeave[0] == 0 ? BattleManager.inst.IndexPlayer.rank.books.Count : 4;
                    vis.ListEnter(nav.yLeave[0], nav.yLeave[1]);
                    break;
                case UIState.Target:
                    vis.CubeExit();
                    vis.ListExit();
                    Ability_Book book = BattleManager.inst.ability_Library.Books[act.book.Key];
                    nav.ySize = book.TargetType == TargetType.Support ? BattleManager.inst.GetFaction(Entity_Preset.EntityFaction.Player).Length : BattleManager.inst.GetFaction(Entity_Preset.EntityFaction.Enemy).Length;
                    vis.TargetEnter(book);
                    break;
            }
        }

        public void TurnState(TurnSystem.Turns turn)
        {
            if(turn == TurnSystem.Turns.Player)
            {
                playerMenu.SetActive(true);
                vis.SetCubeActive(true);
            }
            else
            {
                playerMenu.SetActive(false);
                vis.SetCubeActive(false);
            }
        }

        public void EndPlayerTurn(Entity_Character[] ts)
        {
            BattleManager.inst.PlayerIndex++;
            nav.Reset();
            return;
        }
    }
}
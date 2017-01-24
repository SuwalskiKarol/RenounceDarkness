using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Scripts.FirstLevel
{
    /// <summary>
    /// Actions
    /// </summary>
    public enum Transition
    {
        NullTransition = 0,
        IdleEnd,
        WalkEnd,
        SawPlayer,
        Attack,
        FarAway,
        LowHealth,
        Beaten,
        FlyBack,
        Falling,
        StartDie,
    }
    /// <summary>
    /// States
    /// </summary>
    public enum StateID
    {
        NullStateID = 0,
        Idle,
        RandomWalk,
        BattleRoar,
        BattleAttack,
        BattleRun,
        BattleRetreat,
        BattleBeaten,
        BattleFlyBack,
        BattleFireball,
        Die,
    }

    public abstract class FiniteStateMachineSystem
    {
        protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
        protected StateID stateID;
        public StateID ID { get { return stateID; } }
        public QState MDPQstate { get; set; }

        public void AddTransition(Transition trans, StateID id)
        {
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR(AddTransition): NullTransition is not allowed for a real transition");
                return;
            }

            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSMState ERROR(AddTransition): NullStateID is not allowed for a real ID");
                return;
            }

            if (map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR(AddTransition): State" + stateID.ToString() + "already has transition" + trans.ToString() + "Impossible to assign to another state");
                return;
            }

            map.Add(trans, id);

        }

        public void DeleteTransition(Transition trans)
        {
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR(DeleteTransition): NullTransition is not allowed");
                return;
            }

            if (map.ContainsKey(trans))
            {
                map.Remove(trans);
                return;
            }
            Debug.LogError("FSMState ERROR(DeleteTransition): Transition" + trans.ToString() + " passed to " + stateID.ToString() + " was not on the state's transition list");

        }

        public StateID GetOutputState(Transition trans)
        {
            if (map.ContainsKey(trans))
            {
                return map[trans];
            }
            return StateID.NullStateID;
        }

        //Thinks to do before entering state
        public virtual void DoBeforeEntering() { }
        //thinks to do before leave state
        public virtual void DoBeforeLeaving() { }
        //Thinks that must be done to go to the other stage
        public abstract void Reason(GameObject player, GameObject npc);
        //thinks to do in this stage
        public abstract void Act(GameObject player, GameObject npc);
    }


    public class FSMSystem
    {
        private List<FiniteStateMachineSystem> states;

        private StateID currentStateID;
        public StateID CurrentStateID { get { return currentStateID; } }
        private FiniteStateMachineSystem currentState;
        public FiniteStateMachineSystem CurrentState { get { return currentState; } }

        public FSMSystem()
        {
            states = new List<FiniteStateMachineSystem>();
        }

        public void AddState(FiniteStateMachineSystem s)
        {
            if (s == null)
            {
                Debug.LogError("FSM ERROR(AddState): Null reference is not allowed");
            }

            if (states.Count == 0)
            {
                states.Add(s);
                currentState = s;
                currentStateID = s.ID;
                return;
            }

            foreach (FiniteStateMachineSystem state in states)
            {
                if (state.ID == s.ID)
                {
                    Debug.LogError("FSM ERROR(AddState): Impossible to add state " + s.ID.ToString() +
                        " because state has already been added");
                    return;
                }

            }
            states.Add(s);
        }

        public void DeleteState(StateID id)
        {
            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSM ERROR(DeleteState): NullStateID is not allowed for a real state");
                return;
            }

            foreach (FiniteStateMachineSystem state in states)
            {
                if (state.ID == id)
                {
                    states.Remove(state);
                    return;
                }

            }
            Debug.LogError("FSM ERROR(DeleteState): Impossible to delete state " + id.ToString() +
                ". It was not on the list of states");
        }

        public void PerformAITransition(StateID NextStateID)
        {
            currentStateID = NextStateID;
            foreach (FiniteStateMachineSystem state in states)
            {
                if (state.ID == currentStateID)
                {
                    currentState.DoBeforeLeaving();
                    currentState = state;
                    currentState.DoBeforeEntering();
                    break;
                }

            }

        }

        public void PerformTransition(Transition trans)
        {
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSM ERROR(PerformTransition): NullTransition is not allowed for a real transition");
                return;
            }

            StateID id = currentState.GetOutputState(trans);
            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSM ERROR(PerformTransition): State " + currentStateID.ToString() + " does not have a target state " +
                    " for transition " + trans.ToString());
                return;

            }

            currentStateID = id;
            foreach (FiniteStateMachineSystem state in states)
            {
                if (state.ID == currentStateID)
                {
                    currentState.DoBeforeLeaving();
                    currentState = state;
                    currentState.DoBeforeEntering();
                    break;

                }

            }
        }
    }
}

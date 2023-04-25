using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tactics.StateMachine
{
    [CreateAssetMenu(fileName = "Machine", menuName = "State Machine/State Machine", order = 0)]
    public class StateMachine : ScriptableObject
    {
        //Active/Gameplay
        public State CurrentState => _stateGraphHistory.Peek();
        private Stack<State> _stateGraphHistory = new Stack<State>();

        //Config
        public State DefaultState => _defaultState;
        private State _defaultState;
        
        //States
        [HideInInspector]   
        public List<State> states = new List<State>();


        public void Init()
        {
            _stateGraphHistory = new Stack<State>();
        }

        public void EnterState(State newState)
        {
            if (_stateGraphHistory.Count > 0)
            {
                if (newState == CurrentState)
                {
                    Debug.Log("Can't enter a state from itself.");
                    return;
                }
                CurrentState.Exit();
            }
            
            _stateGraphHistory.Push(newState);
            newState.Enter();
        }

        public bool LeaveState()
        {
            //todo: if there is another state to fall back to, exit state and return to that previous one.
            return false;
        }
        
        public void SetDefaultState(State newState)
        {
            if (states.Contains(newState))
            {
                _defaultState = newState;
            }
        }

        private void OnValidate()
        {
            //We probably just created a new state.
            //todo move validate to editor
            if (DefaultState == null && states.Count == 1)
            {
                SetDefaultState(states[0]);
            }
        }
    }
}

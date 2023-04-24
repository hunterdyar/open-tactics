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

        public State DefaultState => _defaultState;
        private State _defaultState;

        
        [HideInInspector]   
        public List<State> states = new List<State>();

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

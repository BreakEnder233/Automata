using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class Automaton<T>
    {
        private readonly T _context;
        private readonly List<State<T>> _states;
        private State<T> _currentState;

        public Automaton(T context)
        {
            this._context = context;
            _states = new List<State<T>>();
        }

        #region API
        public int AddState(State<T>.UpdateFunction updateFunction = null,
            State<T>.EntryFunction entryFunction = null,
            State<T>.ExitFunction exitFunction = null)
        {
            int index = _states.Count;
            State<T> newState;
            if (updateFunction != null) newState = new State<T>(updateFunction);
            else newState = new State<T>();
            if(entryFunction != null) newState.AddEntry(entryFunction);
            if (exitFunction != null) newState.AddExit(exitFunction);
            _states.Add(newState);
            return index;
        }
        public void AddEntry(int stateId, State<T>.EntryFunction entry)
        {
            var state = _states[stateId];
            state.AddEntry(entry);
        }
        public void AddTransfer(int origin, int target, State<T>.ConditionFunction condition, State<T>.TransferFunction transferFunction)
        {
            var state0 = _states[origin];
            var state1 = _states[target];
            state0.AddTransfer(state1, condition, transferFunction);
        }
        public void AddExit(int stateId, State<T>.ExitFunction exit)
        {
            var state = _states[stateId];
            state.AddExit(exit);
        }
        public void Init(int initialState)
        {
            _currentState = _states[initialState];
        }
        public void Update()
        {
            _currentState?.Update(_context);
            _currentState = _currentState?.GetNextState(_context);
        }
        public void TransferForce(int state)
        {
            _currentState.ExitForce(_context);
            _currentState = _states[state];
            _currentState.EnterForce(_context);
        }
        #endregion


        #region Convenience
        public void Trim()
        {
            _states.TrimExcess();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class State<T>
    {
        public delegate void UpdateFunction(T context);
        public delegate bool ConditionFunction(T context);
        public delegate void EntryFunction(T context);
        public delegate void TransferFunction(T context);
        public delegate void ExitFunction(T context);

        private readonly UpdateFunction _update;
        private event EntryFunction _entryEvent;
        private readonly List<Route> _routes;
        private event ExitFunction _exitEvent;


        internal State()
        {
            _routes = new List<Route>();
        }
        internal State(UpdateFunction updateFunction)
        {
            _update = updateFunction;
            _routes = new List<Route>();
        }
        #region API
        internal void Update(T context)
        {
            _update?.Invoke(context);
        }
        internal State<T> GetNextState(T context)
        {
            foreach(var route in _routes)
            {
                if (route.condition(context))
                {
                    _exitEvent?.Invoke(context);
                    route.transfer(context);
                    route.nextState._entryEvent?.Invoke(context);
                    return route.nextState;
                }
            }
            return this;
        }
        internal void EnterForce(T context)
        {
            _entryEvent?.Invoke(context);
        }
        internal void ExitForce(T context)
        {
            _exitEvent?.Invoke(context);
        }
        internal void AddEntry(EntryFunction entry)
        {
            _entryEvent += entry;
        }
        internal void AddTransfer(State<T> targetState, ConditionFunction condition, TransferFunction transfer)
        {
            _routes.Add(new Route
            {
                nextState = targetState,
                condition = condition,
                transfer = transfer
            });
        }
        internal void AddExit(ExitFunction exit)
        {
            _exitEvent += exit;
        }
        #endregion

        #region Convenience
        internal void Trim()
        {
            _routes.TrimExcess();
        }
        #endregion
        private struct Route
        {
            public State<T> nextState;
            public ConditionFunction condition;
            public TransferFunction transfer;
        }
    }
}

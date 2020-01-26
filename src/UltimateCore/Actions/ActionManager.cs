using System;
using System.Collections.Generic;

namespace UltimateCore.Actions
{
        public abstract class ActionManager<T>
        {
                private class ActionWrapper<K> : IAction
                {
                        private readonly K _key;

                        private readonly Action<K, ActionParameter<K>> _innerAction;

                        private readonly Func<K, ActionParameter<K>, bool> _innerFunc;

                        public ActionWrapper(K key, Action<K, ActionParameter<K>> action, Func<K, ActionParameter<K>, bool> actionCanExecute)
                        {
                                _key = key;
                                _innerAction = action;
                                _innerFunc = actionCanExecute;
                        }

                        public void Invoke(object parameter)
                        {
                                this.Invoke(null, parameter);
                        }

                        public void Invoke(object sender, object parameter)
                        {
                                ActionParameter<K> actionParameter = new ActionParameter<K>(sender, this._key, parameter);

                                if (this._innerFunc == null
                                        || this._innerFunc(this._key, actionParameter))
                                {
                                        this._innerAction?.Invoke(this._key, actionParameter);
                                }
                        }
                }

                private readonly Dictionary<T, IAction> _actionContainer = new Dictionary<T, IAction>();

                public Action<T, ActionParameter<T>> Action
                {
                        get;
                        set;
                }

                public Func<T, ActionParameter<T>, bool> ActionCanExecute
                {
                        get;
                        set;
                }


                public IAction this[T key]
                {
                        get
                        {
                                if (this._actionContainer.TryGetValue(key, out IAction actionwrapper))
                                {
                                        return actionwrapper;
                                }

                                ActionWrapper<T> wrapper = new ActionWrapper<T>(key, this.Action, this.ActionCanExecute);

                                this._actionContainer.Add(key, wrapper);

                                return wrapper;
                        }
                }
        }
}

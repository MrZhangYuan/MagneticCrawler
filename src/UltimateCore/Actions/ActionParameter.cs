namespace UltimateCore.Actions
{
        public class ActionParameter<T>
        {
                public object Sender
                {
                        get;
                }
                public T ActionKey
                {
                        get;
                }
                public object Parameter
                {
                        get;
                }

                public ActionParameter(object sender, T actionKey, object parameter)
                {
                        Sender = sender;
                        ActionKey = actionKey;
                        Parameter = parameter;
                }
        }
}

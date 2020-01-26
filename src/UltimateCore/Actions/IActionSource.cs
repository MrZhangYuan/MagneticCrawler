namespace UltimateCore.Actions
{
        public interface IActionSource
        {
                IAction Action
                {
                        get;
                }
                object ActionParameter
                {
                        get;
                }
        }
}

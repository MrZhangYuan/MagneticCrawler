using System.Linq;
using System.Text;

namespace UltimateCore.Actions
{
        public class DefaultActionManager : ActionManager<string>
        {
                public static DefaultActionManager Instance
                {
                        get;
                }

                static DefaultActionManager()
                {
                        Instance = new DefaultActionManager();
                }
        }
}

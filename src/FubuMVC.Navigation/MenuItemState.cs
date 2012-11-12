using System.Linq;

namespace FubuMVC.Navigation
{
    public class MenuItemState
    {
        public static MenuItemState Hidden = new MenuItemState("Hidden", 0, false, false);
        public static MenuItemState Disabled = new MenuItemState("Disabled", 1, true, false);
        public static MenuItemState Available = new MenuItemState("Available", 2, true, true);
        public static MenuItemState Active = new MenuItemState("Active", 3, true, true);
        private readonly string _name;
        private readonly int _level;
        private readonly bool _isShown;
        private readonly bool _isEnabled;

        private MenuItemState(string name, int level, bool isShown, bool isEnabled)
        {
            _name = name;
            _level = level;
            _isShown = isShown;
            _isEnabled = isEnabled;
        }

        public bool IsShown
        {
            get { return _isShown; }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        public string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            return string.Format("MenuItemState: {0}", _name);
        }

        public static MenuItemState Least(params MenuItemState[] states)
        {
            return states.OrderBy(x => x._level).FirstOrDefault() ?? Available;
        }
    }
}
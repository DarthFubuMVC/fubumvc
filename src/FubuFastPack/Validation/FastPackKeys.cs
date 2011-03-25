using FubuLocalization;

namespace FubuFastPack.Validation
{
    public class FastPackKeys : StringToken
    {
        protected FastPackKeys(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public static readonly StringToken FIELD_MUST_BE_UNIQUE = new FastPackKeys("FIELD_MUST_BE_UNIQUE", "Is already in use and must be unique");
        public static readonly FastPackKeys SAVE_KEY = new FastPackKeys("SAVE", "Save");
        public static readonly StringToken NOT_AUTHORIZED = new FastPackKeys("NOT_AUTHORIZED", "You are not authorized to perform this action");
        public static readonly StringToken INVALID_TYPE_CONVERSION = new FastPackKeys("INVALID_TYPE_CONVERSION", "{0} is invalid. The value must be {1}.");
        public static readonly StringToken INVALID_VALUE = new FastPackKeys("INVALID_VALUE", "The value {0} is invalid");
        public static readonly StringToken PARSE_VALUE = new FastPackKeys("PARSE_VALUE", "Invalid data");
    }
}
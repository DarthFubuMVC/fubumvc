namespace FubuFastPack.Querying
{
    public static class FilterTypeExtensions
    {
        public static string Operator(this IFilterType filter)
        {
            return filter.Key.Key;
        }

        public static OperatorDTO ToDTO(this IFilterType filter)
        {
            return new OperatorDTO(){
                display = filter.Key.ToString(),
                value = filter.Key.Key
            };
        }
    }
}
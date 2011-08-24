namespace FubuMVC.Core.Assets.Content
{
    public enum ActionType
    {
        Generate = 1,
        Substitution = 2,
        Transformation = 3,
        BatchedTransformation = 4, 
        Global = 5 // minification mostly, but might use this for tracing too
    }
}
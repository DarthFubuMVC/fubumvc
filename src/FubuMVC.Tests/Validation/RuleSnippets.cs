using FubuMVC.Core.Validation;

namespace FubuMVC.Tests.Validation
{
	public class RuleSnippets
	{
// SAMPLE: GreaterThanZeroRuleAttribute
public int GreaterThanZero { get; set; }
// ENDSAMPLE

// SAMPLE: GreaterThanOrEqualToZeroRuleAttribute
public int GreaterOrEqualToZero { get; set; }
// ENDSAMPLE

// SAMPLE: MinLengthRuleAttribute
public string LongerThanTen { get; set; }
// ENDSAMPLE

// SAMPLE: MaxLengthRuleAttribute
public string NoMoreThanFiveCharacters { get; set; }
// ENDSAMPLE

// SAMPLE: RangeLengthRuleAttribute
public string AtLeastFiveButNotTen { get; set; }
// ENDSAMPLE

// SAMPLE: MinValueRuleAttribute
public int GreaterThanFive { get; set; }
// ENDSAMPLE

// SAMPLE: MaxValueRuleAttribute
public double LessThanFifteen { get; set; }
// ENDSAMPLE

// SAMPLE: EmailRuleAttribute
public string Email { get; set; }
// ENDSAMPLE

// SAMPLE: RequiredRuleAttribute
[Required]
public string Required { get; set; }
// ENDSAMPLE
	}

	public class RuleSnippetsDsl : ClassValidationRules<RuleSnippets>
	{
		public RuleSnippetsDsl()
		{
// SAMPLE: GreaterThanZeroRuleDsl
Property(x => x.GreaterThanZero).GreaterThanZero();
// ENDSAMPLE

// SAMPLE: GreaterThanOrEqualToZeroRuleDsl
Property(x => x.GreaterOrEqualToZero).GreaterOrEqualToZero();
// ENDSAMPLE

// SAMPLE: MinLengthRuleDsl
Property(x => x.LongerThanTen).MinimumLength(10);
// ENDSAMPLE

// SAMPLE: MaxLengthRuleDsl
Property(x => x.NoMoreThanFiveCharacters).MaximumLength(5);
// ENDSAMPLE

// SAMPLE: RangeLengthRuleDsl
Property(x => x.AtLeastFiveButNotTen).RangeLength(5, 10);
// ENDSAMPLE

// SAMPLE: MinValueRuleDsl
Property(x => x.GreaterThanFive).MinValue(5);
// ENDSAMPLE

// SAMPLE: MaxValueRuleDsl
Property(x => x.LessThanFifteen).MaxValue(15);
// ENDSAMPLE

// SAMPLE: EmailRuleDsl
Property(x => x.Email).Email();
// ENDSAMPLE

// SAMPLE: RequiredRuleDsl
Property(x => x.Required).Required();
// ENDSAMPLE
		}
	}
}
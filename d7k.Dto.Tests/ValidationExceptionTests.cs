using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class ValidationExceptionTests
	{
		[TestMethod]
		public void IssueDescription_Compare_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new CompareRule.Greater() { Value = 1 });

			try
			{
				validation.Validate(new SaveLotDtoTemp() { Sum = 0 }).ThrowIssues();
				Assert.Fail();
			}
			catch (ValidationException e)
			{
				e.Issues.Should().HaveCount(1);
				e.Issues[0].Code.Should().Be("CompareRule.Greater");
				e.Issues[0].Message.Should().ContainAll("Sum", "not greater", "1");

				((BasicDescription)e.Issues[0].Description).Should().BeOfType(typeof(BasicDescription));
				var description = (BasicDescription)e.Issues[0].Description;
				description.Path.Should().Be(".Sum");
			}
		}

		[TestMethod]
		public void IssueDescription_NotEmpty_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new NotEmptyRule(typeof(decimal?)));

			try
			{
				validation.Validate(new SaveLotDtoTemp() { Sum = null }).ThrowIssues();
				Assert.Fail();
			}
			catch (ValidationException e)
			{
				e.Issues.Should().HaveCount(1);
				e.Issues[0].Code.Should().Be("NotEmptyRule");
				e.Issues[0].Message.Should().ContainAll("Sum", "empty");

				e.Issues[0].Description.Should().BeOfType(typeof(NotEmptyDescription));
				var description = (NotEmptyDescription)e.Issues[0].Description;
				description.Path.Should().Be(".Sum");
				description.Required.Should().BeTrue();
			}
		}
	}
}
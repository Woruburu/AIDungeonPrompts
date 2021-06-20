using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.ClearReport;
using FluentValidation.Results;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.ClearReport
{
	public class ClearReportCommandValidatorTest
	{
		private readonly ClearReportCommandValidator _validator;

		public ClearReportCommandValidatorTest()
		{
			_validator = new ClearReportCommandValidator();
		}

		[Fact]
		public async Task ValidateAsync_ReturnsNotValid_WhenIdIsDefault()
		{
			//arrange
			var query = new ClearReportCommand(default);

			//act
			ValidationResult? actual = await _validator.ValidateAsync(query);

			//assert
			Assert.False(actual.IsValid);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(64)]
		[InlineData(256)]
		public async Task ValidateAsync_ReturnsValid_WhenIdIsNotDefault(int id)
		{
			//arrange
			var query = new ClearReportCommand(id);

			//act
			ValidationResult? actual = await _validator.ValidateAsync(query);

			//assert
			Assert.True(actual.IsValid);
		}
	}
}

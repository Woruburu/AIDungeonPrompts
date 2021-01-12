using System.Collections.Generic;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetAllTags;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetAllTags
{
	public class GetAllTagsQueryHandlerTest : DatabaseFixtureTest
	{
		private readonly GetAllTagsQueryHandler _handler;

		public GetAllTagsQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new GetAllTagsQueryHandler(DbContext);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenAllTagsAreAssociatedWithDrafts()
		{
			//arrange
			var promptOne = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = new Tag
						{
							Name = "PromptTagOne"
						}
					}
				},
				IsDraft = true
			};
			var promptTwo = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = new Tag
						{
							Name = "PromptTagTwo"
						}
					}
				},
				IsDraft = true
			};
			DbContext.Prompts.AddRange(promptOne, promptTwo);
			await DbContext.SaveChangesAsync();
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenAllTagsAreNotAssociatedToAPrompt()
		{
			//arrange
			DbContext.Tags.AddRange(new Tag
			{
				Name = "TestTagOne"
			}, new Tag
			{
				Name = "TestTagTwo"
			});
			await DbContext.SaveChangesAsync();
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyList_WhenThereAreNoTagsInDatabase()
		{
			//arrange
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task Handle_ReturnsOneTag_WithACountOfTwo_WhenThereIsOneTagAssociatedWIthTwoPromptsInTheDatabase()
		{
			//arrange
			const int expectedCount = 2;
			var tag = new Tag
			{
				Name = "PromptTagOne"
			};
			var promptOne = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = tag
					}
				}
			};
			var promptTwo = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = tag
					}
				}
			};
			DbContext.Prompts.AddRange(promptOne, promptTwo);
			await DbContext.SaveChangesAsync();
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Single(actual);
			Assert.Equal(expectedCount, actual[0].Count);
		}

		[Fact]
		public async Task Handle_ReturnsTheExpectedTitle()
		{
			//arrange
			const string expectedName = "PromptTagOne";
			var promptOne = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = new Tag
						{
							Name = expectedName
						}
					}
				}
			};
			DbContext.Prompts.AddRange(promptOne);
			await DbContext.SaveChangesAsync();
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Single(actual);
			Assert.Equal(expectedName, actual[0].Name);
		}

		[Fact]
		public async Task Handle_ReturnsTwoTags_WhenThereAreTwoTagsInTheDatabase()
		{
			//arrange
			const int expectedCount = 2;
			var promptOne = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = new Tag
						{
							Name = "PromptTagOne"
						}
					}
				}
			};
			var promptTwo = new Prompt
			{
				PromptTags = new List<PromptTag>
				{
					new PromptTag
					{
						Tag = new Tag
						{
							Name = "PromptTagTwo"
						}
					}
				}
			};
			DbContext.Prompts.AddRange(promptOne, promptTwo);
			await DbContext.SaveChangesAsync();
			var query = new GetAllTagsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedCount, actual.Count);
		}
	}
}

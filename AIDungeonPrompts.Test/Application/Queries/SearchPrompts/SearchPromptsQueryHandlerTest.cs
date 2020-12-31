using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.SearchPrompts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.SearchPrompts
{
	public class SearchPromptsQueryHandlerTest : AbstractDatabaseFixtureTest
	{
		private readonly SearchPromptsQueryHandler _handler;

		public SearchPromptsQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new SearchPromptsQueryHandler(DbContext);
		}

		[Fact]
		public async Task Handle_ReturnsDateOrderedPrompts()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				PageSize = 15
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Results[0].DateCreated > actual.Results[^1].DateCreated);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyResults_AndSinglePage_WhenThereAreNoEntriesInTheDatabase()
		{
			//arrange
			var query = new SearchPromptsQuery();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual.Results);
			Assert.Equal(1, actual.TotalPages);
		}

		[Fact]
		public async Task Handle_ReturnsEmptyResults_WhenGivenAUserIdWithNoPrompts()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				UserId = 0
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual.Results);
		}

		[Fact]
		public async Task Handle_ReturnsFifteenResults_WhenThereFifteenPrompts_AndTheDefaultPageIsFifteen()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				PageSize = 15
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(15, actual.Results.Count);
			Assert.Equal(1, actual.TotalPages);
		}

		[Fact]
		public async Task Handle_ReturnsFiveResults_WhenNsfwIsEnabled_AndThereAreFiveNsfwPrompts()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				Nsfw = SearchNsfw.NsfwOnly
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(5, actual.Results.Count);
			Assert.Equal(1, actual.TotalPages);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(-10)]
		[InlineData(-256)]
		public async Task Handle_ReturnsOneResult_WhenThereAreFifteenPrompts_AndPageIsDefault_AndPageSizeIsSetToANegativeNumber(int pageSize)
		{
			//arrange
			var query = new SearchPromptsQuery
			{
				PageSize = pageSize
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Single(actual.Results);
		}

		[Fact]
		public async Task Handle_ReturnsPromptsBelongingToUser_WhenGivenAValidUserId()
		{
			//arrange
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();
			var userId = DbContext.Users.First().Id;
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				UserId = DbContext.Users.First().Id
			};

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(5, actual.Results.Count);
			Assert.True(actual.Results.All(e => e.OwnerId == userId));
		}

		[Theory]
		[InlineData("user", 5)]
		[InlineData("1", 3)]
		[InlineData("title", 15)]
		[InlineData("THIS TITLE DOES NOT EXIST", 0)]
		public async Task Handle_ReturnsPromptsWithTitleContainingSearchQuery_WhenThereArePromptsWithTheSearchQueryInTheTitle(string titleQuery, int expectedAmount)
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Search = titleQuery
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
			Assert.True(actual.Results.All(prompt => prompt.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase)));
		}

		[Theory]
		[InlineData(new[] { "tag" }, 0)]
		[InlineData(new[] { "tag1" }, 15)]
		[InlineData(new[] { "tag1", "tag2" }, 15)]
		[InlineData(new[] { "tag3", "tag5", "THIS TAG DOES NOT EXIST" }, 9)]
		[InlineData(new[] { "tag5" }, 3)]
		[InlineData(new[] { "tag", "tag1" }, 15)]
		[InlineData(new[] { "tag", "tag5" }, 3)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 15)]
		public async Task Handle_ReturnsResultsThatMatchAnyTag_WhenGivenAListOfTags_AndTagJoinIsOr(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
				TagJoin = TagJoin.Or,
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
			Assert.True(actual.Results.All(prompt => prompt.SearchPromptsTagViewModel.Any(tag => tagList.Contains(tag.Name))));
		}

		[Theory]
		[InlineData(new[] { "tag" }, 15)]
		[InlineData(new[] { "tag", "tag3" }, 15)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 15)]
		[InlineData(new[] { "tag", "THIS TAG DOES NOT EXIST" }, 15)]
		[InlineData(new[] { "THIS TAG DOES NOT EXIST" }, 0)]
		public async Task Handle_ReturnsResultsThatMatchAnyTagFuzzily_WhenGivenAListOfTags_AndTagJoinIsOr_AndTagsFuzzyIsTrue(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
				TagJoin = TagJoin.Or,
				TagsFuzzy = true
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
		}

		[Theory]
		[InlineData(new[] { "tag" }, 15)]
		[InlineData(new[] { "tag1" }, 0)]
		[InlineData(new[] { "tag1", "tag2" }, 0)]
		[InlineData(new[] { "tag3", "tag5", "THIS TAG DOES NOT EXIST" }, 6)]
		[InlineData(new[] { "tag5" }, 12)]
		[InlineData(new[] { "tag", "tag1" }, 0)]
		[InlineData(new[] { "tag", "tag5" }, 12)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 0)]
		public async Task Handle_ReturnsResultsThatMatchNoTag_WhenGivenAListOfTags_AndTagJoinIsNone(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
				TagJoin = TagJoin.None,
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
		}

		[Theory]
		[InlineData(new[] { "tag" }, 0)]
		[InlineData(new[] { "tag1" }, 0)]
		[InlineData(new[] { "tag1", "tag2" }, 0)]
		[InlineData(new[] { "tag3", "tag5", "THIS TAG DOES NOT EXIST" }, 6)]
		[InlineData(new[] { "tag5" }, 12)]
		[InlineData(new[] { "tag", "tag1" }, 0)]
		[InlineData(new[] { "tag", "tag5" }, 0)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 0)]
		[InlineData(new[] { "THIS TAG DOES NOT EXIST" }, 15)]
		public async Task Handle_ReturnsResultsThatMatchNoTagFuzzily_WhenGivenAListOfTags_AndTagJoinIsNone_AndFuzzyIsTrue(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
				TagJoin = TagJoin.None,
				TagsFuzzy = true
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
		}

		[Theory]
		[InlineData(new[] { "tag" }, 0)]
		[InlineData(new[] { "tag", "tag1" }, 0)]
		[InlineData(new[] { "tag1" }, 15)]
		[InlineData(new[] { "tag1", "tag2" }, 12)]
		[InlineData(new[] { "tag2" }, 12)]
		[InlineData(new[] { "tag1", "tag5" }, 3)]
		[InlineData(new[] { "tag5" }, 3)]
		[InlineData(new[] { "tag4" }, 6)]
		[InlineData(new[] { "tag4", "tag5" }, 3)]
		[InlineData(new[] { "tag1", "tag2", "tag3", "tag4", "tag5" }, 3)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 0)]
		public async Task Handle_ReturnsResultsThatMatchTagsExactly_WhenGivenAListOfTags(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
			Assert.True(actual.Results.All(prompt => prompt.SearchPromptsTagViewModel.Any(tag => tagList.Contains(tag.Name))));
		}

		[Theory]
		[InlineData(new[] { "tag" }, 15)]
		[InlineData(new[] { "tag1" }, 15)]
		[InlineData(new[] { "tag", "tag2" }, 12)]
		[InlineData(new[] { "tag", "tag5" }, 3)]
		[InlineData(new[] { "tag", "tag1", "tag2", "tag3", "tag4", "tag5" }, 3)]
		[InlineData(new[] { "THIS TAG DOES NOT EXIST" }, 0)]
		public async Task Handle_ReturnsResultsThatMatchTagsFuzzily_WhenGivenAListOfTags_AndFuzzyIsTrue(string[] tags, int expectedAmount)
		{
			//arrange
			var tagList = tags.ToList();
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Tags = tagList,
				TagsFuzzy = true
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(expectedAmount, actual.Results.Count);
		}

		[Fact]
		public async Task Handle_ReturnsReverseDateOrderedPrompts_WhenReverseIsSet()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				PageSize = 15,
				Reverse = true
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.True(actual.Results[^1].DateCreated > actual.Results[0].DateCreated);
		}

		[Fact]
		public async Task Handle_ReturnsTenResults_WhenNsfwIsDisabled_AndThereAreFiveNsfwPrompts()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				Nsfw = SearchNsfw.SafeOnly
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(10, actual.Results.Count);
			Assert.Equal(1, actual.TotalPages);
		}

		[Fact]
		public async Task Handle_ReturnsTwoPagesOfResult_AndFiveResults_WhenThereAreFifteenPrompts_AndPageSizeIsDefault_AndPageIsTwo()
		{
			//arrange
			var query = new SearchPromptsQuery()
			{
				Page = 2
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(5, actual.Results.Count);
			Assert.Equal(2, actual.TotalPages);
		}

		[Fact]
		public async Task Handle_ReturnsTwoPagesOfResult_AndTenResults_WhenThereAreFifteenPrompts_AndPageSizeIsDefault_AndPageIsDefault()
		{
			//arrange
			var query = new SearchPromptsQuery();
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(10, actual.Results.Count);
			Assert.Equal(2, actual.TotalPages);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(-10)]
		[InlineData(-256)]
		public async Task Handle_ReturnsTwoPagesOfResult_AndTenResults_WhenThereAreFifteenPrompts_AndPageSizeIsDefault_AndPageIsSetToANegativeNumber(int page)
		{
			//arrange
			var query = new SearchPromptsQuery
			{
				Page = page
			};
			var prompts = GeneratePromptData();
			DbContext.Prompts.AddRange(prompts);
			await DbContext.SaveChangesAsync();

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(10, actual.Results.Count);
			Assert.Equal(2, actual.TotalPages);
		}

		private List<Prompt> GeneratePromptData()
		{
			var prompts = new List<Prompt>(15);

			var tags = new Tag[]
			{
				new Tag
				{
					Name = "tag1"
				},
				new Tag
				{
					Name = "tag2"
				},
				new Tag
				{
					Name = "tag3"
				},
				new Tag
				{
					Name = "tag4"
				},
				new Tag
				{
					Name = "tag5"
				}
			};

			//basic
			for (var i = 0; i < 5; i++)
			{
				prompts.Add(new Prompt
				{
					Title = $"BasicTitle{i}",
					PromptContent = $"BasicContent{i}",
					DateCreated = DateTime.Now.AddDays(-i),
					PromptTags = tags[..(i + 1)].Select(tag => new PromptTag
					{
						Tag = tag
					}).ToList()
				});
			}

			//user
			var owner = new User
			{
				Username = $"Username"
			};
			for (var i = 0; i < 5; i++)
			{
				prompts.Add(new Prompt
				{
					Title = $"UserTitle{i}",
					PromptContent = $"UserContent{i}",
					DateCreated = DateTime.Now.AddDays(-i),
					Owner = owner,
					PromptTags = tags[..(i + 1)].Select(tag => new PromptTag
					{
						Tag = tag
					}).ToList()
				});
			}

			//nsfw
			for (var i = 0; i < 5; i++)
			{
				prompts.Add(new Prompt
				{
					Title = $"NsfwTitle{i}",
					PromptContent = $"NsfwContent{i}",
					Nsfw = true,
					DateCreated = DateTime.Now.AddDays(-i),
					PromptTags = tags[..(i + 1)].Select(tag => new PromptTag
					{
						Tag = tag
					}).ToList()
				});
			}

			return prompts;
		}
	}
}

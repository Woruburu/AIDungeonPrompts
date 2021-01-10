using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AIDungeonPrompts.Web.ModelMetadataDetailsProviders
{
	public class DoNotConvertEmptyStringToNullMetadataDetailsProvider : IDisplayMetadataProvider
	{
		public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
		{
			if (context.Key.MetadataKind == ModelMetadataKind.Property)
			{
				context.DisplayMetadata.ConvertEmptyStringToNull = false;
			}
		}
	}
}

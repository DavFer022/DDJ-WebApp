using Mapster;

namespace DDJ.Application.Common.Mappings;

/// <summary>
/// Global Mapster configuration. Add mapping rules here using
/// TypeAdapterConfig.GlobalSettings.NewConfig&lt;Source, Destination&gt;()
/// </summary>
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Auth mappings, Product mappings, etc. will be added here as features are built.
        // Example:
        // config.NewConfig<Product, ProductDto>()
        //       .Map(dest => dest.CategoryName, src => src.Category.Name);
    }
}

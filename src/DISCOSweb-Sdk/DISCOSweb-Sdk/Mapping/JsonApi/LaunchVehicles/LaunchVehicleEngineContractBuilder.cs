using DISCOSweb_Sdk.Misc;
using DISCOSweb_Sdk.Models.ResponseModels.LaunchVehicles;
using Hypermedia.Configuration;

namespace DISCOSweb_Sdk.Mapping.JsonApi.LaunchVehicles;

internal static class LaunchVehicleEngineContractBuilder
{
	internal static DelegatingContractBuilder<LaunchVehicleEngine> WithLaunchVehicleEngine(this IBuilder builder)
	{
		const string engineIdFieldName = nameof(LaunchVehicleEngine.Id);
		const string vehicleLinkTemplate = $"/api/launch-vehicles/engines/{engineIdFieldName}/vehicles";
		object IdSelector(LaunchVehicleEngine engine) => engine.Id;

		return builder.With<LaunchVehicleEngine>("engine")
			   .Id(nameof(LaunchVehicleEngine.Id))
			   .HasMany<LaunchVehicle>(AttributeUtilities.GetJsonPropertyName<LaunchVehicleEngine>(nameof(LaunchVehicleEngine.Vehicles)))
			   .Template(vehicleLinkTemplate, engineIdFieldName, IdSelector);
	}
}

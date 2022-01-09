#include <toml++/toml.h>

#include "ModConfig.h"

InternalModConfig::GlobalSettings ModConfiguration;

namespace InternalModConfig
{

GlobalSettings ParseSettings(const toml::table& Table);
AssetOverride ParseOverride(const toml::table& Table, bool Enable);

bool InitializeDefault()
{
	return LoadFromFile("mod_config.toml");
}

bool LoadFromFile(const std::string_view FilePath)
{
	// Try to parse toml data from disk
	toml::table table;

	try
	{
		table = toml::parse_file("mod_config.toml");
	}
	catch (const toml::parse_error&)
	{
		return false;
	}

	ModConfiguration = ParseSettings(table);
	return true;
}

#define PARSE_TOML_MEMBER(obj, x) o.x = obj[#x].value_or(decltype(o.x){})

GlobalSettings ParseSettings(const toml::table& Table)
{
	GlobalSettings o;

	auto& general = *Table["General"].as_table();
	PARSE_TOML_MEMBER(general, EnableDebugMenu);
	PARSE_TOML_MEMBER(general, EnableCoreLogging);
	PARSE_TOML_MEMBER(general, EnableAssetLogging);
	PARSE_TOML_MEMBER(general, EnableAssetOverrides);
	PARSE_TOML_MEMBER(general, EnableDiscordRichPresence);

	auto& gameplay = *Table["Gameplay"].as_table();
	PARSE_TOML_MEMBER(gameplay, SkipIntroLogos);
	PARSE_TOML_MEMBER(gameplay, UnlockNGPExtras);
	PARSE_TOML_MEMBER(gameplay, UnlockEntitlementExtras);

	if (o.EnableAssetOverrides)
	{
		auto enabledOverrides = Table["AssetOverrides"]["Enabled"].as_array();
		auto disabledOverrides = Table["AssetOverrides"]["Disabled"].as_array();

		if (enabledOverrides)
		{
			for (auto& entry : *enabledOverrides)
				o.AssetOverrides.emplace_back(ParseOverride(*entry.as_table(), true));
		}

		if (disabledOverrides)
		{
			for (auto& entry : *disabledOverrides)
				o.AssetOverrides.emplace_back(ParseOverride(*entry.as_table(), false));
		}
	}

	return o;
}

AssetOverride ParseOverride(const toml::table& Table, bool Enable)
{
	AssetOverride o;

	o.Enabled = Enable;
	PARSE_TOML_MEMBER(Table, ObjectUUIDs);
	PARSE_TOML_MEMBER(Table, ObjectTypes);
	PARSE_TOML_MEMBER(Table, Path);
	PARSE_TOML_MEMBER(Table, Value);

	return o;
}

#undef PARSE_TOML_MEMBER

}
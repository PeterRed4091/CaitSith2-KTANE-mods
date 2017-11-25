﻿using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;


public class ModuleSettings
{
    private const int DefaultSeed = 6502;

    public int SettingsVersion;
    public string HowToUse0 = "Don't Touch this value. It is used by the mod internally to determine if there are new settings to be saved.";

    
    public bool ResetToDefault = false;
    public string HowToReset = "Changing this setting to true will reset ALL your setting back to default.";

    //Add your own settings here.  If you wish to have explanations, define them as strings similar to as shown above.
    //Make sure those strings are JSON compliant.
    public int RuleSeed = DefaultSeed;

    public string HowToUse1 = "Sets the seed that will be used to generate the ruleset.";
    public string HowToUse2 = "1 = Vanilla";
    public string HowToUse3 = "2 = LtHummus Hack";
    public string HowToUse4 = DefaultSeed + " = VanillaRuleModifier Default Seed";
}

public class ModSettings
{
    public readonly int ModSettingsVersion = 2;
    public ModuleSettings Settings = new ModuleSettings();

    public string ModuleName { get; private set; }
    //Update this line each time you make changes to the Settings version.


    public ModSettings(KMBombModule module)
    {
        ModuleName = module.ModuleType;
    }

    public ModSettings(KMNeedyModule module)
    {
        ModuleName = module.ModuleType;
    }

    public ModSettings(string moduleName)
    {
        ModuleName = moduleName;
    }

    private string GetModSettingsPath(bool directory)
    {
        string ModSettingsDirectory = Path.Combine(Application.persistentDataPath, "Modsettings");
        return directory ? ModSettingsDirectory : Path.Combine(ModSettingsDirectory, ModuleName + "-settings.txt");
    }

    public bool WriteSettings()
    {
        Debug.LogFormat("Writing Settings File: {0}", GetModSettingsPath(false));
        try
        {
            if (!Directory.Exists(GetModSettingsPath(true)))
            {
                Directory.CreateDirectory(GetModSettingsPath(true));
            }

            Settings.SettingsVersion = ModSettingsVersion;
            string settings = JsonConvert.SerializeObject(Settings, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(GetModSettingsPath(false), settings);
            Debug.LogFormat("New settings = {0}", settings);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogFormat("Failed to Create settings file due to Exception:\n{0}\nStack Trace:\n{1}", ex.Message,
                ex.StackTrace);
            return false;
        }
    }

    public bool ReadSettings()
    {
        string ModSettings = GetModSettingsPath(false);
        try
        {
            if (File.Exists(ModSettings))
            {
                string settings = File.ReadAllText(ModSettings);
                Settings = JsonConvert.DeserializeObject<ModuleSettings>(settings, new StringEnumConverter());

                if (Settings.SettingsVersion != ModSettingsVersion)
                    return WriteSettings();
                if (!Settings.ResetToDefault) return true;
                Settings = new ModuleSettings();
                return WriteSettings();
            }
            Settings = new ModuleSettings();
            return WriteSettings();
        }
        catch (Exception ex)
        {
            Debug.LogFormat(
                "Settings not loaded due to Exception:\n{0}\nStack Trace:\n{1}\nLoading default settings instead.",
                ex.Message, ex.StackTrace);
            Settings = new ModuleSettings();
            return WriteSettings();
        }
    }
}

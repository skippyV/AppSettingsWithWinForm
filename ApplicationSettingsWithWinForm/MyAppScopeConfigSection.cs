
using System.Configuration;

namespace ApplicationSettingsWithWinForm
{
    
    public class MyAppScopeConfigSection : ConfigurationSection
    {
        public static string SkippySectionName = "SkippySection";
        public MyAppScopeConfigSection()
        {
            Name = SkippySectionName;
        }

        // [UserScopedSettingAttribute()]         // this does work but don't think it is applicable
        // [DefaultSettingValueAttribute("777")]  // this does not appear to work
        // [ApplicationScopedSettingAttribute()]  // This doesn't work for me at all

        [ConfigurationPropertyAttribute("PortAppConfig", DefaultValue = "767")] // this attribute works
        public int PortAppConfig
        {
            get
            {
                return (int)this[nameof(PortAppConfig)];
            }
            set
            {
                this[nameof(PortAppConfig)] = value;
            }
        }

        public string Name { get; }
    }
}

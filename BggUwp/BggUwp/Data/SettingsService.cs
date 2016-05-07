using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.SettingsService;

namespace BggUwp.Data
{
    public class SettingsService
    {
        public static readonly SettingsService Instance;
        static SettingsService() { Instance = Instance ?? new SettingsService(); }

        SettingsHelper _helper;
        private SettingsService() { _helper = new SettingsHelper(); }

        public bool UpdateDataOnlyOnWiFi
        {
            get { return _helper.Read(nameof(UpdateDataOnlyOnWiFi), false); }
            set { _helper.Write(nameof(UpdateDataOnlyOnWiFi), value); }
        }

        public int SavedStatusFilter
        {
            get { return _helper.Read(nameof(SavedStatusFilter), 0); }
            set { _helper.Write(nameof(SavedStatusFilter), value); }
        }

        public int SavedExpansionFilter
        {
            get { return _helper.Read(nameof(SavedExpansionFilter), 0); }
            set { _helper.Write(nameof(SavedExpansionFilter), value); }
        }

        public PlayerFilter SavedPlayerFilter
        {
            get { return _helper.Read(nameof(SavedPlayerFilter), new PlayerFilter()); }
            set { _helper.Write(nameof(SavedPlayerFilter), value); }
        }

        public PlayTimeFilter SavedPlayTimeFilter
        {
            get { return _helper.Read(nameof(SavedPlayTimeFilter), new PlayTimeFilter()); }
            set { _helper.Write(nameof(SavedPlayTimeFilter), value); }
        }
    }
}

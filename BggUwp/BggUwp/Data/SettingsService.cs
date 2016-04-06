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
    }
}

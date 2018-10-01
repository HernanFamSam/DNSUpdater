﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceProcess;
using System.Timers;
using System.Configuration;
using System.Threading.Tasks;

namespace DNSUpdater
{
    public partial class DNSUpdaterService : ServiceBase
    {
        public DNSUpdaterService()
        {
            InitializeComponent();

            ServiceName = "DNSUpdaterService";
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;

            EventLog.WriteEntry("Service Starting", EventLogEntryType.Information);
        }

        internal void TestStartAndStop(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            EncryptConfigSection("userSettings/DNSUpdaterService.Properties.DusApi");

            Timer timer = new Timer()
            {
                Interval = 10000
            };
            timer.Elapsed += new ElapsedEventHandler(OnTimer);
            timer.Start();

        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            using (var client = new GoDaddyHttpClient())
            {
                var caller = new ApiCaller<GoDaddyDomain, GoDaddyDnsRecord>();
                caller.UpdateProvider(client, EventLog);
            }

        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Stopping Service");
        }

        private static void EncryptConfigSection(string sectionKey)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection section = config.GetSection(sectionKey);
            if (section != null)
            {
                if (!section.SectionInformation.IsProtected)
                {
                    if (!section.ElementInformation.IsLocked)
                    {
                        section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                        section.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }
                }
            }

        }

        public static async Task<string> GetPublicIP()
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri("https://api.ipify.org");
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var ip = await response.Content.ReadAsStringAsync();
                    return ip;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

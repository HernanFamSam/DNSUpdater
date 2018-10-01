﻿using DNSUpdater.Base;
using DNSUpdater.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DNSUpdater
{
    public class GoDaddyHttpClient : DusHttpClient
    {
        public GoDaddyHttpClient()
        {
            Uri = new Uri(ConfigurationManager.AppSettings["URL"]);
            HeaderValue = new MediaTypeWithQualityHeaderValue(ConfigurationManager.AppSettings["RequestHeaders"]);
            SsoKey = new AuthenticationHeaderValue(
                "sso-key", $"{DusApi.Default.AccessKey}:{DusApi.Default.SecretKey}");

            BaseAddress = Uri;
            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.Accept.Add(HeaderValue);
            DefaultRequestHeaders.Authorization = SsoKey;

            DomainApiCall = $"domains/{DusApi.Default.DomainName}";
            DomainRecordApiCall = $"domains/{DusApi.Default.DomainName}/records/A/@";
            DomainUpdateDnsApiCall = $"domains/{DusApi.Default.DomainName}/records/A/@";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (obj is GoDaddyHttpClient compare &&
                Uri.OriginalString == compare.Uri.OriginalString &&
                HeaderValue.MediaType == compare.HeaderValue.MediaType &&
                SsoKey.Parameter == compare.SsoKey.Parameter)
            {
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
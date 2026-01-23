using System;
using System.Net;

namespace Menchul.Import.GeoNames.org
{
    internal class ExtendedWebClient : WebClient
    {
        public string? FileName { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            webRequest.Timeout = int.MaxValue;

            return webRequest;
        }
    }
}
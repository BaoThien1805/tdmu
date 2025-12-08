using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WEBXEMPHIMHKTMOVIE.Helpers
{
    public class PayLib
    {
        private SortedList<string, string> requestData =
            new SortedList<string, string>(new PayCompare());

        private SortedList<string, string> responseData =
            new SortedList<string, string>(new PayCompare());

        // ===== REQUEST =====
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                requestData.Add(key, value);
        }

        public string CreateRequestUrl(string baseUrl, string secretKey)
        {
            var query = new StringBuilder();

            foreach (var item in requestData)
            {
                query.Append(WebUtility.UrlEncode(item.Key) + "=" +
                             WebUtility.UrlEncode(item.Value) + "&");
            }

            query.Length -= 1;

            string sign =
                Util.HmacSHA512(secretKey, query.ToString());

            return baseUrl + "?" + query + "&vnp_SecureHash=" + sign;
        }

        // ===== RESPONSE =====
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                responseData.Add(key, value);
        }

        public string GetResponseData(string key)
        {
            return responseData.ContainsKey(key) ? responseData[key] : "";
        }

        public bool ValidateSignature(string hash, string secretKey)
        {
            if (responseData.ContainsKey("vnp_SecureHash"))
                responseData.Remove("vnp_SecureHash");
            if (responseData.ContainsKey("vnp_SecureHashType"))
                responseData.Remove("vnp_SecureHashType");

            var raw = new StringBuilder();
            foreach (var item in responseData)
            {
                raw.Append(WebUtility.UrlEncode(item.Key) + "=" +
                           WebUtility.UrlEncode(item.Value) + "&");
            }

            raw.Length -= 1;

            string myHash = Util.HmacSHA512(secretKey, raw.ToString());
            return myHash == hash;
        }
    }
}

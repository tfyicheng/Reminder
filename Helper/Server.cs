using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Reminder
{
    public static class Server
    {
        private static readonly HttpClient _client = new HttpClient();
        /// <summary>
        /// 发送 HTTP 请求
        /// </summary>
        /// <param name="method">请求方法 GET/POST</param>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求体（POST 时有效</param>
        /// <returns>响应内容</returns>
        public static async Task<string> SendAsync(
            string method,
            string url,
            string body, Dictionary<string, string> replacements = null)
        {
            try
            {

                if (replacements != null)
                {
                    // 处理占位符
                    url = ReplacePlaceholders(url, replacements);
                    body = ReplacePlaceholders(body, replacements);
                }


                using (var request = new HttpRequestMessage(
                    method.ToUpper() == "POST" ? HttpMethod.Post : HttpMethod.Get,
                    url
                ))
                {
                    // 如果是 POST，写入请求体
                    if (method.ToUpper() == "POST" && !string.IsNullOrWhiteSpace(body))
                    {
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    }
                    var response = await _client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.Growl.Warning($"请求失败: {ex.Message}");
                return $"请求失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 替换字符串中 {{key}} 格式的内容
        /// </summary>
        /// <param name="template">包含 {{}} 的模板字符串</param>
        /// <param name="getValueFunc">根据 key 获取替换值的函数</param>
        /// <returns>替换后的字符串</returns>
        private static string ReplacePlaceholders(string input, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(input) || replacements == null)
                return input;

            return Regex.Replace(input, @"\{\{(.*?)\}\}", match =>
            {
                var key = match.Groups[1].Value.Trim();
                return replacements.TryGetValue(key, out var value) ? value : match.Value;
            });
        }
    }

}

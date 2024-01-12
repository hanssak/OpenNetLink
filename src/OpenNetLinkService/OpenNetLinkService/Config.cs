using System;
using System.Text.Json;

namespace OpenNetLinkService
{
    class Config
    {
        public static Config FromJsonText(string jsonString)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<Config>(jsonString);
                return obj;
            }
            catch (System.Exception err)
            {
                throw new Exception(Environment.NewLine + "! Json Parse 실패! 설정 파일 확인이 필요합니다. ");
            }

        }
        /// <summary>
        /// 사이트명 (사이트별 Custome이 발생하기 때문에 추가 적으로 계속 갱신 될것으로 판단)
        /// </summary>
        public string SiteName { get; set; } = "KoReg";
        /// <summary>
        /// IP 주소 (주로 로컬호스트)
        /// </summary>
        public string Ip { get; set; } = "127.0.0.1";
        /// <summary>
        /// Listen하는 port 번호
        /// </summary>
        public string port { get; set; } = "9992";
    }
}

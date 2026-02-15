namespace ReadIraq.Configuration.Dto
{
    public class SmsSettingDto
    {
        //public int SmsCountRetry { get; set; }
        /// <summary>
        /// accountSid
        /// </summary>
        public string SmsUserName { get; set; }
        /// <summary>
        /// authToken
        /// </summary>
        public string SmsPassword { get; set; }
        /// <summary>
        /// Phone Number Sender
        /// </summary>
        public string ServiceAccountSID { get; set; }
        //public string SmsApiUrl { get; set; }
        //public string EnFirstPartSmsMessage { get; set; }
        //public string EnSecondPartSmsMessage { get; set; }
        //public string ArFirstPartSmsMessage { get; set; }
        //public string ArSecondPartSmsMessage { get; set; }
    }
}

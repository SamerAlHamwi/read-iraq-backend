namespace ReadIraq.Configuration.Dto
{
    public class EmailSettingDto
    {
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SenderHost { get; set; }
        public int SenderPort { get; set; }
        /// <summary>
        /// Normal Message For Verify Account
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Message For Reset Account 
        /// </summary>
        public string MessageForResetPassword { get; set; }
        public bool SenderEnableSsl { get; set; }
        public bool SenderUseDefaultCredentials { get; set; }
    }
}

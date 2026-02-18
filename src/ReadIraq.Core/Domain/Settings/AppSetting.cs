using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Settings
{
    [Table("AppSettings")]
    public class AppSetting : FullAuditedEntity<Guid>
    {
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Value, stored as JSON string.
        /// </summary>
        public string Value { get; set; }
    }
}

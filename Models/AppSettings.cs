using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Duckify.Models {
    public class AppSettings {
        /// <summary>
        /// WARNING: Obsolete. Use AllowedEmails instead.
        /// </summary>
        public string AllowedEmailsStr { get; set; }
        [NotMapped]
        public string[] AllowedEmails {
            get => AllowedEmailsStr.Split(";");
            set => AllowedEmailsStr = string.Join(";", value);
        }
        public bool SlowMode { get; set; }
        public int SlowModeLimit { get; set; }
    }
}
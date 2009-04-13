using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace EmailSender {
    /// <summary>
    /// See http://stackoverflow.com/questions/704636/sending-email-through-gmail-smtp-server-with-c
    /// </summary>
    public class GmailSender: SmtpSender {
        
        

        public GmailSender(string accountEmailAddress, string accountPassword) : base("smtp.gmail.com")
        {
            Port = 587;
            UserName = accountEmailAddress;
            Password = accountPassword;
            EnableSsl = true;
        }

        protected override void ConfigureSender(Message message) {
            if (!this.HasCredentials)
            {
                throw new Exception("Gmail Sender must have login and password");
            }

            base.ConfigureSender(message);

        }

    }
}

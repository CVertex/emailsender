using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace EmailSender.Tests {
    [TestFixture]
    [Description("Simple SMTP email sending")]
    public class SmtpSending : SmtpSendingBase {
        [Test]
        public void SimpleSendShouldSucceed() {
            
            CreateSender().Send(From, To, "TEST: simple message", "message body");

        }

        [Test]
        public void HtmlMessageSendShouldSucceed() {
            
            var msg = new Message(From, To, "TEST: html message",
                                  "<a href=\"http://emailsender.codeplex.com\">Email Sender at Codeplex</a>. That should be an anchor")
                          {Format = Format.Html};

            CreateSender().Send(msg);

        }
    }
}

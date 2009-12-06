using MbUnit.Framework;

namespace EmailSender.Tests
{
    [TestFixture]
    [Description("SMTP email sending with attachments")]
    public class SmtpSendingWithAttachments : SmtpSendingBase {
        [Test]
        public void SendWithAttachmentShouldSucceed() {
            
            var message = new Message
                              {
                                  From = From,
                                  To = To,
                                  Subject = "TEST: message with 2 attachments",
                                  Body = "message body",
                              };

            message.Attachments.Add(new MessageAttachment("first-file.png","image/png", ResourceStreamHelper.For("file.png")));
            message.Attachments.Add(new MessageAttachment("second-file.png", "image/png", ResourceStreamHelper.For("file.png")));
            CreateSender().Send(message );

        }
    }
}
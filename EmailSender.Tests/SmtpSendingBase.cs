using System.Xml.Linq;

namespace EmailSender.Tests
{
    /// <summary>
    /// 
    ///<example>
    ///<?xml version="1.0" encoding="utf-8" ?>
    ///<SetUp>
    ///    <Smtp>
    ///         <HostName>smtp.isp.com</HostName>
    ///         <Port>25</Port>
    ///         <TimeOut>3000</TimeOut>
    ///         <UserName>example_username</UserName>
    ///         <Password>example_password</Password>
    ///         <From>example@example.com</From>
    ///         <To>example@example.com</To>
    ///    </Smtp>     
    ///</SetUp> 
    ///</example> 
    /// 
    /// </summary>
    public class SmtpSendingBase: TestBase {

        #region Settings

        protected override void ParseSetUpXml(XElement element) {
            var smtp = element.Element("Smtp");

            if (smtp == null)
                throw new SetUpException("Smtp element does not exist");

            HostName = smtp.Element("HostName").Value;
            Port = smtp.Element("TimeOut").ParseInt(25);
            TimeOut = smtp.Element("Port").ParseInt(3000);
            UserName = smtp.Element("UserName").ToValueOrEmpty();
            Password = smtp.Element("Password").ToValueOrEmpty();
            
            From = smtp.Element("From").Value;
            To = smtp.Element("To").Value;
        }

        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int TimeOut { get; set; }

        #endregion


        #region Protected

        protected SmtpSender CreateSender()
        {
            return new SmtpSender(HostName) {
                Port = this.Port,
                UserName = this.UserName,
                Password = this.Password,
                Timeout = TimeOut
            };
        }

        #endregion



    }
}
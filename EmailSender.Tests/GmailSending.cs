using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace EmailSender.Tests {

    /// <summary>
    /// 
    ///<example>
    ///<?xml version="1.0" encoding="utf-8" ?>
    ///<SetUp>
    ///    <Gmail>
    ///         <Account>user_name@gmail.com</Account>
    ///         <Password>gmail_account_password</Password>
    ///         <To>some_other@example.com</To>
    ///    </Gmail>     
    ///</SetUp> 
    ///</example> 
    /// 
    /// </summary>
    [TestFixture]
    [Description("Sending mail w/ Gmail SMTP service")]
    public class GmailSending :TestBase {

        public string Account { get; set; }
        public string Password { get; set; }
        public string To { get; set; }

        protected override void ParseSetUpXml(System.Xml.Linq.XElement element) {
            var gmail = element.Element("Gmail");
            if (gmail==null)
                throw new SetUpException("Gmail element is missing");

            Account = gmail.Element("Account").Value;
            Password = gmail.Element("Password").Value;
            To = gmail.Element("To").Value;
        }


        [Test]
        public void SimpleSendShouldSucceed() {
            var g = new GmailSender(Account, Password);
            g.Send(Account,To,"TEST: gmail test message","test message body");
        }
    }
}

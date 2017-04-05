!! Project Description
{project:description}

This project was inspired by the EmailSender component in [url:Castle Components|http://www.castleproject.org/components/sitemap.html]

The library has been used in production for many months, but the level of abstraction may be unsuitable for your purposes. Please [url:let us know|http://emailsender.codeplex.com/Thread/List.aspx] how and where to abstract. Feedback is greatly appreciated.

!! Interfaces

* IEmailSender - a very simple email sending interface
* IRichMesssageEmailSender - a more complex contract that extends IEmailSender

!! Email Senders

* SmtpSender - wraps the native .NET Framework System.Net.Mail.SmtpClient
* GmailSender - extends SmtpSender for getting up and running with Gmail smtp server
* MockSender - mock sender for use within unit tests

!! Dependencies

* .NET Framework v2.0
* .NET Framework v3.5 (for tests)
* MbUnit/Gallio 3.0.6 - unit testing framework

!! [Console App]

Included is a simple app for sending mail with any of the supplied senders using the command line.

!! Running Unit Tests

Running the unit tests are good if you want to test if a system can actually send mail. 
To run the unit tests, you'll need to follow [set up instructions].

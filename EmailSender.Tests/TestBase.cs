using System;
using System.IO;
using System.Xml.Linq;
using MbUnit.Framework;

namespace EmailSender.Tests
{
    public abstract class TestBase
    {
        public XDocument Document { get; protected set; }

        public const string SetUpFileName = "SetUp.xml";
        public const string RootElementName = "SetUp";

        [Test(Order = Int32.MinValue)]
        public void SetUpXmlExists()
        {
            if (!File.Exists(SetUpFileName)) {
                throw new FileNotFoundException(
                    "You must provide an SetUp.xml file to run these unit tests");
            }
        }
        

        [SetUp]
        public void SetUp() {
           

            try
            {
                Document = XDocument.Load(SetUpFileName);
                var root = Document.Element(RootElementName);
               
                // Do validation
                if (root == null) {
                    throw new SetUpException("SetUp element could not be found from SetUp.xml");
                }
               
                // Do parsing
                ParseSetUpXml(root);

            } catch (Exception ex)
            {
                throw new SetUpException("Test settings could not be parsed from SetUp.xml",ex);
            }
        }

        protected abstract void ParseSetUpXml(XElement element);
    }
}
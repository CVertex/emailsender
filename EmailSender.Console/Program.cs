using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Mail;
using C = System.Console;
/*
 * Before deploy
 * ==============
 * clean, check-in and ignore SetUp.xml
 * 
 * 
 */ 

namespace EmailSender.Console {
    class Program {
        
        private readonly List<SenderType> _senderTypes;
        private readonly string[] _args;
        
        public Program(string[] args)
        {
            // load the IEmailSender types
            Type iEmailSenderType = typeof(IEmailSender);

            // create the new sender type list
            _senderTypes = new List<SenderType>();

            // find and save all loaded IEmailSender types
            _senderTypes =(
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetExportedTypes()
                where iEmailSenderType.IsAssignableFrom(t) && !t.IsAbstract
                select new SenderType(t)
                )
                .ToList();

            
            // save args
            _args = args;


        }

        public void Run()
        {
            if (_args.Length<2)
                throw new NoParametersException();

            string typeName = _args[0];

            var args = ConvertArgsToDictionary(_args);
            var sender = CreateSender(typeName,args);
            Send(sender, args);


        }
        

        private IEmailSender CreateSender(string typeName, IDictionary<string,string> parameters)
        {
            // match a sender type
            SenderType found = (from s in _senderTypes
                               where s.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase)
                               select s)
                               .FirstOrDefault();

            if (found == null)
                throw new SenderCreationException(String.Format("sender type {0} could not be found",typeName));

            IEmailSender instance = found.Instantiate(parameters);

            if (instance==null)
                throw new SenderCreationException(String.Format("sender type {0} could not be created", typeName));
            

            return instance;
        }

        private void Send(IEmailSender sender, IDictionary<string,string> args)
        {
            var to = GetOrThrow(args, "to");
            var from = GetOrThrow(args, "from");
            var subject = GetOrThrow(args, "subject");
            var message = GetOrThrow(args, "message");

            try
            {
                sender.Send(from, to, subject, message);
                C.WriteLine("Email sent");
            } catch (Exception ex)
            {
                throw new SenderInvocationException("Could not send message", ex);
            }

        }



        public void WriteUsageInfo()
        {
            int screenWidth = C.WindowWidth;

            var infos = _senderTypes.SelectMany(st => st.GetUsageInfos());
            var column1 = new List<string>()
                                       {
                                           "sender type"
                                       };
            column1.AddRange(from i in infos select i.Name);

            var column2 = new List<string>()
                                       {
                                           "required parameters"
                                       };
            column2.AddRange(from i in infos select String.Join(",",i.RequiredParameters));

            var column3 = new List<string>()
                                       {
                                           "optional parameters"
                                       };
            column3.AddRange(from i in infos select String.Join(",", i.OptionalParameters));


            int column1Width = column1.Max(n => n.Length) + 1;
            int column2Width = column2.Max(n => n.Length) + 1;
            int column3Width = column3.Max(n => n.Length) + 1;

            const string columnSeparator = " | ";

            int totalWidth = column1Width + column2Width + column3Width + (columnSeparator.Length*2);


            C.WriteLine("usage: EmailSender <sender type> {parameter=value}");
            C.WriteLine();
            C.WriteLine("required parameters: to, subject, from, message");
            C.WriteLine();

            


            Func<int,string> createLine = (idx) =>
            {
                var col1 = column1[idx] + CreateString(' ', column1Width - column1[idx].Length) + columnSeparator;
                var col2 = column2[idx] + CreateString(' ', column2Width - column2[idx].Length) + columnSeparator;
                var col3 = column3[idx];

                int c1Andc2Width = column1Width + column2Width + (columnSeparator.Length * 2);
                int c3Available = (C.WindowWidth - c1Andc2Width)-1;

                if (col3.Trim().Length>c3Available)
                {
                    List<string> col3Lines = new List<string>();
                    
                    int c3CurrentIdx = 0;

                    col3Lines.Add(col3.Substring(c3CurrentIdx, c3Available));
                    c3CurrentIdx += c3Available;
                    while (c3CurrentIdx<col3.Length)
                    {
                        col3Lines.Add(CreateString(' ', c1Andc2Width) + col3.Substring(c3CurrentIdx, Math.Min(c3Available,col3.Length-c3CurrentIdx)));
                        c3CurrentIdx += c3Available;
                    }
                    col3 = String.Join(Environment.NewLine, col3Lines.ToArray());
                }
                

                return col1 + col2 + col3;

            };

            

            // header
            string header = column1[0] + CreateString(' ', column1Width - column1[0].Length) + columnSeparator + 
                            column2[0] + CreateString(' ', column2Width - column2[0].Length) + columnSeparator +
                            column3[0];
            C.WriteLine(header);

            // table header separator
            C.WriteLine(CreateString('-',C.WindowWidth)); 

            // body
            for (var l = 1; l < column1.Count ;l++ )
                C.WriteLine(createLine(l));

            // end line returns
            C.WriteLine();
            C.WriteLine();
        }

        #region Utility

        static string CreateString(char c,int charCount)
        {
            if (charCount<=0)
                throw new ArgumentOutOfRangeException("charCount");

            var chars = new List<char>(charCount);
            for (var i = 0; i < charCount; i++)
                chars.Add(c);

            return new String(chars.ToArray());

        }

        static Dictionary<string, string> ConvertArgsToDictionary(string[] args) {
            var dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            for (int a = 1; a < args.Length; a++) {
                var argSplit = args[a].Split(new[] {'='},2);
                dict.Add(argSplit[0], argSplit[1]);
            }

            return dict;
        }

        static string GetOrThrow(IDictionary<string,string> dict, string key)
        {
            if (!dict.ContainsKey(key))
            {
                const string format = "\"{0}\" is a required parameter";
                throw new SenderInvocationException(String.Format(format,key));
            }

            return dict[key];
        }

        #endregion

        
        #region Entry Point

        static void WriteUsageInfo(Program p)
        {

            C.WriteLine(CreateString('=', 80));
            if (p!=null)
                p.WriteUsageInfo();
        }

        static void Main(string[] args) {

            Program p = null;

            try
            {
                p = new Program(args);
                p.Run();
                
            } catch (SenderExceptionBase se){
                C.WriteLine(se.Message);
                WriteUsageInfo(p);

            } catch (Exception ex)
            {
                C.WriteLine("There was an email sender exception:");
                C.WriteLine(ex);
                WriteUsageInfo(p);
            }
            finally
            {
                C.WriteLine("Press [enter] to exit");
                C.ReadLine();
            }
           
        }
        #endregion

    }
}

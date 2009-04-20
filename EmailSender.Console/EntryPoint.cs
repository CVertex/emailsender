using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C = System.Console;

namespace EmailSender.Console {
    public class EntryPoint {
        static void WriteUsageInfo(Program p) {
            C.WriteLine();
            C.WriteLine();
            C.WriteLine('='.Repeat(C.WindowWidth));
            C.WriteLine();
            if (p != null)
                p.WriteUsageInfo();
        }

        static void Main(string[] args) {
            

            Program p = null;

            try {
                p = new Program(args);
                p.Run();

            
            } catch (SenderExceptionBase se) {
                C.WriteLine(se.Message);
                if (se.InnerException!=null)
                {
                    C.WriteLine(se.InnerException.ToString());
                }
                WriteUsageInfo(p);

            } catch (Exception ex) {

                C.WriteLine("There was an email sender exception:");
                C.WriteLine(ex.ToString());
                WriteUsageInfo(p);
            } finally {
                C.WriteLine("Press [enter] to exit");
                C.ReadLine();
            }

        }

    }
}

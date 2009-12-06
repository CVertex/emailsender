using System.IO;
using System.Linq;
using System.Reflection;

namespace EmailSender.Tests
{
    public static class ResourceStreamHelper {
        public static Stream For(string fileName) {
            var fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var foundStreamName = (from f in fileNames
                                   where f.EndsWith(fileName)
                                   select f).FirstOrDefault();

            return Assembly.GetExecutingAssembly().GetManifestResourceStream(foundStreamName);

        }

    }
}
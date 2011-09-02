using System;
using System.IO;
using System.Reflection;

namespace SizSelCsZzz.Extras
{
    public class ResourceLoader {
        public static string LoadResourceRelativeToType(Type type, string name) {

            Assembly assembly = Assembly.GetAssembly(type);

            using(Stream resource = assembly.GetManifestResourceStream(type, name))
            using(StreamReader textStreamReader = new StreamReader(resource))
            {
                return textStreamReader.ReadToEnd();
            }
        }
    }
}
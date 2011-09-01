using System;
using System.IO;
using System.Reflection;

namespace SizSelCsZzz.Core
{
    public class ResourceLoader {
        public static string LoadResourceRelativeToType(Type type, string name) {

            Assembly assembly = Assembly.GetAssembly(type);
            //string[] names = assembly.GetManifestResourceNames();
            using(Stream resource = assembly.GetManifestResourceStream(type, name))
            using(StreamReader textStreamReader = new StreamReader(resource))
            {
                return textStreamReader.ReadToEnd();
            }
        }
    }
}
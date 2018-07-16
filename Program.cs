using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LogReader {
    class Program {
        static void Main (string[] args) {
            List<string> functions = new List<string> () { "MSGBO001", "MSGBO003" };
            string path = Directory.GetCurrentDirectory ();

            // Leer el log
            string[] lines = System.IO.File.ReadAllLines (path + "/log.txt");
            // Leer la trama XXX
            foreach (var line in lines) {
                string function = line.Substring (0, line.IndexOf ("-")).Trim ();

                if (functions.Find (match: element => element == function) != null) {
                    // Remover function de la lista
                    functions.Remove (function);

                    // Obtener la trama
                    string trama = line.Substring (line.IndexOf ("-") + 1).Trim ();

                    // Formatear la trama JSON / XML
                    if (trama.StartsWith ("<")) {
                        trama = FormatXml (trama);
                        Console.WriteLine (FormatXml (trama));
                        // Escribirla en un archivo aparte con el nombre de la trama
                        System.IO.File.WriteAllText (path: path + "/" + function + ".xml", contents: trama);
                    } else {
                        trama = FotmatJSON (trama);
                        Console.WriteLine (FotmatJSON (trama));
                        // Escribirla en un archivo aparte con el nombre de la trama
                        System.IO.File.WriteAllText (path: path + "/" + function + ".json", contents: trama);
                    }
                }
            }
        }

        static string FormatXml (string xml) {
            try {
                XDocument document = XDocument.Parse (xml);
                return document.ToString ();
            } catch (System.Exception) {
                return xml;
            }
        }

        static string FotmatJSON (string json) {
            try {
                JObject jobject = JObject.Parse (json);
                return jobject.ToString ();
            } catch (System.Exception) {
                return json;
            }
        }
    }
}
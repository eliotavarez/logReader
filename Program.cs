using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LogReader {
    class Program {
        static void Main (string[] args) {
            List<string> functions = new List<string> () { "MSGBO40001" };
            string path = Directory.GetCurrentDirectory ();
            int position = 151;

            // Leer el log
            string[] lines = System.IO.File.ReadAllLines (path + "/broker20180618_15.log");

            // Leer la trama XXX
            foreach (var line in lines) {

                if (line.IndexOf ("<FunctionName>") > 0) {
                    XDocument document = XDocument.Parse (line.Substring (position));
                    XElement element = document.Root.Element ("FunctionName");
                }

                /*string function = line.Substring (0, line.IndexOf (" - ")).Trim ();

                if (functions.Find (match: element => element == function) != null) {
                    // Remover function de la lista
                    functions.Remove (function);

                    // Obtener la trama
                    string trama = line.Substring (line.IndexOf (" - ") + 1).Trim ();

                    // Formatear la trama JSON / XML
                    if (trama.StartsWith (" < ")) {
                        trama = FormatXml (trama);
                        Console.WriteLine (FormatXml (trama));
                        // Escribirla en un archivo aparte con el nombre de la trama
                        System.IO.File.WriteAllText (path: path + " / " + function + ".xml ", contents: trama);
                    } else {
                        trama = FotmatJSON (trama);
                        Console.WriteLine (FotmatJSON (trama));
                        // Escribirla en un archivo aparte con el nombre de la trama
                        System.IO.File.WriteAllText (path: path + " / " + function + ".json ", contents : trama);
                    }
                }*/
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
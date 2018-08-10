using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LogReader;
using Newtonsoft.Json.Linq;

namespace LogReader {
    class Program {

        static List<string> links = new List<string> ();
        static string wikiPath = @"C:/wamp/www/dokuwiki/data/pages/";
        static List<BrokerFunction> brokerFuntions;

        static void Main (string[] args) {
            string brokerLogs = Directory.GetCurrentDirectory () + "/logs";
            brokerFuntions = LoadBrokerFunctions ();

            foreach (var File in System.IO.Directory.GetFiles (path: brokerLogs)) {
                if (brokerFuntions.Count == 0) break;

                // Leer el log
                string[] rows = System.IO.File.ReadAllLines (File);

                // Leer la trama XXX
                foreach (var row in rows) {
                    if (brokerFuntions.Count == 0) break;

                    if (row.IndexOf ("<FunctionName>") > 0) {
                        LoadRequest (row);
                    } else if (row.IndexOf ("MessageId") > 0) {
                        BrokerFunction brokerFunction = LoadResponse (row);
                        if (brokerFunction != null) {
                            CreateAndSaveFile (brokerFunction);
                        }
                    }
                }
            }
            SetBrokerWiki ();
        }

        private static void LoadRequest (string row) {
            string xml = row.Substring (row.IndexOf ('<'));
            string correlationId = row.Substring (51, 97).Split (':') [0];

            try {
                // Verificando si no es un request SOAP
                if (xml.IndexOf ("ns:") < 0) {
                    XDocument document = XDocument.Parse (xml);
                    string functionName = (string) (from el in document.Descendants ("FunctionName") select el).First ();

                    // Verificar si la funcion esta en el listado                    
                    BrokerFunction brokerFunction = brokerFuntions.Find (x => x.Name.Equals (functionName));
                    if (brokerFunction != null) {
                        brokerFunction.CorrelationId = correlationId;
                        brokerFunction.Request = document;
                    }
                }
            } catch (System.Exception ex) {
                Console.WriteLine (ex.Message);
            }
        }

        private static BrokerFunction LoadResponse (string row) {
            string xml = row.Substring (151);
            string[] correlationId = row.Substring (51, 97).Split (':');

            BrokerFunction brokerFunction = brokerFuntions.Find (x => x.CorrelationId.Equals (correlationId[0]) ||
                x.CorrelationId.Equals (correlationId[1]));

            if (brokerFunction != null) {
                XDocument document = XDocument.Parse (xml);
                brokerFunction.Response = document;
            }

            return brokerFunction;
        }

        private static void CreateAndSaveFile (BrokerFunction brokerFunction) {
            // Plantilla
            string dokufile = System.IO.File.ReadAllText (Directory.GetCurrentDirectory () + "/broker_function_markdown.txt");
            dokufile = dokufile.Replace ("{Name}", brokerFunction.Name);
            dokufile = dokufile.Replace ("{Description}", brokerFunction.Description);
            dokufile = dokufile.Replace ("{Request}", brokerFunction.Request.ToString ());
            dokufile = dokufile.Replace ("{Response}", brokerFunction.Response.ToString ());

            string path = wikiPath + brokerFunction.FileName + ".txt";
            //string path = Directory.GetCurrentDirectory () + "/tramas/" + brokerFunction.FileName;
            System.IO.File.WriteAllText (path: path, contents: dokufile);

            links.Add (string.Format ("[[{0}|{1}]]", brokerFunction.FileName, brokerFunction.Alias));
            brokerFuntions.Remove (brokerFunction);
        }

        private static void SetBrokerWiki () {
            // Dokuwiki - Link
            string main = System.IO.File.ReadAllText (Directory.GetCurrentDirectory () + "/broker_main_markdown.txt");

            List<string> actualLinks = new List<string> ();
            foreach (var row in System.IO.File.ReadAllLines (wikiPath + "/start.txt")) {
                if (row.Contains ("*")) {
                    actualLinks.Add (row.Substring (6));
                }
            }

            main = main.Replace ("{Personal}", string.Join ("\n    * ", links.Union (actualLinks)));
            System.IO.File.WriteAllText (path: wikiPath + "/start.txt", contents: main);

            Console.WriteLine ("Funciones no encontradas:");
            brokerFuntions.ForEach (function => Console.WriteLine ($"* {function.Name}"));
        }

        // Cargar funciones de Broker que se van a documentar
        static List<BrokerFunction> LoadBrokerFunctions () {
            string path = Directory.GetCurrentDirectory () + "/brokerFunctions.txt";

            Console.WriteLine ("Funciones de Broker a documentar, desde la ruta: " + path);
            string[] rows = System.IO.File.ReadAllLines (path: path);

            List<BrokerFunction> brokerFunctions = new List<BrokerFunction> ();

            foreach (var row in rows) {
                if (row.ElementAt (0) == '/') continue;

                string[] temp = row.Split (';');

                BrokerFunction funcion = new BrokerFunction () {
                    Name = temp[0],
                    Description = temp[1],
                    Alias = temp[2]
                };

                brokerFunctions.Add (funcion);
                Console.WriteLine ($"* {temp[0]}");
            }

            return brokerFunctions;
        }
    }
}
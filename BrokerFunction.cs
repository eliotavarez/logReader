using System.Xml.Linq;

namespace LogReader {
    public class BrokerFunction {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public string CorrelationId { get; set; } = "";
        public XDocument Request { get; set; }
        public XDocument Response { get; set; }

        public string FileName => "broker_" + this.Name.ToLower ();
    }
}
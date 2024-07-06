using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PluginsSystem.Models
{
#nullable disable

    [XmlRoot("Parameter")]
    public class ParameterInfo
    {
        [XmlAttribute(nameof(Title))]
        public string Title { get; set; }

		[XmlAttribute(nameof(Type))]
		public string Type { get; set; }

		[XmlAttribute(nameof(ConfigKey))]
		public string ConfigKey { get; set; }

		[XmlAttribute(nameof(DefaultValue))]
		public string DefaultValue { get; set; }
    }
}

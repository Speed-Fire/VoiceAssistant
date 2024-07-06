using PluginsSystem.Entities;
using PluginsSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PluginsSystem.Settings.Initialization
{
    public sealed class PluginSettingsReader
    {
        private readonly XmlSchemaSet _settingsSchema;

        public PluginSettingsReader()
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var assemblyName = assembly.GetName().

            //if (assemblyName is null)
            //    throw new InvalidOperationException("Can't get name of executing assembly!");

            using var schemaStream = assembly
                .GetManifestResourceStream($"PluginsSystem.Settings.Initialization.SettingsSchema.xsd");

            if (schemaStream is null)
                throw new InvalidOperationException("Can't get SettingsSchema.xsd!");

            _settingsSchema = new();
            using var reader = XmlReader.Create(schemaStream);

            _settingsSchema.Add(null, reader);
        }

        public async Task<List<ParameterInfoEntity>> ReadParameters(PluginInfo pluginInfo)
        {
            var result = new List<ParameterInfoEntity>();

            if (pluginInfo.SettingsStream is null)
                return result;

            XDocument? document;

            try
            {
                document = await XDocument.LoadAsync(pluginInfo.SettingsStream,
                    LoadOptions.None, default);
            }
            catch
            {
                return result;
            }
            
            if (document is null)
                return result;

            if (!ValidateXml(document))
                return result;

            var serializer = new XmlSerializer(typeof(Models.ParameterInfo));

            var descendents = document.Elements().First().Elements();

			foreach (var xelement in descendents)
            {
                using var reader = xelement.CreateReader();

                var parameterInfo = (Models.ParameterInfo?)serializer.Deserialize(reader);

                if (parameterInfo is null)
                    continue;

                result.Add(new(parameterInfo, pluginInfo));
            }

            return result;
        }

        private bool ValidateXml(XDocument document)
        {
            bool result = true;

            document.Validate(_settingsSchema, (sender, e) =>
            {
                result = false;
            });

            return result;
        }
    }
}

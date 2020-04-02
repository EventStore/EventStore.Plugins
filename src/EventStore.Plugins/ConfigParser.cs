using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace EventStore.Plugins
{
    public static class ConfigParser
    {
        public static T ReadConfiguration<T>(string fileName, string sectionName) where T : class
        {
            try
            {
                var yamlStream = new YamlStream();
                var stringReader = new StringReader(File.ReadAllText(fileName));
                try {
                    yamlStream.Load(stringReader);
                } catch (Exception ex) {
                    throw new Exception(
                        $"An invalid configuration file has been specified. {Environment.NewLine}{ex.Message}");
                }

                var yamlNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
                if (!String.IsNullOrEmpty(sectionName)) {
                    Func<KeyValuePair<YamlNode, YamlNode>, bool> predicate = x =>
                        x.Key.ToString() == sectionName && x.Value is YamlMappingNode;

                    var nodeExists = yamlNode.Children.Any(predicate);
                    if (nodeExists) {
                        yamlNode = (YamlMappingNode)yamlNode.Children.First(predicate).Value;
                    }
                }

                if (yamlNode == null)
                {
                    return default;
                }

                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                using (var reader = new StreamReader(stream))
                {
                    new YamlStream(new YamlDocument[] { new YamlDocument(yamlNode) }).Save(writer);
                    writer.Flush();
                    stream.Position = 0;
                    return new Deserializer().Deserialize<T>(reader);
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex, "Cannot find the specified config file {0}.", fileName);
                throw;
            }
        }
    }
}
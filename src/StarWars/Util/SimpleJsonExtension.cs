using SimpleJson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace adenin.Core
{
    public static class SimpleJsonExtension
    {
        private const string INDENT_STRING = "  ";

        public static JsonObject Merge(JsonObject srcObject, JsonObject mergeObject)
        {
            return Merge(srcObject, mergeObject, new List<KeyValuePair<string, string>>() { });
        }


        public static JsonObject Merge(JsonObject srcObject, JsonObject mergeObject, List<KeyValuePair<string, string>> keyProperties)
        {
            JsonObject result = srcObject;
            foreach (string prop in mergeObject.GetDynamicMemberNames())
            {
                if (result[prop] != null)
                {
                    if (mergeObject[prop] is JsonArray)
                    {
                        if ((mergeObject[prop] as JsonArray).Count > 0)
                        {
                            srcObject[prop] = Merge((srcObject[prop] as JsonArray), (mergeObject[prop] as JsonArray), prop, keyProperties);
                        }
                    }
                    else if (mergeObject[prop] is JsonObject)
                    {
                        if (result[prop] is JsonObject)
                        {
                            result[prop] = Merge(result[prop] as JsonObject, mergeObject[prop] as JsonObject, keyProperties);
                        }
                        else
                        {
                            result[prop] = mergeObject[prop];
                        }
                    }
                    else
                    {
                        result[prop] = mergeObject[prop];
                    }
                }
                else
                {
                    result.Add(prop, mergeObject[prop]);
                }
            }

            return result;
        }

        public static JsonArray Merge(JsonArray srcArray, JsonArray mergeArray, string keyProperty, List<KeyValuePair<string, string>> keyProperties)
        {
            JsonArray result = srcArray;
            if (srcArray == null || srcArray.Count <= 0)
            {
                return mergeArray;
            }
            else
            {
                if (mergeArray.Count > 0)
                {
                    var prop = keyProperties.FirstOrDefault(item => string.Equals(item.Key, keyProperty)).Value;

                    foreach (var item in mergeArray)
                    {
                        if (item is JsonObject)
                        {
                            if (!string.IsNullOrWhiteSpace(prop))
                            {
                                string propValue = ((item as JsonObject)[prop]).ToString();
                                if (!string.IsNullOrWhiteSpace(propValue))
                                {
                                    var resultItem = result.Find(x => string.Equals((x as JsonObject)[prop], propValue));
                                    if (resultItem != null)
                                    {
                                        resultItem = Merge((resultItem as JsonObject), (item as JsonObject), keyProperties);
                                        continue;
                                    }
                                }
                            }

                            result.Add(item);
                        }
                        else if (!result.Contains(item))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public static dynamic ConvertToJson(DataTable dtObject)
        {
            var result = new SimpleJson.JsonArray();

            foreach (DataRow row in dtObject.Rows)
            {
                dynamic record = new SimpleJson.JsonObject();
                foreach (DataColumn col in dtObject.Columns)
                {
                    record[col.ColumnName] = row[col.ColumnName];

                    // if date returned from database is kind unknown then we assume its UTC
                    if (col.DataType.FullName == "System.DateTime")
                    {
                        DateTime dt = (DateTime)row[col.ColumnName];
                        if (dt.Kind == DateTimeKind.Unspecified) record[col.ColumnName] = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
                result.Add(record);
            }
            return result;

        }


        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;

                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;

                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        /// <summary>
        /// Serializes a SimpleJson.JsonObject to yaml string
        /// </summary>
        /// <param name="simpleJsonObject"></param>
        /// <returns></returns>
        public static string SerializeToYaml(dynamic simpleJsonObject)
        {
            var yamlSerializerSettings = new SharpYaml.Serialization.SerializerSettings()
            {
                ComparerForKeySorting = null,
                EmitAlias = false,
                EmitTags = false
            };

            var yamlSerializer = new SharpYaml.Serialization.Serializer(yamlSerializerSettings);
            string yamlStringContent = yamlSerializer.Serialize(simpleJsonObject);

            return yamlStringContent;
        }

        public static bool IsJsonString(string stringContent)
        {
            bool isJsonString = false;
            bool firstNonWhitespace = false;
            int strLength = stringContent.Length;
            for (int i = 0; i < strLength && !firstNonWhitespace; i++)
            {
                char currentChar = stringContent[i];
                firstNonWhitespace = !Char.IsWhiteSpace(currentChar);
                isJsonString = firstNonWhitespace && currentChar == '{';
            }
            return isJsonString;
        }

        /// <summary>
        /// Deserializes yaml string or json string to JsonObject
        /// </summary>
        /// <param name="yamlStringContent"></param>
        /// <returns></returns>
        public static dynamic DeserializeFromYaml(string yamlStringContent)
        {
            // return empty JSON object for empty string
            if (string.IsNullOrWhiteSpace(yamlStringContent))
            {
                return new SimpleJson.JsonObject();
            }

            // detect if yamlStringContent is json string
            if (IsJsonString(yamlStringContent))
            {
                var result = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(yamlStringContent);
                return result;
            }

            #region parse YAML text

            var textReader = new System.IO.StringReader(yamlStringContent);
            var yamlParser = new SharpYaml.Parser(textReader);

            var yamlEventReader = new SharpYaml.EventReader(yamlParser);

            var eventStack = new Stack<SharpYaml.Events.ParsingEvent>();
            SharpYaml.Events.ParsingEvent eventFromStack;

            var yamlEvent = yamlEventReader.Allow<SharpYaml.Events.ParsingEvent>();

            var objectStack = new Stack<object>();

            object current = null;
            object previousCurrent = null;

            while (yamlEvent != null)
            {
                switch (yamlEvent.Type)
                {
                    case SharpYaml.Events.EventType.YAML_NO_EVENT:
                        break;

                    case SharpYaml.Events.EventType.YAML_STREAM_START_EVENT:
                        eventStack.Push(yamlEvent);
                        break;

                    case SharpYaml.Events.EventType.YAML_STREAM_END_EVENT:
                        eventFromStack = eventStack.Pop();
                        if (eventFromStack.Type != SharpYaml.Events.EventType.YAML_STREAM_START_EVENT)
                        {
                            throw new Exception("StreamEnd does not have matching stream start event");
                        }
                        break;

                    case SharpYaml.Events.EventType.YAML_DOCUMENT_START_EVENT:
                        eventStack.Push(yamlEvent);
                        break;

                    case SharpYaml.Events.EventType.YAML_DOCUMENT_END_EVENT:
                        eventFromStack = eventStack.Pop();
                        if (eventFromStack.Type != SharpYaml.Events.EventType.YAML_DOCUMENT_START_EVENT)
                        {
                            throw new Exception("Document End does not have matching Document Start event");
                        }
                        break;

                    case SharpYaml.Events.EventType.YAML_ALIAS_EVENT:
                        break;

                    case SharpYaml.Events.EventType.YAML_SCALAR_EVENT:

                        var scalarEvent = yamlEvent as SharpYaml.Events.Scalar;
                        if (current is JsonObject)
                        {
                            var currentObject = current as JsonObject;
                            var nextEvent = yamlEventReader.Peek<SharpYaml.Events.ParsingEvent>();
                            if (nextEvent.Type == SharpYaml.Events.EventType.YAML_SCALAR_EVENT)
                            {
                                nextEvent = yamlEventReader.Allow<SharpYaml.Events.ParsingEvent>();
                                var nextScalarEvent = nextEvent as SharpYaml.Events.Scalar;
                                if (nextScalarEvent.Tag != null)
                                {
                                    if (nextScalarEvent.Tag.Contains("int"))
                                    {
                                        int intValue = 0;
                                        Int32.TryParse(nextScalarEvent.Value, out intValue);
                                        currentObject.Add(scalarEvent.Value, intValue);
                                    }
                                    else if (nextScalarEvent.Tag.Contains("bool"))
                                    {
                                        bool boolValue = false;
                                        Boolean.TryParse(nextScalarEvent.Value, out boolValue);
                                        currentObject.Add(scalarEvent.Value, boolValue);
                                    }
                                    else if (nextScalarEvent.Tag.Contains("float"))
                                    {
                                        double floatValue = double.Parse(nextScalarEvent.Value, System.Globalization.CultureInfo.InvariantCulture);
                                        currentObject.Add(scalarEvent.Value, floatValue);
                                    }
                                    else
                                    {
                                        if (nextScalarEvent.Value == "null")
                                        {
                                            currentObject.Add(scalarEvent.Value, null);
                                        }
                                        else
                                        {
                                            currentObject.Add(scalarEvent.Value, nextScalarEvent.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (nextScalarEvent.Value == "null")
                                    {
                                        currentObject.Add(scalarEvent.Value, null);
                                    }
                                    else
                                    {
                                        currentObject.Add(scalarEvent.Value, nextScalarEvent.Value);
                                    }
                                }
                            }
                            else
                            {
                                // property is either mapping or sequence, push event on stack
                                eventStack.Push(yamlEvent);
                            }
                        }
                        else if (current is JsonArray)
                        {
                            var currentArray = current as JsonArray;
                            currentArray.Add(scalarEvent.Value);
                        }

                        break;

                    case SharpYaml.Events.EventType.YAML_SEQUENCE_START_EVENT:
                        eventStack.Push(yamlEvent);

                        objectStack.Push(current);
                        current = new JsonArray();

                        break;

                    case SharpYaml.Events.EventType.YAML_SEQUENCE_END_EVENT:
                        eventFromStack = eventStack.Pop();
                        if (eventFromStack.Type != SharpYaml.Events.EventType.YAML_SEQUENCE_START_EVENT)
                        {
                            throw new Exception("Sequence end does not have matching Sequence Start event");
                        }

                        if (objectStack.Count == 0)
                        {
                            break;
                        }

                        eventFromStack = eventStack.Peek();
                        previousCurrent = objectStack.Pop();
                        if (eventFromStack.Type == SharpYaml.Events.EventType.YAML_SCALAR_EVENT)
                        {
                            var scalarFromStack = eventStack.Pop() as SharpYaml.Events.Scalar;

                            if (previousCurrent is JsonObject)
                            {
                                var previousObject = previousCurrent as JsonObject;
                                previousObject.Add(scalarFromStack.Value, current);
                            }
                            else if (previousCurrent is JsonArray)
                            {
                                var previousArray = previousCurrent as JsonArray;
                                previousArray.Add(scalarFromStack.Value);
                            }
                        }
                        current = previousCurrent;
                        break;

                    case SharpYaml.Events.EventType.YAML_MAPPING_START_EVENT:
                        eventStack.Push(yamlEvent);
                        if (current != null)
                        {
                            objectStack.Push(current);
                            current = new JsonObject();
                        }
                        else
                        {
                            current = new JsonObject();
                        }
                        break;

                    case SharpYaml.Events.EventType.YAML_MAPPING_END_EVENT:
                        eventFromStack = eventStack.Pop();
                        if (eventFromStack.Type != SharpYaml.Events.EventType.YAML_MAPPING_START_EVENT)
                        {
                            throw new Exception("Document End does not have matching Document Start event");
                        }

                        if (objectStack.Count == 0)
                        {
                            break;
                        }

                        eventFromStack = eventStack.Peek();
                        previousCurrent = objectStack.Pop();

                        if (eventFromStack.Type == SharpYaml.Events.EventType.YAML_SCALAR_EVENT)
                        {
                            var scalarFromStack = eventStack.Pop() as SharpYaml.Events.Scalar;
                            if (previousCurrent is JsonObject)
                            {
                                var previousObject = previousCurrent as JsonObject;
                                previousObject.Add(scalarFromStack.Value, current);
                            }
                            else if (previousCurrent is SimpleJson.JsonArray)
                            {
                                var previousArray = previousCurrent as SimpleJson.JsonArray;
                                previousArray.Add(scalarFromStack.Value);
                            }
                        }
                        // this else if branch allows for parsing of array of objects
                        else if (eventFromStack.Type == SharpYaml.Events.EventType.YAML_SEQUENCE_START_EVENT)
                        {
                            var previousArray = previousCurrent as JsonArray;
                            previousArray.Add(current);
                        }

                        current = previousCurrent;
                        break;

                    default:
                        break;
                }

                // Note 1. we use yamlEvent == null to exit the while loop
                // Note 2. SharpYaml doens't have a convenient way for programmer to test for end of parsing
                // ideally yamlEventReader.isEnd() should exist
                // Instead isEnd() can be emulated with yamlEventReader.Parser.Current != null; if Current is null we are at the end of parsing
                yamlEvent = yamlEventReader.Parser.Current != null ? yamlEventReader.Allow<SharpYaml.Events.ParsingEvent>() : null;
            }

            textReader.Dispose();

            return current;

            #endregion
        }

        /// <summary>
        /// Converts jsonString to yamlString
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static string ConvertJsonToYaml(string jsonString)
        {
            if (IsJsonString(jsonString))
            {
                var jsonObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(jsonString);
                var yamlString = SimpleJsonExtension.SerializeToYaml(jsonObject);
                return yamlString;
            }
            return jsonString;
        }

        /// <summary>
        /// Converts yamlString to jsonString
        /// </summary>
        /// <param name="yamlString"></param>
        /// <returns></returns>
        public static string ConvertYamlToJson(string yamlString)
        {
            // return empty JSON object for empty string
            if (string.IsNullOrWhiteSpace(yamlString))
            {
                return "{}";
            }
            var jsonObject = SimpleJsonExtension.DeserializeFromYaml(yamlString);
            var jsonString = SimpleJson.SimpleJson.SerializeObject(jsonObject);
            return jsonString;
        }
    }
}


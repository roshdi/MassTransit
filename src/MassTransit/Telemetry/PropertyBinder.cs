// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Templates;


    class PropertyBinder
    {
        static readonly TelemetryLogEventProperty[] NoProperties = new TelemetryLogEventProperty[0];
        readonly PropertyValueConverter _valueConverter;

        public PropertyBinder(PropertyValueConverter valueConverter)
        {
            _valueConverter = valueConverter;
        }

        /// <summary>
        /// Create properties based on an ordered list of provided values.
        /// </summary>
        /// <param name="messageTemplate">The template that the parameters apply to.</param>
        /// <param name="messageTemplateParameters">Objects corresponding to the properties
        /// represented in the message template.</param>
        /// <returns>A list of properties; if the template is malformed then
        /// this will be empty.</returns>
        public IEnumerable<TelemetryLogEventProperty> ConstructProperties(MessageTemplate messageTemplate, object[] messageTemplateParameters)
        {
            if (messageTemplateParameters == null || messageTemplateParameters.Length == 0)
            {
                if (messageTemplate.NamedProperties != null || messageTemplate.PositionalProperties != null)
                    TelemetryContext.CurrentOrDefault.Warning("Required properties not provided for: {0}", messageTemplate);

                return NoProperties;
            }

            if (messageTemplate.PositionalProperties != null)
                return ConstructPositionalProperties(messageTemplate, messageTemplateParameters);

            return ConstructNamedProperties(messageTemplate, messageTemplateParameters);
        }

        IEnumerable<TelemetryLogEventProperty> ConstructPositionalProperties(MessageTemplate template, object[] messageTemplateParameters)
        {
            var positionalProperties = template.PositionalProperties;

            if (positionalProperties.Length != messageTemplateParameters.Length)
                TelemetryContext.CurrentOrDefault.Warning("Positional property count does not match parameter count: {0}", template);

            var result = new TelemetryLogEventProperty[messageTemplateParameters.Length];
            foreach (var property in positionalProperties)
            {
                int position;
                if (property.TryGetPositionalValue(out position))
                {
                    if (position < 0 || position >= messageTemplateParameters.Length)
                        TelemetryContext.CurrentOrDefault.Warning("Unassigned positional value {0} in: {1}", position, template);
                    else
                        result[position] = ConstructProperty(property, messageTemplateParameters[position]);
                }
            }

            var next = 0;
            for (var i = 0; i < result.Length; ++i)
            {
                if (result[i] != null)
                {
                    result[next] = result[i];
                    ++next;
                }
            }

            if (next != result.Length)
                Array.Resize(ref result, next);

            return result;
        }

        IEnumerable<TelemetryLogEventProperty> ConstructNamedProperties(MessageTemplate template, object[] messageTemplateParameters)
        {
            var namedProperties = template.NamedProperties;
            if (namedProperties == null)
                return Enumerable.Empty<TelemetryLogEventProperty>();

            var matchedRun = namedProperties.Length;
            if (namedProperties.Length != messageTemplateParameters.Length)
            {
                matchedRun = Math.Min(namedProperties.Length, messageTemplateParameters.Length);
                TelemetryContext.CurrentOrDefault.Warning("Named property count does not match parameter count: {0}", template);
            }

            var result = new TelemetryLogEventProperty[matchedRun];
            for (var i = 0; i < matchedRun; ++i)
            {
                var property = template.NamedProperties[i];
                var value = messageTemplateParameters[i];
                result[i] = ConstructProperty(property, value);
            }

            return result;
        }

        TelemetryLogEventProperty ConstructProperty(PropertyToken propertyToken, object value)
        {
            return new TelemetryLogEventProperty(
                propertyToken.PropertyName,
                _valueConverter.CreateValue(value, propertyToken.Destructuring));
        }
    }
}
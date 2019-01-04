﻿#region License
// The MIT License (MIT)
// 
// Copyright (c) 2016 João Simões
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
namespace SimpleSOAPClient.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Exceptions;
    using Models;

    /// <summary>
    /// Helper methods for working with <see cref="SoapEnvelopeOld"/> instances.
    /// </summary>
    public static class EnvelopeHelpers
    {
        private static readonly XName SoapFaultXName =
            XName.Get("Fault", Constant.Namespace.OrgXmlSoapSchemasSoapEnvelope);

        #region Body

        /// <summary>
        /// Sets the given <see cref="XElement"/> as the envelope body.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to be used.</param>
        /// <param name="body">The <see cref="XElement"/> to set as the body.</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld Body(this SoapEnvelopeOld envelope, XElement body)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            if (envelope.Body == null)
                envelope.Body = new SoapEnvelopeBodyOld();

            envelope.Body.Value = body;

            return envelope;
        }

        /// <summary>
        /// Sets the given entity as the envelope body.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to be used.</param>
        /// <param name="body">The entity to set as the body.</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld Body<T>(this SoapEnvelopeOld envelope, T body)
        {
            return envelope.Body(body.ToXElement());
        }

        /// <summary>
        /// Extracts the <see cref="SoapEnvelopeOld.Body"/> as an object of the given type.
        /// </summary>
        /// <typeparam name="T">The type do be deserialized.</typeparam>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/></param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FaultException">Thrown if the body contains a fault</exception>
        public static T Body<T>(this SoapEnvelopeOld envelope)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            envelope.ThrowIfFaulted();

            return envelope.Body.Value.ToObject<T>();
        }

        #endregion

        #region Headers

        /// <summary>
        /// Appends the received <see cref="XElement"/> collection to the existing
        /// ones in the received <see cref="SoapEnvelopeOld"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to append the headers</param>
        /// <param name="headers">The <see cref="SoapHeaderOld"/> collection to append</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld WithHeaders(
            this SoapEnvelopeOld envelope, params XElement[] headers)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            if (headers.Length == 0) return envelope;

            if (envelope.Header == null)
            {
                envelope.Header = new SoapEnvelopeHeaderOld
                {
                    Headers = headers
                };
            }
            else
            {
                var envelopeHeaders = new List<XElement>(envelope.Header.Headers);
                envelopeHeaders.AddRange(headers);
                envelope.Header.Headers = envelopeHeaders.ToArray();
            }

            return envelope;
        }

        /// <summary>
        /// Appends the received <see cref="XElement"/> collection to the existing
        /// ones in the received <see cref="SoapEnvelopeOld"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to append the headers</param>
        /// <param name="headers">The <see cref="SoapHeaderOld"/> collection to append</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld WithHeaders(
            this SoapEnvelopeOld envelope, IEnumerable<XElement> headers)
        {
            return envelope.WithHeaders(headers.ToArray());
        }

        /// <summary>
        /// Appends the received <see cref="SoapHeaderOld"/> collection to the existing
        /// ones in the received <see cref="SoapEnvelopeOld"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to append the headers</param>
        /// <param name="headers">The <see cref="SoapHeaderOld"/> collection to append</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld WithHeaders(
            this SoapEnvelopeOld envelope, params SoapHeaderOld[] headers)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            if (headers.Length == 0) return envelope;

            var xElementHeaders = new XElement[headers.Length];
            for (var i = 0; i < headers.Length; i++)
                xElementHeaders[i] = headers[i].ToXElement();

            return envelope.WithHeaders(xElementHeaders);
        }

        /// <summary>
        /// Appends the received <see cref="SoapHeaderOld"/> collection to the existing
        /// ones in the received <see cref="SoapEnvelopeOld"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to append the headers</param>
        /// <param name="headers">The <see cref="SoapHeaderOld"/> collection to append</param>
        /// <returns>The <see cref="SoapEnvelopeOld"/> after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapEnvelopeOld WithHeaders(
            this SoapEnvelopeOld envelope, IEnumerable<SoapHeaderOld> headers)
        {
            return envelope.WithHeaders(headers.ToArray());
        }

        /// <summary>
        /// Gets a given <see cref="XElement"/> by its <see cref="XName"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> with the headers.</param>
        /// <param name="name">The <see cref="XName"/> to search.</param>
        /// <returns>The <see cref="XElement"/> or null if not match is found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static XElement Header(this SoapEnvelopeOld envelope, XName name)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            if (envelope.Header == null || envelope.Header.Headers.Length == 0)
                return null;

            return envelope.Header.Headers.FirstOrDefault(xElement => xElement.Name == name);
        }

        /// <summary>
        /// Gets a given <see cref="SoapHeaderOld"/> by its <see cref="XName"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> with the headers.</param>
        /// <param name="name">The <see cref="XName"/> to search.</param>
        /// <returns>The <see cref="SoapHeaderOld"/> or null if not match is found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T Header<T>(this SoapEnvelopeOld envelope, XName name)
            where T: SoapHeaderOld
        {
            return envelope.Header(name).ToObject<T>();
        }

        #endregion

        #region Faulted

        /// <summary>
        /// Does the <see cref="SoapEnvelopeOld.Body"/> contains a fault?
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to validate</param>
        /// <returns>True if a fault exists</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsFaulted(this SoapEnvelopeOld envelope)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            return envelope.Body?.Value != null && envelope.Body.Value.Name == SoapFaultXName;
        }

        /// <summary>
        /// Checks if the <see cref="SoapEnvelopeOld.Body"/> contains a fault 
        /// and throws an <see cref="FaultException"/> if true.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to validate.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FaultException">Thrown if the body contains a fault</exception>
        public static void ThrowIfFaulted(this SoapEnvelopeOld envelope)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            if (!envelope.IsFaulted()) return;

            var fault = envelope.Fault();
            throw new FaultException
            {
                Code = fault.Code,
                String = fault.String,
                Actor = fault.Actor,
                Detail = fault.Detail
            };
        }

        /// <summary>
        /// Extracts the <see cref="SoapEnvelopeOld.Body"/> as a <see cref="SoapFaultOld"/>.
        /// It will fail to deserialize if the body is not a fault. Consider to
        /// use <see cref="IsFaulted"/> first.
        /// </summary>
        /// <param name="envelope">The <see cref="SoapEnvelopeOld"/> to be used</param>
        /// <returns>The <see cref="SoapFaultOld"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SoapFaultOld Fault(this SoapEnvelopeOld envelope)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));

            return envelope.Body?.Value.ToObject<SoapFaultOld>();
        }

        #endregion
    }
}

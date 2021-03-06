﻿/*
 * NanoXLSX is a small .NET library to generate and read XLSX (Microsoft Excel 2007 or newer) files in an easy and native way
 * Copyright Raphael Stoeckli © 2018
 * This library is licensed under the MIT License.
 * You find a copy of the license in project folder or on: http://opensource.org/licenses/MIT
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IOException = NanoXLSX.Exception.IOException;

namespace NanoXLSX.LowLevel
{
    /// <summary>
    /// Class representing a reader for the shared strings table of XLSX files
    /// </summary>
    public class SharedStringsReader
    {

        #region properties

        /// <summary>
        /// List of shared string entries
        /// </summary>
        /// <value>
        /// String entry, sorted by its internal index of the table
        /// </value>
        public List<string> SharedStrings { get; private set; }

        /// <summary>
        /// Gets whether the workbook contains shared strings
        /// </summary>
        /// <value>
        ///   True if at least one shared string object exists in the workbook
        /// </value>
        public bool HasElements
        {
            get
            {
                if (SharedStrings.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the value of the shared string table by its index
        /// </summary>
        /// <param name="index">Index of the stared string entry</param>
        /// <returns>Determined shared string value. Returns null in case of a invalid index</returns>
        public string GetString(int index)
        {
            if (HasElements == false || index > SharedStrings.Count - 1)
            {
                return null;
            }
            return SharedStrings[index];
        }
        #endregion

        #region constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SharedStringsReader()
        {
            SharedStrings = new List<string>();
        }
        #endregion

        #region methods

        /// <summary>
        /// Reads the XML file form the passed stream and processes the shared strings table
        /// </summary>
        /// <param name="stream">Stream of the XML file</param>
        /// <exception cref="IOException">Throws IOException in case of an error</exception>
        public void Read(MemoryStream stream)
        {
            try
            {
                using (stream) // Close after processing
                {
                    XmlDocument xr = new XmlDocument();
                    xr.Load(stream);
                    StringBuilder sb = new StringBuilder();
                    XmlNodeList nodes = xr.DocumentElement.ChildNodes;
                    foreach (XmlNode node in xr.DocumentElement.ChildNodes)
                    {
                        if (node.LocalName.ToLower() == "si")
                        {
                            sb.Clear();
                            GetTextToken(node, ref sb);
                            if (sb.Length > 0)
                            {
                                SharedStrings.Add(sb.ToString());
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new IOException("XMLStreamException", "The XML entry could not be read from the input stream. Please see the inner exception:", ex);
            }


        }

        /// <summary>
        /// Function collects text tokens recursively in case of a split by formatting
        /// </summary>
        /// <param name="node">Root node to process</param>
        /// <param name="sb">StringBuilder reference</param>
        private void GetTextToken(XmlNode node, ref StringBuilder sb)
        {
            if (node.LocalName.ToLower() == "t" && string.IsNullOrEmpty(node.InnerText) == false)
            {
                sb.Append(node.InnerText);
            }

            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    GetTextToken(childNode, ref sb);
                }
            }
        }

        #endregion

    }
}

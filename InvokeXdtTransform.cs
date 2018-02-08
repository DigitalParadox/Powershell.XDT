using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Tracing;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Web.XmlTransform;
using Xdt.Powershell.Properties;

namespace Xdt.Powershell
{

    

    /// <inheritdoc />
    /// <summary>
    ///     A simple Cmdlet that outputs a greeting to the pipeline.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "XdtTransform")]
    [CmdletBinding]
    public class InvokeXdtTransform : Cmdlet
    {
        private  IXmlTransformationLogger _logger;

        public InvokeXdtTransform()
        {
            Logger = new PowershellTransformationLogger(this);
        }

        /// <summary>
        ///  XDT Transform Xml    
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "XDT Transform XML Text")]
        public XmlDocument TransformXML { get; set; } 

        /// <summary>
        ///  Target XML To Apply XDT Transform
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "The name of the person to greet", ValueFromPipeline = true)]
        public XmlDocument TargetXML { get; set; }

        
        /// <summary>
        ///  Provide Custom Logging Implementation for XML Transforms, Defaults to Powerhell Console Logging
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "The name of the person to greet", ValueFromPipeline = true)]
        public IXmlTransformationLogger Logger { get; set; }

        /// <summary>
        ///  Path to XDT Transform file
        /// </summary>
        [ValidatePathExists]
        [Parameter(Position = 0, HelpMessage = "Path to XDT Transform file", ValueFromPipeline = true)]
        public FileInfo TransformPath { get; set; }


        /// <summary>
        ///  Target XML To Apply XDT Transform
        /// </summary>
        
        [Parameter(Position = 0, HelpMessage = "Path to Output result file. This will leave the original target xml file intact and output the result to a separate fie.", ValueFromPipelineByPropertyName = true)]
        public FileInfo OutputPath { get; set; }


        
        /// <inheritdoc />
        /// <summary>
        ///     Perform Cmdlet processing.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (_logger != null)
            {
                var exception = new InvalidOperationException("Logger configuration provided is null or invalid.");

                var error = new ErrorRecord(exception, "InvalidOperationException", ErrorCategory.InvalidOperation, _logger);
                WriteError(error);
            }


            if (TransformPath != null)
            {
                WriteVerbose($"Loading XDT Transform from {TransformPath}");

                TransformXML = new XmlDocument();
                TransformXML.Load(TransformPath.FullName);

                WriteVerbose("done.");
                WriteVerbose($"{TransformXML.OuterXml}");
            }



            WriteVerbose("Validating Transform is valid XML Document Transform");
            var xdtSchema = XmlSchema.Read(new StringReader(Resources.XMLDocumentTransform), ValidationEventHandler);
            TransformXML.Schemas.Add(xdtSchema);
            TransformXML.Validate(ValidationEventHandler);
            WriteVerbose("done.");


            WriteVerbose("Convert Transform XML Text to XMLTransformation");
            var xdtTransform = new XmlTransformation( TransformXML.OuterXml , false, _logger);
            WriteVerbose("done.");


            WriteVerbose("Convert XML Document to convertable format.");
            var targetXml = new XmlTransformableDocument();
            var targetXmlReader = XmlReader.Create(new StringReader(TargetXML.OuterXml));
            targetXml.Load(targetXmlReader);
            WriteVerbose("done.");


            WriteVerbose("Apply XDT Transform to Target XML");
            WriteVerbose(TransformXML.ToString());
            WriteVerbose(TransformXML.OuterXml);
            xdtTransform.Apply(targetXml);
            WriteVerbose("done.");

            TargetXML = targetXml;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            var exception = new ParseException($"XML Validation Failed. - { args.Message }", args.Exception);
            switch (args.Severity)
            {
                case XmlSeverityType.Error:
                    WriteError(new ErrorRecord(exception, "ParserValidationError", ErrorCategory.ParserError, args.Message));
                    break;
                case XmlSeverityType.Warning:
                    WriteWarning(args.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(TargetXML);

            if (OutputPath != null)
            {
                WriteVerbose($"OutputPath {OutputPath} specified. Saving Transformed XML to this location.");
                TargetXML.Save(OutputPath.FullName);
            }
            base.EndProcessing();
        }
    }
}

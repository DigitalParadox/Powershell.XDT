using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Xdt.Powershell
{
    class ValidatePathExists : ValidateEnumeratedArgumentsAttribute
    {
        public PathType PathType { get; set; }

        public ValidatePathExists(PathType pathType = PathType.Any)
        {
            PathType = pathType;
        }

        protected override void ValidateElement(object element)
        {

            var path = (element as FileSystemInfo)?.FullName ?? element.ToString();

            var isValid = false;

            switch (PathType)
            {
                case PathType.Any:
                    isValid = Directory.Exists(path) || File.Exists(path); 
                    break;
                case PathType.Leaf:
                    isValid = File.Exists(path);
                    break;
                case PathType.Directory:
                    isValid = Directory.Exists(path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!isValid)
            {
                throw new ItemNotFoundException($"{element} was not found.");
            }
        }
    }
}
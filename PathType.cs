using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerShell.Cmdletization.Xml;

namespace Xdt.Powershell
{
    public enum PathType
    {
        Any,
        Leaf,
        Directory
    }
}

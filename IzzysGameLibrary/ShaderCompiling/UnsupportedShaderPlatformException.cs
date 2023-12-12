using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzzysGameLibrary.ShaderCompiling
{
    public class UnsupportedShaderPlatformException : Exception
    {
        public UnsupportedShaderPlatformException(string attemptedShaderPlatform) : base($"Unsupported shader platform {attemptedShaderPlatform}") { }
    }
}

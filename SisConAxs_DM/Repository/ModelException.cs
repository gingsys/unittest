using System;
using System.Text;

namespace SisConAxs_DM.Repository
{
    public class ModelException : Exception
    {
        public ModelException() : base() { }

        public ModelException(string message) : base(message) { }
        public ModelException(string message, Exception innerException) : base(message, innerException) { }

        public ModelException(string format, params object[] args) : base(string.Format(format, args)) { }
    }
}

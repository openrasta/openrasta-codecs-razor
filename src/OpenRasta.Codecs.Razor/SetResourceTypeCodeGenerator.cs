using System;
using System.Globalization;
using System.Web.Razor.Generator;

namespace OpenRasta.Codecs.Razor
{
    internal class SetResourceTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly string _genericTypeFormat;

        public SetResourceTypeCodeGenerator(string modelType, string genericTypeFormat) : base(modelType)
        {
            _genericTypeFormat = genericTypeFormat;
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return string.Format(CultureInfo.InvariantCulture, _genericTypeFormat, new object[2] { context.Host.DefaultBaseClass, baseType });
        }

        public override bool Equals(object obj)
        {
            var typeCodeGenerator = obj as SetResourceTypeCodeGenerator;
            
            if (typeCodeGenerator != null && base.Equals(obj))
            {
                return string.Equals(_genericTypeFormat, typeCodeGenerator._genericTypeFormat, StringComparison.Ordinal);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (_genericTypeFormat != null ? _genericTypeFormat.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return "Resource:" + BaseType;
        }
    }
}
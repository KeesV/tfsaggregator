using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

namespace Aggregator.Core.Extensions
{
    public class FieldValue : IConvertible
    {
        public Type UnderlyingType { get; set; }

        private readonly object value;

        private readonly ILogEvents logger;

        public FieldValue(Type underlyingType, ILogEvents logger)
        {
            this.UnderlyingType = underlyingType;
            this.logger = logger;
            this.value = null;
        }

        public FieldValue(Type underlyingType, ILogEvents logger, object value)
        {
            this.UnderlyingType = underlyingType;
            this.value = value;
            this.logger = logger;
        }

        public FieldValue(object value)
        {
            this.UnderlyingType = null;
            this.value = value;
            this.logger = null;
        }

        public static implicit operator double(FieldValue source)
        {
            if (source.logger != null)
            {
                source.logger.ValueAsignmentRequiresConversion(source.UnderlyingType, typeof(double), source.value);
            }

            if (source.UnderlyingType != null && source.UnderlyingType == typeof(double))
            {
                return (double)source.value;
            }

            return Convert.ToDouble(source.value, CultureInfo.InvariantCulture);
        }

        public static implicit operator DateTime(FieldValue source)
        {
            if (source.logger != null)
            {
                source.logger.ValueAsignmentRequiresConversion(source.UnderlyingType, typeof(DateTime), source.value);
            }

            if (source.UnderlyingType != null && source.UnderlyingType == typeof(DateTime))
            {
                return (DateTime)source.value;
            }

            return Convert.ToDateTime(source.value, CultureInfo.InvariantCulture);
        }

        public static implicit operator int(FieldValue source)
        {
            if (source.logger != null)
            {
                source.logger.ValueAsignmentRequiresConversion(source.UnderlyingType, typeof(int), source.value);
            }

            if (source.UnderlyingType != null && source.UnderlyingType == typeof(int))
            {
                return (int)source.value;
            }

            return Convert.ToInt32(source.value, CultureInfo.InvariantCulture);
        }

        public static implicit operator string(FieldValue source)
        {
            if (source.logger != null)
            {
                source.logger.ValueAsignmentRequiresConversion(source.UnderlyingType, typeof(string), source.value);
            }

            if (source.value == null)
            {
                return null;
            }

            if (source.UnderlyingType != null && source.UnderlyingType == typeof(string))
            {
                return (string)source.value;
            }

            return Convert.ToString(source.value, CultureInfo.InvariantCulture);
        }

        public static implicit operator Guid(FieldValue source)
        {
            if (source.logger != null)
            {
                source.logger.ValueAsignmentRequiresConversion(source.UnderlyingType, typeof(Guid), source.value);
            }

            if (source.UnderlyingType != null && source.UnderlyingType == typeof(Guid))
            {
                return (Guid)source.value;
            }

            return Guid.Parse(source.value.ToString());
        }

        public static implicit operator FieldValue(double? source)
        {
            return new FieldValue(null, null, source.HasValue ? source.Value : (object)null);
        }

        public static implicit operator FieldValue(DateTime? source)
        {
            return new FieldValue(null, null, source.HasValue ? source.Value : (object)null);
        }

        public static implicit operator FieldValue(int? source)
        {
            return new FieldValue(null, null, source.HasValue ? source.Value : (object)null);
        }

        public static implicit operator FieldValue(string source)
        {
            return new FieldValue(null, null, source);
        }

        public static implicit operator FieldValue(Guid? source)
        {
            return new FieldValue(null, null, source.HasValue ? source.Value : (object)null);
        }

        public override bool Equals(object other)
        {
            return other is FieldValue && this.Equals((FieldValue)other);
        }

        public bool Equals(FieldValue other)
        {
            if (this.value == null && other.value == null)
            {
                return true;
            }

            if (this.value == null && other.value != null)
            {
                return false;
            }

            return this.value.Equals(other.value);
        }

        public override int GetHashCode()
        {
            return this.value?.GetHashCode() ?? int.MinValue;
        }

        public object Value(Type t)
        {
            return Cast(this.value, t);
        }

        private static object Cast(object obj, Type t)
        {
            try
            {
                var param = Expression.Parameter(typeof(object));
                return Expression.Lambda(Expression.Convert(param, t), param)
                    .Compile().DynamicInvoke(obj);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        public string ToString(IFormatProvider provider)
        {
            return (string)this;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Cast(this, conversionType);
        }

        public override string ToString()
        {
            return this.value?.ToString() ?? "null";
        }
    }
}
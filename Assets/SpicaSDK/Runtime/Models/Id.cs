using System;
using SpicaSDK.Runtime.Utils;

namespace SpicaSDK.Services.Models
{
    public readonly struct Id
    {
        private static Id empty = new Id(String.Empty);
        public static Id Empty => empty;

        public static Id From(object value) => new Id(value.ToString());
        
        public readonly string Value;

        public Id(string _id)
        {
            Value = _id;
        }

        public bool Equals(Id other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Id other)
                return Equals(other);
            
            SpicaLogger.Instance.Log("Comparing id with non id object");
            return Equals(From(obj));
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
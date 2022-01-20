using System;

namespace SpicaSDK.Services.Models
{
    public readonly struct Id
    {
        private static Id empty = new Id(String.Empty);
        public static Id Empty => empty;

        public static Id From(string value) => new Id(value);
        
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
            return obj is Id other && Equals(other);
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
using System;

namespace R
{
    public class RsqlgnoreAttribute : Attribute
    {
        public RsqlgnoreAttribute()
        {
            
        }
        public RsqlgnoreAttribute(bool b)
        {

        }
    }
    public class RsqlDateTimeAttribute : Attribute
    {
        public RsqlDateTimeAttribute()
        {

        }
        public RsqlDateTimeAttribute(bool b)
        {

        }
    }
    public class RsqlFloatAttribute : Attribute
    {
        public RsqlFloatAttribute(bool b)
        {
            
        }
    }

    public class RsqlDoubleAttribute : Attribute
    {
        public RsqlDoubleAttribute()
        {
            
        }
        public RsqlDoubleAttribute(bool b)
        {

        }
    }

    public class RsqlIntAttribute : Attribute
    {
        public RsqlIntAttribute()
        {
            
        }
        public RsqlIntAttribute(bool b)
        {

        }
    }

    public class RsqlVarcharAttribute : Attribute
    {
        public int Value { get; set; }
        public RsqlVarcharAttribute(int i)
        {
            Value = i;
        }
    }

    public class RsqlTextAttribute : Attribute
    {
        public RsqlTextAttribute()
        {

        }
        public RsqlTextAttribute(bool b)
        {

        }
    }

}

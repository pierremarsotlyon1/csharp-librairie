using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.Annotations
{
    public class MyDateTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime d = Convert.ToDateTime(value);
                return d <= DateTime.Now && d >= now.AddYears(-150);
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}

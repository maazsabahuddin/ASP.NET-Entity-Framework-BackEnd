using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crud.Models
{
    public class DtoResponseModel
    {
        public int emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_email { get; set; }
        public string emp_phone { get; set; }
        public string dname { get; set; }
    }
}
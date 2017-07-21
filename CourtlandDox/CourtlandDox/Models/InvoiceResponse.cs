using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtlandDox.Models
{
    
    public class InvoiceResponse
    {
        public List<Field> fields { get; set; }
        public object[] invoiceNumberProbables { get; set; }
        public object[] invoiceDateProbables { get; set; }
        public object[] invoiceTotalProbables { get; set; }
    }
    

}
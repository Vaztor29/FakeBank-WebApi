//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FakeBank.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Transaction
    {
        public System.Guid Id { get; set; }
        public System.DateTime Date { get; set; }
        public System.Guid IdSourceAccount { get; set; }
        public System.Guid IdDestinationAccount { get; set; }
        public double Amount { get; set; }
        public int Type { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Account Account1 { get; set; }
    }
}

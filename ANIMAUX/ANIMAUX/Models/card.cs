//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ANIMAUX.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class card
    {
        public int id { get; set; }
        public System.DateTime date_added { get; set; }
        public int organisation_id { get; set; }
        public int district_id { get; set; }
        public int animal_id { get; set; }
    
        public virtual animal animal { get; set; }
        public virtual district district { get; set; }
        public virtual organisation organisation { get; set; }
    }
}
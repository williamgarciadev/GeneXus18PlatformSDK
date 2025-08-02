using System;

namespace Acme.Packages.Menu
{
    public class ObjectHistory
    {
        public int Id { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }

}
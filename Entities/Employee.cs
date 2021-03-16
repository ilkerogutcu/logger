using Core.Entities;

namespace Entities
{
    public class Employee:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public decimal Salary { get; set; }
    }
}
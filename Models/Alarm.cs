using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmClock.Models
{
    [Table("Alarms")]
    public class Alarm
    {
        [Key]
        public int Id { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool Enabled { get; set; }
        public string? Label { get; set; }
        public int RepeatMask { get; set; }
    }
}

using System.Collections.Generic;

namespace Can_I_retire_yet.Models
{
    public class SavedData
    {
        public List<List<string>> assets { get; set; }
        public List<List<string>> cash { get; set; }
        public List<List<string>> savings { get; set; }
        public List<List<string>> bonds { get; set; }
        public List<List<string>> stocks_shares { get; set; }
        public List<List<string>> income { get; set; }
        public List<List<string>> expenses { get; set; }
        public List<List<string>> future_income { get; set; }
        public List<List<string>> future_expenses { get; set; }
        public string salary { get; set; }
        public string inflation { get; set; }
        public string currency { get; set; }
        public string age { get; set; }
        public string length { get; set; }

    }
}
using System;

namespace csdb.Database.Models
{
    public class Auftrag
    {
        public virtual int Aufnr { get; set; }
        public virtual DateTime AufDat { get; set; }
        public virtual DateTime? ErlDat { get; set; }
        public virtual decimal? Dauer { get; set; }
        public virtual int? Anfahrt { get; set; }
        public virtual string? Beschreibung { get; set; }
        public virtual Mitarbeiter Mitarbeiter { get; set; } = null!;
        public virtual Kunde Kunde { get; set; } = null!;
    }
}

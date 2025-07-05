namespace csdb.Database.Models
{
    public class Kunde
    {
        public virtual int KunNr { get; set; }
        public virtual string KunName { get; set; } = null!;
        public virtual string KunOrt { get; set; } = null!;
        public virtual string KunPLZ { get; set; } = null!;
        public virtual string KunStrasse { get; set; } = null!;
    }
}

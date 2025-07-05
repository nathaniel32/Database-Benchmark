namespace csdb.Database.Models
{
    public class Ersatzteil
    {
        public virtual string EtID { get; set; } = null!;
        public virtual string EtBezeichnung { get; set; } = null!;
        public virtual decimal EtPreis { get; set; }
        public virtual int EtAnzLager { get; set; }
        public virtual string EtHersteller { get; set; } = null!;
    }
}

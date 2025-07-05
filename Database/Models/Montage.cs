namespace csdb.Database.Models
{
    public class Montage
    {
        public virtual int Anzahl { get; set; }
        public virtual Auftrag Auftrag { get; set; } = null!;
        public virtual Ersatzteil Ersatzteil { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Montage other = (Montage)obj;
            return Auftrag.Aufnr == other.Auftrag.Aufnr && Ersatzteil.EtID == other.Ersatzteil.EtID;
        }

        public override int GetHashCode()
        {
            return (Auftrag.Aufnr + "|" + Ersatzteil.EtID).GetHashCode();
        }
    }
}

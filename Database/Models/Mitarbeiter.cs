namespace csdb.Database.Models {
    public class Mitarbeiter {
        public virtual string MitID { get; set; } = null!;  // Primary key CHAR
        public virtual string MitName { get; set; } = null!;
        public virtual string? MitVorname { get; set; }  // Nullable
        public virtual DateTime MitGebDat { get; set; }
        public virtual string MitJob { get; set; } = null!;
        public virtual decimal? MitStundensatz { get; set; }  // Nullable smallmoney
        public virtual string? MitEinsatzort { get; set; }
    }
}
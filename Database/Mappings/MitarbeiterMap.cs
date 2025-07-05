using FluentNHibernate.Mapping;
using csdb.Database.Models;

namespace csdb.Database.Mappings {
    public class MitarbeiterMap : ClassMap<Mitarbeiter> {
        public MitarbeiterMap() {
            Table("Mitarbeiter");
            Id(x => x.MitID).Column("MitID").GeneratedBy.Assigned();  // CHAR primary key, not auto-increment
            Map(x => x.MitName).Column("MitName").Not.Nullable();
            Map(x => x.MitVorname).Column("MitVorname").Nullable();
            Map(x => x.MitGebDat).Column("MitGebDat").Not.Nullable();
            Map(x => x.MitJob).Column("MitJob").Not.Nullable();
            Map(x => x.MitStundensatz).Column("MitStundensatz").Nullable();
            Map(x => x.MitEinsatzort).Column("MitEinsatzort").Nullable();
        }
    }
}
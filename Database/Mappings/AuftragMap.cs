using csdb.Database.Models;
using FluentNHibernate.Mapping;

namespace csdb.Database.Mappings
{
    public class AuftragMap : ClassMap<Auftrag>
    {
        public AuftragMap()
        {
            Table("Auftrag");
            Id(x => x.Aufnr).GeneratedBy.Assigned();
            Map(x => x.AufDat);
            Map(x => x.ErlDat).Nullable();
            Map(x => x.Dauer).Nullable();
            Map(x => x.Anfahrt).Nullable();
            Map(x => x.Beschreibung).Nullable();
            References(x => x.Mitarbeiter).Column("MitID");
            References(x => x.Kunde).Column("KunNr");
        }
    }
}

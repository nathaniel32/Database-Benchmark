using csdb.Database.Models;
using FluentNHibernate.Mapping;

namespace csdb.Database.Mappings
{
    public class MontageMap : ClassMap<Montage>
    {
        public MontageMap()
        {
            Table("Montage");
            CompositeId()
                .KeyReference(x => x.Auftrag, "AufNr")
                .KeyReference(x => x.Ersatzteil, "EtID");
            Map(x => x.Anzahl);
        }
    }
}

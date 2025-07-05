using csdb.Database.Models;
using FluentNHibernate.Mapping;

namespace csdb.Database.Mappings
{
    public class ErsatzteilMap : ClassMap<Ersatzteil>
    {
        public ErsatzteilMap()
        {
            Table("Ersatzteil");
            Id(x => x.EtID).GeneratedBy.Assigned();
            Map(x => x.EtBezeichnung);
            Map(x => x.EtPreis);
            Map(x => x.EtAnzLager);
            Map(x => x.EtHersteller);
        }
    }
}

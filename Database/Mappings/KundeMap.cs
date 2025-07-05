using csdb.Database.Models;
using FluentNHibernate.Mapping;

namespace csdb.Database.Mappings
{
    public class KundeMap : ClassMap<Kunde>
    {
        public KundeMap()
        {
            Table("Kunde");
            Id(x => x.KunNr).GeneratedBy.Assigned();
            Map(x => x.KunName);
            Map(x => x.KunOrt);
            Map(x => x.KunPLZ);
            Map(x => x.KunStrasse);
        }
    }
}

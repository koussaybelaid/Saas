using EM.Data.Infrastructure;
using EM.Domain.Entities;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Service
{
    public class Evenement_service : Service<Evenement>, Ievenementservice
    {
        static IDataBaseFactory dbf = new DataBaseFactory();

        static IUnitOfWork uow = new UnitOfWork(dbf);

        public Evenement_service() : base(uow)
        {
        }
    }
}

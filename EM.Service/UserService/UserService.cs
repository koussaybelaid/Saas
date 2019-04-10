using EM.Data;
using EM.Data.Infrastructure;
using EM.Domain.Entities;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Service.UserService
{
    public class UserService : Service<ApplicationUser>, IUserService
    {
        static IDataBaseFactory Factory = new DataBaseFactory();//l'usine de fabrication du context
        static IUnitOfWork utk = new UnitOfWork(Factory);//unité de travail a besoin du factory pour communiquer avec la base
        public UserService():base(utk)
        {

        }
    }
}
